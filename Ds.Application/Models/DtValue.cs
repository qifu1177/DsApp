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
        public DateTimeOffset Dt { get; set ; }
        public double Value { get; set; }
        public static DtValue Create(DateTimeOffset dt,double value)
        {            
            var dv = ObjectFactory<DtValue>.Create();
            dv.Dt = dt; 
            dv.Value = value;
            return dv;
        }
        public void d()
        {
            object o=new object();
            var dv=this.MemberwiseClone();
        }
    }
}
