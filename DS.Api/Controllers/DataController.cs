using DS.Api.Base;
using DS.Api.Extensions;
using DS.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerAbstract
    {
        private readonly ILogger<DataController> _logger;
        private readonly FileService _fileService;
        public DataController(ILogger<DataController> logger, FileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
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
                var datas = _fileService.GetValues(path, filename);

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
                return stringBuilder.ToString();
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
