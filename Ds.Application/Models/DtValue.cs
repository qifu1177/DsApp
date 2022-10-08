using Ds.Infrastructure.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Application.Models
{
    public class DtValue : IDtValue
    {
        public DateTimeOffset Dt { get; set ; }
        public double Value { get; set; }
    }
}
