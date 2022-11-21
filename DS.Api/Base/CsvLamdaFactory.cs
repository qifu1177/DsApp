using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using DS.Api.Extensions;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DS.Api.Base
{
    public class CsvLamdaFactory
    {
        private object _lockFlag = new object();
        public static CsvLamdaFactory Instacne { get; private set; }
        static CsvLamdaFactory()
        {
            Instacne = new CsvLamdaFactory();
        }
        private Dictionary<string, Action<List<DtValue>, string>> _lamdaDic
            = new Dictionary<string, Action<List<DtValue>, string>>();
        public Action<List<DtValue>, string> GetLamda(string jsonStr)
        {
            if (_lamdaDic.ContainsKey(jsonStr))
                return _lamdaDic[jsonStr];
            else
            {
                lock (_lockFlag)
                {
                    if (!_lamdaDic.ContainsKey(jsonStr))
                    {
                        var options = JsonConvert.DeserializeObject<CsvExpressionOptions>(jsonStr);
                        _lamdaDic[jsonStr] = CreateAction(options);
                    }
                    return _lamdaDic[jsonStr];
                }
            }
        }

        protected virtual Action<List<DtValue>, string> CreateAction(CsvExpressionOptions options)
        {
            ConstantExpression constantExpression = Expression.Constant(options.Delimiter);
            ParameterExpression listParameterExpression = Expression.Parameter(typeof(List<DtValue>), "list");
            ParameterExpression strParameterExpression = Expression.Parameter(typeof(string), "str");
            ParameterExpression strsParameterExpression = Expression.Parameter(typeof(string[]), "strs");
            var methodInfo = typeof(string).GetMethod("Split", BindingFlags.Instance | BindingFlags.Public,new Type[] {typeof(String),typeof(StringSplitOptions) });
            MethodCallExpression spliteExpression = Expression.Call(strParameterExpression, methodInfo,
                new Expression[] { constantExpression });
            BinaryExpression assignExpression = Expression.Assign(strsParameterExpression, spliteExpression);
            ConstantExpression lengthExpression = Expression.Constant(options.ArrayLength);
            BinaryExpression equalExpression = Expression.Equal(Expression.ArrayLength(strsParameterExpression), lengthExpression);

            BlockExpression addDtValueExpression = Expression.Block(new Expression[]
            { 
                //for add DtValue to list
                Expression.Call(listParameterExpression, typeof(List<DtValue>).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public),
                new Expression[]
                {
                    Expression.Call(null,typeof(DtValue).GetMethod("Create",BindingFlags.Static|BindingFlags.Public),new Expression[]
                    {
                        CreateDtExpression(options,strsParameterExpression),
                        CreateValueExpression(options,strsParameterExpression)
                    }),
                })
            });
            ConditionalExpression ifExpression = Expression.IfThen(equalExpression, addDtValueExpression);
            BlockExpression blockExpression = Expression.Block(new Expression[] { assignExpression, ifExpression });
            Expression<Action<List<DtValue>, string>> expression = Expression.Lambda<Action<List<DtValue>, string>>(blockExpression,
                new ParameterExpression[] { listParameterExpression, strParameterExpression });
            return expression.Compile();
        }

        private Expression CreateDtExpression(CsvExpressionOptions options, ParameterExpression strsParameterExpression)
        {
            string datePattern = options.DateTimeOption.DateFormat;
            datePattern = datePattern.Replace("yyyy", "\\d{4}");
            datePattern = datePattern.Replace("MM", "\\d{2}");
            datePattern = datePattern.Replace("dd", "\\d{2}");
            string timePattern = "\\d{2}:\\d{2}:\\d{2}";

            ParameterExpression dateFormatExpression = Expression.Parameter(typeof(string), "dateFormatStr");
            Expression.Assign(dateFormatExpression, Expression.Constant(options.DateTimeOption.DateFormat));
            ConstantExpression timeFormatExpression = Expression.Constant(options.DateTimeOption.TimeFormat);

            ParameterExpression dayPositionExpression = Expression.Parameter(typeof(int), "dayIndex");
            Expression.Assign(dayPositionExpression, Expression.Call(dateFormatExpression, typeof(string).GetMethod("IndexOf", BindingFlags.Public | BindingFlags.Instance),
                    new Expression[] { Expression.Constant("dd") }));
            ParameterExpression monthPositionExpression = Expression.Parameter(typeof(int), "monthIndex");
            Expression.Assign(monthPositionExpression, Expression.Call(dateFormatExpression, typeof(string).GetMethod("IndexOf", BindingFlags.Public | BindingFlags.Instance),
                    new Expression[] { Expression.Constant("MM") }));
            ParameterExpression yearPositionExpression = Expression.Parameter(typeof(int), "yearIndex");
            Expression.Assign(yearPositionExpression, Expression.Call(dateFormatExpression, typeof(string).GetMethod("IndexOf", BindingFlags.Public | BindingFlags.Instance),
                    new Expression[] { Expression.Constant("yyyy") }));

            ParameterExpression dateStrExpression = Expression.Parameter(typeof(string), "dateStr");
            Expression dateMatchExpression = Expression.Call(null, typeof(Regex).GetMethod("Match", BindingFlags.Static | BindingFlags.Public),
                new Expression[] { Expression.ArrayAccess(strsParameterExpression, Expression.Constant(options.DateTimeOption.DatePosition)),
                    Expression.Constant(datePattern) });           
            Expression dtstrMatchsExpression = Expression.Parameter(typeof(MatchCollection), "dtstrMatchs");
            Expression.Assign(dtstrMatchsExpression, Expression.Call(null, typeof(Regex).GetMethod("Matches", BindingFlags.Static | BindingFlags.Public),
                new Expression[]
                {
                    Expression.Property(dateMatchExpression, "Value"),
                    Expression.Constant("\\d{2,4}")
                }));
            Expression.Assign(dateStrExpression, Expression.Call(null,typeof(string).GetMethod("Format",BindingFlags.Public|BindingFlags.Static),
                new Expression[]
                {
                    Expression.Constant("{0}-{1}-{2}"),
                    Expression.Property(Expression.ArrayAccess(dtstrMatchsExpression,yearPositionExpression),"Value"),
                    Expression.Property(Expression.ArrayAccess(dtstrMatchsExpression,monthPositionExpression),"Value"),
                    Expression.Property(Expression.ArrayAccess(dtstrMatchsExpression,dayPositionExpression),"Value")
                }));

            ParameterExpression timeStrExpression = Expression.Parameter(typeof(string), "timeStr");
            Expression timeMatchEXpression = Expression.Call(null, typeof(Regex).GetMethod("Match", BindingFlags.Static | BindingFlags.Public),
                new Expression[] { Expression.ArrayAccess(strsParameterExpression, Expression.Constant(options.DateTimeOption.TimePosition)) ,
                    Expression.Constant(timePattern)});
            Expression.Assign(timeStrExpression, Expression.Property(timeMatchEXpression, "Value"));

            Expression dateTimeExpression = Expression.Call(null, typeof(Convert).GetMethod("ToDateTime", BindingFlags.Public | BindingFlags.Static),
                Expression.Call(null, typeof(string).GetMethod("Format", BindingFlags.Public | BindingFlags.Static),
                    new Expression[]
                    {
                        Expression.Constant("{0} {1}"),
                        dateStrExpression,timeStrExpression
                    }));
            return Expression.New(typeof(DateTimeOffset).GetConstructor(BindingFlags.Public, new Type[] { typeof(DateTime) }), dateTimeExpression);
        }

        private Expression CreateValueExpression(CsvExpressionOptions options, ParameterExpression strsParameterExpression)
        {
            ParameterExpression valueExpression = Expression.Parameter(typeof(double), "value");
            ParameterExpression valueStrExpression = Expression.Parameter(typeof(string), "valueStr");
            ConstantExpression valuePositionExpression = Expression.Constant(options.ValueOption.Position);
            ConstantExpression decimalExpression = Expression.Constant(options.ValueOption.Decimal);
            ConstantExpression pointExpression = Expression.Constant(".");
            ParameterExpression tempStrExpression = Expression.Parameter(typeof(string), "tempStr");
            Expression.IfThenElse(Expression.Equal(decimalExpression, pointExpression),
                Expression.Assign(valueStrExpression, Expression.ArrayAccess(strsParameterExpression, valuePositionExpression)
                ), Expression.Block(new Expression[]
                {
                    Expression.Assign(tempStrExpression,Expression.ArrayAccess(strsParameterExpression,valuePositionExpression)),
                    Expression.Assign(valueStrExpression,
                        Expression.Call(tempStrExpression,typeof(string).GetMethod("Replace",BindingFlags.Instance|BindingFlags.Public),
                            new Expression[]{decimalExpression,pointExpression}))
                }));
            Expression.Assign(valueExpression, Expression.Call(null, typeof(Convert).GetMethod("ToDouble", BindingFlags.Static | BindingFlags.Public),
            new Expression[]
            {
                    valueStrExpression
            }));
            return valueExpression;
        }
    }
}
