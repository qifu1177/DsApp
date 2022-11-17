using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using System.Diagnostics;

namespace DS.Api.Base
{
    public delegate IEnumerable<IDtValue> CsvGetValues(string path, string fileName);
    public class CsvInterpretationFactory
    {

        public static CsvInterpretationFactory Instance { get; private set; }
        static CsvInterpretationFactory()
        {
            Instance = new CsvInterpretationFactory();
        }
        public CsvGetValues Create(string path, string fileName)
        {
            string filePath = Path.Combine(path, fileName);
            var info = new FileInfo(filePath);
            if (info.Exists)
            {
                using (var stream = new StreamReader(info.FullName))
                {
                    for (var str = stream.ReadLine(); ; str = stream.ReadLine())
                    {
                        if (string.IsNullOrEmpty(str))
                            break;
                        string[] strs = str.Split(';');
                        if (strs.Length == 6)
                            return Template1_GetValues;
                        else if (strs.Length > 6)
                            return Template2_GetValues;
                    }
                }
            }
            return null;
        }
        public IEnumerable<IDtValue> Template1_GetValues(string path, string fileName)
        {
            var info = new FileInfo(Path.Combine(path, fileName));
            List<string> headers = new List<string>();
            using (var stream = new StreamReader(info.FullName))
            {
                Debug.WriteLine($"open file {info.Name}");
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
                        string[] dtstrs = strs[0].Split('.');
                        string dtstr = string.Format("{0}-{1}-{2}", dtstrs[2], dtstrs[1], dtstrs[0]);
                        var dtvalue = DtValue.Create(new DateTimeOffset(Convert.ToDateTime(string.Format("{0} {1}", dtstr, strs[1]))),
                            Convert.ToDouble(strs[4].Replace(',', '.')));

                        yield return dtvalue;
                    }
                }
            }
        }
        public IEnumerable<IDtValue> Template2_GetValues(string path, string fileName)
        {
            var info = new FileInfo(Path.Combine(path, fileName));
            List<string> headers = new List<string>();
            using (var stream = new StreamReader(info.FullName))
            {
                Debug.WriteLine($"open file {info.Name}");
                for (var str = stream.ReadLine(); ; str = stream.ReadLine())
                {
                    if (string.IsNullOrEmpty(str))
                        break;
                    string[] strs = str.Split(';');
                    if (strs.Length != 12)
                        continue;
                    string[] dtstrs = strs[0].Split(' ');
                    string[] dstrs=dtstrs[0].Split('.');
                    string dtstr = string.Format("{0}-{1}-{2}", dstrs[2], dstrs[1], dstrs[0]);
                    var dtvalue = DtValue.Create(new DateTimeOffset(Convert.ToDateTime(string.Format("{0} {1}", dtstr, dtstrs[1]))),
                        Convert.ToDouble(strs[5].Replace(',', '.')));
                    yield return dtvalue;
                }
            }
        }
    }
}
