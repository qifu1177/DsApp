using Ds.Infrastructure.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ds.Infrastructure.Interfaces.Services
{
    public interface IDbService
    {
        void SaveData(IIdEntity data);
    }
}
