using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Infrastructure.Interfaces.Models
{
    public interface IStatusData
    {
        DateTimeOffset Sdt { get; set; }
        DateTimeOffset Edt { get; set; }
        string Value { get; set; }
        double Duration { get; }
    }
}
