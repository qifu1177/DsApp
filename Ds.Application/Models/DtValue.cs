using Ds.Infrastructure.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Application.Models
{
    public class DtValue : IDtValue
    {
        private static DtValue _temp;
        static DtValue()
        {
            _temp = new DtValue
            {
                Value = 0,
                Dt=DateTimeOffset.Now
            };
        }
        public DateTimeOffset Dt { get; set ; }
        public double Value { get; set; }
        public static DtValue Create(DateTimeOffset dt,double value)
        {
            DtValue dv = (DtValue)_temp.MemberwiseClone();
            dv.Dt = dt; 
            dv.Value = value;
            return dv;
        }
       
    }
}
