using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using DS.Api.Extensions;
using Newtonsoft.Json;
using System.Linq;
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
        private Dictionary<string, Action<List<DtValue>, string, string[], double>> _lamdaDic
            = new Dictionary<string, Action<List<DtValue>, string, string[], double>>();
        public Action<List<DtValue>, string, string[], double> GetLamda(string jsonStr)
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

        protected virtual Action<List<DtValue>, string, string[],double> CreateAction(CsvExpressionOptions options)
        {
            ParameterExpression list = Expression.Parameter(typeof(List<DtValue>), "list");
            ParameterExpression str = Expression.Parameter(typeof(String), "str");
            ParameterExpression strs = Expression.Parameter(typeof(String[]), "strs");
            ParameterExpression value = Expression.Parameter(typeof(double), "value");
            var methodInfo = typeof(String).GetMethod("Split", BindingFlags.Instance | BindingFlags.Public, new Type[] { typeof(String), typeof(StringSplitOptions) });
            MethodInfo listAdd = typeof(List<DtValue>).GetMethod("Add", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo createDt = typeof(DtValue).GetMethod("Create", BindingFlags.Static | BindingFlags.Public);
            MethodCallExpression spliteExpression = Expression.Call(str, methodInfo,
                new Expression[] {
                    Expression.Constant(options.Delimiter),
                    Expression.Constant(StringSplitOptions.None)
                });
            Expression excuite = Expression.Block(
                Expression.Assign(strs, spliteExpression),
                Expression.IfThen(
                    Expression.Equal(Expression.ArrayLength(strs), Expression.Constant(options.ArrayLength)),
                        Expression.Call(list,listAdd,
                            Expression.Call(null, createDt, new Expression[]
                            {
                                CreateConvertToDateTime(strs,options.DateTimeOption.DateFormat,
                                    options.DateTimeOption.DatePosition,options.DateTimeOption.TimePosition),
                                CreateConvertToDouble(strs,value,options.ValueOption.Position,options.ValueOption.Decimal)
                            }
                        )
                    )));
            Expression<Action<List<DtValue>, string, string[],double>> expression = Expression.Lambda<Action<List<DtValue>, string, string[],double>>(excuite,
                new ParameterExpression[] { list, str, strs,value });
            return expression.Compile();
        }

                
        public Expression CreateConvertToDouble(ParameterExpression strs, ParameterExpression value,int valueIndex,string decimalPoint)
        {
            Expression valueAccess = Expression.ArrayAccess(strs, Expression.Constant(valueIndex));
            LabelTarget returnLabel = Expression.Label(typeof(double));
            MethodInfo replace = typeof(string).GetMethod("Replace", BindingFlags.Instance | BindingFlags.Public, new[] { typeof(string), typeof(string) });
            MethodInfo toDouble = typeof(Convert).GetMethod("ToDouble", BindingFlags.Public | BindingFlags.Static, new Type[] { typeof(string) });
            Expression calculate = Expression.Block(
                Expression.IfThen(
                    Expression.NotEqual(Expression.Constant(decimalPoint), Expression.Constant(".")),
                    Expression.Assign(valueAccess, Expression.Call(valueAccess, replace, new[] { Expression.Constant(decimalPoint), Expression.Constant(".") }))
                    ),
                Expression.Call(null, typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) }), valueAccess),
                Expression.Assign(value, Expression.Call(null, toDouble, new[] { valueAccess })),
                Expression.Return(returnLabel, value),
                Expression.Label(returnLabel, value)
                );

            return calculate;
        }
        public Expression CreateConvertToDateTime(ParameterExpression strs,string formatOption, int dateIndex, int timeIndex)
        {
            string separation = GetSeparation(formatOption);            
            string format = formatOption.Replace("yyyy", "\\d{4}");
            format = format.Replace("MM", "\\d{2}");
            format = format.Replace("dd", "\\d{2}");
            var dtformats = formatOption.Split(separation).ToList();
            int monthIndex = dtformats.IndexOf("MM");
            int dayIndex = dtformats.IndexOf("dd");
            string timeFormat = @"\d{2}:\d{2}:\d{2}";
            Expression<Func<string, string>> getTimeStrLamda = (str) => Regex.Match(str, timeFormat).Value;
            Expression<Func<string, string>> yearStrLamda = (str) => Regex.Match(Regex.Match(str, format).Value, "\\d{4}").Value;
            Expression<Func<string, string>> monthStrLamda = (str) => Regex.Matches(Regex.Match(str, format).Value, "\\d{2}")[monthIndex].Value;
            Expression<Func<string, string>> dayStrLamda = (str) => Regex.Matches(Regex.Match(str, format).Value, "\\d{2}")[dayIndex].Value;
            Expression<Func<string, string, string, string, string>> toDtFomratStrLamda = (year, month, day, time) => string.Format("{0}-{1}-{2} {3}", year, month, day, time);
            Expression dateAccess = Expression.ArrayAccess(strs, Expression.Constant(dateIndex));
            Expression timeAccess = Expression.ArrayAccess(strs, Expression.Constant(timeIndex));
            Expression toDtFormatStr = Expression.Invoke(toDtFomratStrLamda, new[]
            {
                Expression.Invoke(yearStrLamda,dateAccess),
                Expression.Invoke(monthStrLamda,dateAccess),
                Expression.Invoke(dayStrLamda,dateAccess),
                Expression.Invoke(getTimeStrLamda,timeAccess)
            });
            Expression calculate = Expression.Call(null,
                typeof(Convert).GetMethod("ToDateTime", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) }),
                new[] { toDtFormatStr }
                );
            var constuctor = typeof(DateTimeOffset).GetConstructor(new[] { typeof(DateTime) });
            var expression = Expression.New(constuctor, new[] { calculate });
            return expression;
        }
        private string GetSeparation(string formatOp)
        {
            string separation = "-";
            if (formatOp.IndexOf("MM") > 0)
                separation = formatOp[formatOp.IndexOf("MM") - 1].ToString();
            else
                separation = formatOp[formatOp.IndexOf("yyyy") - 1].ToString();
            return separation;
        }

    }
}
