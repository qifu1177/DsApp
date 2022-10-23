using Autofac.Features.Metadata;
using Ds.Application.Models;
using Ds.Infrastructure.Interfaces.Models;
using Ds.Infrastructure.Interfaces.Services;
using DS.Api.Base;
using DS.Api.Extensions;
using DS.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DS.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerAbstract
    {
        private const string RawValue = "raw";
        private const string Index_Activity = "activity";

        private readonly ILogger<DataController> _logger;
        private readonly FileService _fileService;
        private readonly IIndexCalculator _indexCalculator;

        private List<IDtValue> indexList;
        public DataController(ILogger<DataController> logger, FileService fileService, IIndexCalculator indexCalculator)
        {
            _logger = logger;
            _fileService = fileService;
            _indexCalculator = indexCalculator;
        }
        protected override Action<Exception> ExceptionHandler => (ex) =>
        {
            _logger.LogError(ex.Message);
        };
        [HttpPost("upload/{filename}")]
        public IActionResult Upload(string filename)
        {
            return RequestHandler(() =>
            {
                if (Request.Form.Files.Count == 0)
                    throw new Exception("NO files");
                var file = Request.Form.Files[0];
                var path = _fileService.GetFilePath();
                string filePath = _fileService.CopyFile(path, file);
            });
        }
        [HttpGet("csvs")]
        public IActionResult GetCsvs()
        {
            return RequestHandler<string[]>(() =>
            {
                var path = _fileService.GetFilePath();
                return _fileService.GetFiles(path, ".csv");
            });
        }
        [HttpGet("data/{filename}/{mindt}/{maxdt}")]
        public IActionResult GetFileData(string filename, long mindt, long maxdt)
        {
            return RequestHandler<string>(() =>
            {
                var cultureInfo = CultureInfo.GetCultureInfo("en-US");
                var path = _fileService.GetFilePath();
                if (!MemoryCaching.TryGetValues(filename, RawValue, out IDtValue[] datas))
                {
                    datas = _fileService.GetValues(path, filename).ToArray();
                    MemoryCaching.SetValues(filename, RawValue, datas);
                }
                StringBuilder stringBuilder = new StringBuilder();
                List<string> list = new List<string>();
                DateTimeOffset minDtOffset = mindt.ToDateTimeOffset();
                DateTimeOffset maxDtOffset = maxdt.ToDateTimeOffset();
                foreach (var item in datas)
                {
                    if (item.Dt >= minDtOffset && item.Dt <= maxDtOffset)
                        list.Add(string.Format("{0}:{1}", item.Dt.ToJsonStr(), item.Value.ToString("0.###", cultureInfo.NumberFormat)));
                }
                int intervall = 1;
                if (list.Count > 2048)
                {
                    intervall = list.Count / 2048;
                }
                for (var i = 0; i < list.Count; i += intervall)
                {
                    if (i > 0)
                        stringBuilder.Append(string.Format(";{0}", list[i]));
                    else
                        stringBuilder.Append(list[i]);
                }
                string result= stringBuilder.ToString();
                return result;
            });
        }
        [HttpGet("indexActivity/{filename}/{mindt}/{maxdt}/{delta}/{minv}/{maxv}")]
        public IActionResult IndexActivity(string filename, long mindt, long maxdt, double delta, double minv, double maxv)
        {
            var cultureInfo = CultureInfo.GetCultureInfo("en-US");

            string key = string.Format("{0}.{1}", filename, Index_Activity);
            string value = string.Format("{0}_{1}_{2}", delta.ToString("0.###", cultureInfo.NumberFormat),
                minv.ToString("0.###", cultureInfo.NumberFormat), maxv.ToString("0.###", cultureInfo.NumberFormat));
            bool souldCalculate = MemoryCaching.Setting.ContainsKey(key) ? value != MemoryCaching.Setting[key] : true;
            MemoryCaching.Setting[key] = value;
            return RequestHandler<string>(() =>
            {
                var path = _fileService.GetFilePath();
                IDtValue[] datas;
                if (!MemoryCaching.TryGetValues(filename, RawValue, out datas))
                {
                    datas = _fileService.GetValues(path, filename).ToArray();
                    MemoryCaching.SetValues(filename, RawValue, datas);
                }
                IDtValue[] indexValues;
                if (souldCalculate || !MemoryCaching.TryGetValues(filename, Index_Activity, out indexValues))
                {
                    indexValues = _indexCalculator.IndexActivitiesCalculate(datas, delta, minv, maxv).ToArray();
                    MemoryCaching.SetValues(filename, Index_Activity, indexValues);
                }

                StringBuilder stringBuilder = new StringBuilder();

                DateTimeOffset minDtOffset = mindt.ToDateTimeOffset();
                DateTimeOffset maxDtOffset = maxdt.ToDateTimeOffset();

                indexList = indexValues.Where(x => x.Dt >= minDtOffset && x.Dt <= maxDtOffset).ToList();
                List<IDtValue> dataList = datas.Where(x => x.Dt >= minDtOffset && x.Dt <= maxDtOffset).ToList();
                int i = 0;

                int intervall = 1;
                if (dataList.Count > 2048)
                {
                    intervall = dataList.Count / 2048;
                }
                int startPosition = 0;
                for (i = 0; i < dataList.Count; i += intervall)
                {
                    IDtValue index = GetCurretnIndex(dataList[i].Dt, delta, ref startPosition);
                    if (index == null)
                        continue;
                    string str = string.Format("{0}:{1}", dataList[i].Dt.ToJsonStr(), index.Value.ToString("0.###", cultureInfo.NumberFormat));
                    if (i > 0)
                        stringBuilder.Append(string.Format(";{0}", str));
                    else
                        stringBuilder.Append(str);
                }
                indexList.Clear();
                string result = stringBuilder.ToString();
                return result;
            });
        }
        private IDtValue GetCurretnIndex(DateTimeOffset dt, double deltaSecond, ref int startPosition)
        {
            for (; startPosition < indexList.Count; startPosition++)
            {
                var item = indexList[startPosition];
                if (dt >= item.Dt && dt <= item.Dt.AddSeconds(deltaSecond))
                {
                    return item;
                }
                else if (dt < item.Dt)
                    break;
            }
            return null;
        }
        [HttpGet("indexActivity/{filename}/{mindt}/{maxdt}/{standbylimit}/{minduration}")]
        public IActionResult Status(string filename,long mindt,long maxdt,double standbylimit,double minduration)
        {
            var path = _fileService.GetFilePath();
            return RequestHandler<IEnumerable<IStatusData>>(() =>
            {

                IDtValue[] indexValues;
                if (MemoryCaching.TryGetValues(filename, Index_Activity, out indexValues))
                {
                    return _indexCalculator.CaculateStatus(indexValues, standbylimit, minduration);
                }
                return new StatusData[0];
            });
        }
        // GET: api/<DataController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return ResultHandler<string[]>(() =>
            {
                return new string[] { "value1", "value2" };
            }, new string[0]);
        }

        // GET api/<DataController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DataController>
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/<DataController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
