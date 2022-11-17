using Ds.Infrastructure.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS.Api.Services.Interfaces
{
    public interface IFileService
    {
        string GetFilePath();
        string CopyFile(string path, IFormFile file);
        string[] GetFiles(string path, string type);
        IEnumerable<IDtValue> GetValues(string path, string fileName);
    }
}
