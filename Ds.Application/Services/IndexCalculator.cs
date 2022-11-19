using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using Ds.Infrastructure.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Application.Services
{
    public class IndexCalculator : IIndexCalculator
    {
        public IEnumerable<IDtValue> IndexActivitiesCalculate(IEnumerable<IDtValue> datas, double deltaTime, double minValue = 0, double maxValue = double.MaxValue)
        {
            List<DtValue> results = new List<DtValue>();
            List<IDtValue> values = new List<IDtValue>();
            foreach (IDtValue value in datas)
            {
                if (values.Any())
                {
                    if ((value.Dt - values[0].Dt).TotalSeconds > deltaTime)
                    {
                        var dtval = DtValue.Create(values[0].Dt, IndexActivityCalculate(values, minValue, maxValue));
                        results.Add(dtval);

                        values.Clear();
                    }
                }
                values.Add(value);
            }
            return results;
        }
        private double IndexActivityCalculate(List<IDtValue> values, double minValue, double maxValue)
        {
            double sum = 0;
            if (values[0].Value < minValue)
                values[0].Value = minValue;
            if (values[0].Value > maxValue)
                values[0].Value = maxValue;
            for (int i = 1; i < values.Count; i++)
            {
                if (values[i].Value < minValue)
                    values[i].Value = minValue;
                if (values[i].Value > maxValue)
                    values[i].Value = maxValue;
                sum += Math.Abs(values[i].Value - values[i - 1].Value);
            }
            if (values.Count > 1)
                return sum / (values.Count - 1);
            return sum;
        }
        public IEnumerable<IStatusData> CaculateStatus(IEnumerable<IDtValue> datas, double standbyLimit, double minDuration)
        {
            List<StatusData> list = new List<StatusData>();
            IDtValue lastDV = null;
            StatusData currentStatusData = null;
            foreach (var item in datas)
            {
                lastDV = item;
                string status = item.Value <= standbyLimit ? AppEnums.Status.Standby.ToString() : AppEnums.Status.Productive.ToString();
                if (currentStatusData == null)
                {
                    currentStatusData = StatusData.Create();
                    currentStatusData.Sdt = item.Dt;
                    currentStatusData.Value = status;
                }
                else
                {
                    if (status != currentStatusData.Value)
                    {
                        currentStatusData.Edt = item.Dt;
                        list.Add(currentStatusData);
                        currentStatusData = StatusData.Create();
                        currentStatusData.Sdt = item.Dt;
                        currentStatusData.Value = status;
                    }
                }
            }
            if (currentStatusData != null && lastDV != null)
            {
                currentStatusData.Edt = lastDV.Dt;
                if (list.Count > 0)
                {
                    var last = list[list.Count - 1];
                    if (last.Sdt != currentStatusData.Sdt)
                        list.Add(currentStatusData);
                }
            }                
            
            return MergeStatus(list, minDuration);
        }
        private StatusData[] MergeStatus(List<StatusData> list, double minDuration)
        {
            List<StatusData> result = new List<StatusData>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Duration >= minDuration)
                {
                    if (result.Count > 0 && result[result.Count - 1].Value == list[i].Value)
                    {
                        result[result.Count - 1].Edt = list[i].Edt;
                        continue;
                    }
                    result.Add(list[i]);
                    if (result.Count == 1)
                    {
                        result[0].Sdt = list[0].Sdt;
                    }
                }
                else
                {
                    if (result.Count > 0)
                        result[result.Count - 1].Edt = list[i].Edt;
                }
            }
            if (result.Count == 0 && list.Count > 0)
            {
                result.Add(list[0]);
                result[0].Edt = list[list.Count - 1].Edt;
            }

            return result.ToArray();
        }
        public double IndexConditionCalculate(IEnumerable<IDtValue> dtValues, double minDelta, double maxDelta)
        {
            double result = 0;
            double sum = 0;
            int count = 0;
            IDtValue lastValue = null;
            foreach (var dtValue in dtValues)
            {
                if (lastValue == null)
                {
                    lastValue = dtValue;
                    continue;
                }
                double delta = dtValue.Value - lastValue.Value;
                if (delta >= minDelta && delta <= maxDelta)
                {
                    sum += delta;
                    count++;
                }
                lastValue = dtValue;
            }
            if (count > 0)
                result = sum / count;
            return result;
        }
        public Dictionary<string, double> IndexTableCalulate(IEnumerable<IDtValue> dtValues, IEnumerable<IStatusData> statuss, double minDelta, double maxDelta)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            double productiveDuration = 0;
            int productiveCount = 0;
            int standbyCount = 0;
            int iminStatndbyCount = 0;
            int min10StatndbyCount = 0;
            int longstStandbyCount = 0;
            double sumDuration = 0;
            double productiveConsumption = 0;
            double sumConsumption = 0;
            double sumDelta = 0;
            int deltaCount = 0;
            IDtValue lastValue = null;
            var currentStatus = statuss.GetEnumerator();
            if (currentStatus != null && currentStatus.MoveNext())
            {
                StatusIndexCalculate(currentStatus.Current, ref productiveDuration, ref productiveCount,
                    ref sumDuration, ref standbyCount, ref iminStatndbyCount, ref min10StatndbyCount, ref longstStandbyCount);
                foreach (var dtValue in dtValues)
                {
                    if (dtValue.Dt >= currentStatus.Current.Edt)
                    {
                        if (!currentStatus.MoveNext())
                            break;
                        StatusIndexCalculate(currentStatus.Current, ref productiveDuration, ref productiveCount,
                    ref sumDuration, ref standbyCount, ref iminStatndbyCount, ref min10StatndbyCount, ref longstStandbyCount);
                    }
                    if (lastValue != null)
                    {
                        double delta = dtValue.Value - lastValue.Value;
                        if (delta >= minDelta && delta <= maxDelta)
                        {
                            sumDelta += delta;
                            deltaCount++;
                        }
                        var lastConsumption = lastValue.Value * (dtValue.Dt - lastValue.Dt).TotalHours;
                        if (currentStatus.Current.Value == AppEnums.Status.Productive.ToString())
                        {
                            productiveConsumption += lastConsumption;
                        }
                        sumConsumption += lastConsumption;
                    }
                    lastValue = dtValue;
                }
            }
            double mtbi = productiveCount > 0 ? productiveDuration / productiveCount : 0;
            double indexCondition = deltaCount > 0 ? sumDelta / deltaCount : 0;
            double productivity = sumDuration > 0 ? productiveDuration / sumDuration : 0;
            double addValue = sumConsumption > 0 ? productiveConsumption / sumConsumption : 0;
            result["productivity"] = productivity * 100;
            result["mtbi"] = mtbi * 1000;
            result["standbycount"] = standbyCount;
            result["min1statndbycount"] = iminStatndbyCount;
            result["min10statndbycount"] = min10StatndbyCount;
            result["min10plusstatndbycount"] = longstStandbyCount;
            result["standbyduration"] = (sumDuration - productiveDuration) * 1000;
            result["addvalue"] = addValue * 100;
            result["consumption"] = sumConsumption;
            result["standbyconsumption"] = sumConsumption - productiveConsumption;
            result["indexcondition"] = indexCondition;
            return result;
        }
        private void StatusIndexCalculate(IStatusData statusData, ref double productiveDuration, ref int productiveCount,
            ref double sumDuration, ref int standbyCount, ref int iminStatndbyCount, ref int min10StatndbyCount, ref int longstStandbyCount)
        {
            double duration = (statusData.Edt - statusData.Sdt).TotalSeconds;
            if (statusData.Value == AppEnums.Status.Productive.ToString())
            {
                productiveCount++;
                productiveDuration += duration;
            }
            else
            {
                standbyCount++;
                if (duration <= 60)
                    iminStatndbyCount++;
                else if (duration > 600)
                    longstStandbyCount++;
                else
                    min10StatndbyCount++;
            }
            sumDuration += duration;
        }
    }
}
