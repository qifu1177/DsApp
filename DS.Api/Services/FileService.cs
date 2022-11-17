using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using DS.Api.Base;
using DS.Api.Services.Interfaces;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace DS.Api.Services
{
    public class FileService : IFileService
    {
        public FileService() { }
        public virtual string GetFilePath()
        {
            var excutFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string filePath = (new FileInfo(excutFile)).DirectoryName;
            var path = Path.Combine(filePath, "files");

            if (Directory.Exists(path))
                return path;
            var info = new DirectoryInfo(path);
            info.Create();
            return path;
        }
        public virtual string CopyFile(string path, IFormFile file)
        {
            string filePath = Path.Combine(path, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return filePath;
        }
        public virtual string[] GetFiles(string path, string type)
        {
            List<string> files = new List<string>();
            var info = new DirectoryInfo(path);
            foreach (var file in info.GetFiles())
            {
                if (file.Name.EndsWith(type))
                    files.Add(file.Name);
            }
            return files.ToArray();
        }
        public virtual IEnumerable<IDtValue> GetValues(string path, string fileName)
        {
            var csvGetValues = CsvInterpretationFactory.Instance.Create(path, fileName);
            if (csvGetValues != null)
                return csvGetValues.Invoke(path, fileName);
            return new DtValue[0];            
        }
    }
}
