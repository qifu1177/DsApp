using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using DS.Api.Extensions;
using Newtonsoft.Json;
using System.Globalization;
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

        private Action<List<DtValue>, string> CreateAction(CsvExpressionOptions options)
        {
            var kvOptions = CreateDataTimeOption(options.DateTimeOption);
            Func<List<string>, Dictionary<EnumDatas.DateEnum, int>, DtValue> createDtValue = CreateDtValueExpression(options).Compile();
            return (dtList, str) =>
            {
                List<string> list = str.Split(options.Delimiter).ToList();
                Dictionary<EnumDatas.DateEnum, int> dts = GetDateTimeValues(list, options.DateTimeOption, kvOptions);
                if (dts != null)
                {
                    dtList.Add(createDtValue.Invoke(list, dts));
                }
            };
        }

        private Expression<Func<List<string>, Dictionary<EnumDatas.DateEnum, int>, DtValue>> CreateDtValueExpression(CsvExpressionOptions option)
        {
            ParameterExpression list = Expression.Parameter(typeof(List<string>), "list");
            ParameterExpression dtKv = Expression.Parameter(typeof(Dictionary<EnumDatas.DateEnum, int>), "dtKv");
            Expression dtExpression = CreateDateTimeExpression(option);
            Expression valueExpression = CreateValueExpression(option);
            List<MemberBinding> bindings = new List<MemberBinding>();
            foreach (var item in typeof(DtValue).GetProperties())
            {
                if (item.Name == "Dt")
                {
                    MemberBinding memberBinding = Expression.Bind(item, Expression.Invoke(dtExpression, dtKv));
                    bindings.Add(memberBinding);
                }
                else if (item.Name == "Value")
                {
                    MemberBinding memberBinding = Expression.Bind(item, Expression.Invoke(valueExpression, list));
                    bindings.Add(memberBinding);
                }

            }
            MemberInitExpression init = Expression.MemberInit(Expression.New(typeof(DtValue)), bindings.ToArray());

            return Expression.Lambda<Func<List<string>, Dictionary<EnumDatas.DateEnum, int>, DtValue>>(init, list, dtKv);
        }
        private Expression CreateDateTimeExpression(CsvExpressionOptions option)
        {
            Dictionary<EnumDatas.DateEnum, int> dtOptionKv = CreateDataTimeOption(option.DateTimeOption);
            Expression<Func<Dictionary<EnumDatas.DateEnum, int>, DateTimeOffset>> lamdaEx = (dic) => new DateTimeOffset(dic[EnumDatas.DateEnum.Year], dic[EnumDatas.DateEnum.Month], dic[EnumDatas.DateEnum.Day], dic[EnumDatas.DateEnum.Hour], dic[EnumDatas.DateEnum.Minut], dic[EnumDatas.DateEnum.Second], TimeSpan.Zero);
            return lamdaEx;
        }
        private Expression CreateValueExpression(CsvExpressionOptions option)
        {
            Expression<Func<List<string>, double>> lamdaEx = (list) => Convert.ToDouble(list[option.ValueOption.Position].Replace(option.ValueOption.Decimal , "."));
            return lamdaEx;
        }
        private Dictionary<EnumDatas.DateEnum, int> GetDateTimeValues(List<string> list, DateTimeOption option, Dictionary<EnumDatas.DateEnum, int> dtOptionKv)
        {
            if (option.DatePosition == option.TimePosition)
            {
                string[] strs = list[option.DatePosition].Split(option.DateTimeDelimiter);
                if (strs.Length != 2)
                    return null;
                return GetDateTimeValues(strs[0], strs[1], option, dtOptionKv);
            }

            return GetDateTimeValues(list[option.DatePosition], list[option.TimePosition], option, dtOptionKv);
        }
        private Dictionary<EnumDatas.DateEnum, int> GetDateTimeValues(string dtStr, string timeStr, DateTimeOption option, Dictionary<EnumDatas.DateEnum, int> dtOptionKv)
        {
            if (string.IsNullOrWhiteSpace(dtStr) || string.IsNullOrWhiteSpace(timeStr))
                return null;
            string[] dts = dtStr.Split(option.DateDelimiter);
            string[] times = timeStr.Split(":");
            if (dts.Length != 3 || times.Length < 2)
                return null;
            Dictionary<EnumDatas.DateEnum, int> dtKv = new Dictionary<EnumDatas.DateEnum, int>();

            dtKv[EnumDatas.DateEnum.Year] = Convert.ToInt32(dts[dtOptionKv[EnumDatas.DateEnum.Year]]);
            dtKv[EnumDatas.DateEnum.Month] = Convert.ToInt32(dts[dtOptionKv[EnumDatas.DateEnum.Month]]);
            dtKv[EnumDatas.DateEnum.Day] = Convert.ToInt32(dts[dtOptionKv[EnumDatas.DateEnum.Day]]);
            dtKv[EnumDatas.DateEnum.Hour] = Convert.ToInt32(times[dtOptionKv[EnumDatas.DateEnum.Hour]]);
            dtKv[EnumDatas.DateEnum.Minut] = Convert.ToInt32(times[dtOptionKv[EnumDatas.DateEnum.Minut]]);
            dtKv[EnumDatas.DateEnum.Second] = dtOptionKv.ContainsKey(EnumDatas.DateEnum.Second) ? Convert.ToInt32(times[dtOptionKv[EnumDatas.DateEnum.Second]]) : 0;
            return dtKv;
        }
        private Dictionary<EnumDatas.DateEnum, int> CreateDataTimeOption(DateTimeOption option)
        {
            Dictionary<EnumDatas.DateEnum, int> keyValues = new Dictionary<EnumDatas.DateEnum, int>();
            string[] dts = option.DateFormat.Split(option.DateDelimiter);
            for (int i = 0; i < dts.Length; i++)
            {
                if (dts[i] == "dd")
                    keyValues[EnumDatas.DateEnum.Day] = i;
                else if (dts[i] == "MM")
                    keyValues[EnumDatas.DateEnum.Month] = i;
                else if (dts[i] == "yyyy")
                    keyValues[EnumDatas.DateEnum.Year] = i;
            }
            string[] times = option.TimeFormat.Split(":");
            for (int i = 0; i < times.Length; i++)
            {
                if (times[i] == "HH")
                    keyValues[EnumDatas.DateEnum.Hour] = i;
                else if (times[i] == "mm")
                    keyValues[EnumDatas.DateEnum.Minut] = i;
                else if (times[i] == "ss")
                    keyValues[EnumDatas.DateEnum.Second] = i;
            }
            return keyValues;
        }

    }
}
