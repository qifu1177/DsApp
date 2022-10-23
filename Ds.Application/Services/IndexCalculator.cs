﻿using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using Ds.Infrastructure.Interfaces.Services;
using System;
using System.Collections.Generic;
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
        public IEnumerable<IStatusData> CaculateStatus(IEnumerable<IDtValue> datas,double standbyLimit, double minDuration)
        {
            List<StatusData> list = new List<StatusData>();
            IDtValue lastDV = null;
            StatusData currentStatusData = null;
            foreach (var item in datas)
            {
                lastDV=item;
                string status = item.Value <= standbyLimit ? AppEnums.Status.Standby.ToString() : AppEnums.Status.Productive.ToString();
                if (currentStatusData==null)
                {
                    currentStatusData = StatusData.Create();
                    currentStatusData.Sdt = item.Dt;
                    currentStatusData.Value = status;
                }
                else
                {
                    if(status!=currentStatusData.Value)
                    {
                        currentStatusData.Edt=item.Dt;
                        list.Add(currentStatusData);
                        currentStatusData = StatusData.Create();
                        currentStatusData.Sdt = item.Dt;
                        currentStatusData.Value = status;
                    }
                }
            }
            if (currentStatusData != null && lastDV != null)
                currentStatusData.Edt = lastDV.Dt;
            return MergeStatus(list,minDuration);
        }
        private StatusData[] MergeStatus(List<StatusData> list, double minDuration)
        {
            List<StatusData> result = new List<StatusData>();
            for(int i=0;i<list.Count;i++)
            {
                if(list[i].Duration>=minDuration)
                {
                    if(result.Count>0 && result[result.Count - 1].Value == list[i].Value)
                    {
                        result[result.Count - 1].Edt = list[i].Edt;
                        continue;
                    }
                    result.Add(list[i]);
                    if(result.Count==1)
                    {
                        result[0].Sdt = list[0].Sdt;
                    }
                }else
                {
                    if(result.Count>0)
                        result[result.Count-1].Edt = list[i].Edt;
                }
            }
            if(result.Count==0 && list.Count>0)
            {
                result.Add(list[0]);
                result[0].Edt = list[list.Count - 1].Edt;
            }
                
            return result.ToArray();
        }
    }
}
