using DS.Api.Base;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerAbstract
    {
        private readonly ILogger<DataController> _logger;

        public DataController(ILogger<DataController> logger)
        {
            _logger = logger;
        }
        protected override Action<Exception> ExceptionHandler => (ex) => { 
            _logger.LogError(ex.Message);
        };
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
            ResultHandler(() => { });
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
