using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFra.Test.ConsoleApp;
using Microsoft.AspNetCore.Mvc;

namespace CoreFra.Test.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly CachingTestClass _cachingTestClass;

        public ValuesController(CachingTestClass cachingTestClass)
        {
            _cachingTestClass = cachingTestClass;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var sss = 10;
            var t = _cachingTestClass.TestString("hiiiiiiiiiii");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
