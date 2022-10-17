using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace DS.Api.Services
{
    public class FileService
    {
        public string GetFilePath()
        {
            var excutFile = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string filePaht = (new FileInfo(excutFile)).DirectoryName;
            var path = Path.Combine(filePaht, "files");

            if (Directory.Exists(path))
                return path;
            var info = new DirectoryInfo(path);
            info.Create();
            return path;
        }
        public string CopyFile(string path, IFormFile file)
        {
            string filePath = Path.Combine(path, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return filePath;
        }
        public string[] GetFiles(string path, string type)
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
        public IEnumerable<IDtValue> GetValues(string path, string fileName)
        {
            string filePath = Path.Combine(path, fileName);
            var info = new FileInfo(filePath);
            if (info.Exists)
            {
                List<string> headers = new List<string>();
                var deCultureInfo = CultureInfo.GetCultureInfo("de-DE");
                using (var stream = new StreamReader(info.FullName))
                {
                    for (var str = stream.ReadLine(); ; str = stream.ReadLine())
                    {
                        if (string.IsNullOrEmpty(str))
                            break;
                        string[] strs = str.Split(';');
                        if (strs.Length != 6)
                            continue;
                        if (headers.Count == 0)
                            headers.AddRange(strs);
                        else
                        {
                            yield return new DtValue
                            {
                                Dt = new DateTimeOffset(Convert.ToDateTime(string.Format("{0} {1}", strs[0], strs[1]), deCultureInfo.DateTimeFormat)),
                                Value = Convert.ToDouble(strs[4], deCultureInfo.NumberFormat)
                            };
                        }

                    }
                }
            }
        }
    }
}
