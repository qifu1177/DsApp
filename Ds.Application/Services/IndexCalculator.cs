using Ds.Application.Models;
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
                        results.Add(new DtValue
                        {
                            Dt = values[0].Dt,
                            Value = IndexActivityCalculate(values, minValue, maxValue),
                        });
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
    }
}
