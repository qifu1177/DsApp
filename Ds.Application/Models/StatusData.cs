using Ds.Infrastructure.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Application.Models
{
    public class StatusData: IStatusData
    {
        public DateTimeOffset Sdt { get; set; }
        public DateTimeOffset Edt { get; set; }
        public string Value { get; set; }
        public double Duration => (Edt - Sdt).TotalSeconds;
        public static StatusData Create()
        {
            return new StatusData();
        }
    }
}
