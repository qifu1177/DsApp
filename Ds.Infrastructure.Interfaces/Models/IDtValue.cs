using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Infrastructure.Interfaces.Models
{
    public interface IDtValue
    {
        DateTimeOffset Dt { get; set; }
        double Value { get; set; }
    }
}
