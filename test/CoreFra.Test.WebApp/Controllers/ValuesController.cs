using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreFra.Domain;
using CoreFra.Test.Common;
using CoreFra.Test.ConsoleApp;
using Microsoft.AspNetCore.Mvc;

namespace CoreFra.Test.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ICachingTest _cachingTest;
        private readonly ITestService _testService;

        public ValuesController(ICachingTest cachingTest, ITestService testService)
        {
            _cachingTest = cachingTest;
            _testService = testService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var sss = 10;

            //var toBeInsertedTestEntities = PopulateTestEntities();
            //foreach (var beInsertedTestEntity in toBeInsertedTestEntities)
            //{
            //    _testService.Insert(beInsertedTestEntity);
            //}

            var all = _testService.GetAll();

            var saeidUsers = _testService.GetByFirstName("Saeid");

            var moreThanAgeUsers = _testService.GetMoreThanGivenAge(29);

            var givenNamesUsers = _testService.GetAllUsersByGivenFirstNames(new List<string>
            {
                "Behzad",
                "Behnam",
                "Omid"
            });



            //var t = _cachingTest.TestString("byyyyeeeee", new List<int>{1, 2, 3, 4, 5}, null, new PagedCollection<string>
            //{
            //    List = new List<string> { "a", "b"},
            //    PageNumber = 1,
            //    PageSize = 1,
            //    TotalCount = 2
            //});

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

        private List<TestEntity> PopulateTestEntities()
        {
            return new List<TestEntity>
            {
                new TestEntity
                {
                    FirstName = "Behzad",
                    Lastname = "Rezaie",
                    Age = 28,
                    BirthDate = new DateTime(1990, 10, 27)
                },
                new TestEntity
                {
                    FirstName = "Mohammad",
                    Lastname = "Chorakchi",
                    Age = 28,
                    BirthDate = new DateTime(1990, 6, 3)
                },
                new TestEntity
                {
                    FirstName = "Behnam",
                    Lastname = "Zeighami",
                    Age = 30,
                    BirthDate = new DateTime(1989, 2, 24)
                },
                new TestEntity
                {
                    FirstName = "Hasan",
                    Lastname = "Gholiloo",
                    Age = 27,
                    BirthDate = new DateTime(1991, 11, 9)
                },
                new TestEntity
                {
                    FirstName = "Mehran",
                    Lastname = "Golzari",
                    Age = 30,
                    BirthDate = new DateTime(1989, 5, 12)
                },
                new TestEntity
                {
                    FirstName = "Saeid",
                    Lastname = "Komeijani",
                    Age = 31,
                    BirthDate = new DateTime(1988, 1, 5)
                },
                new TestEntity
                {
                    FirstName = "Saeid",
                    Lastname = "Akrami",
                    Age = 30,
                    BirthDate = new DateTime(1989, 6, 27)
                },
                new TestEntity
                {
                    FirstName = "Omid",
                    Lastname = "Rafiei",
                    Age = 27,
                    BirthDate = new DateTime(1992, 3, 8)
                },
                new TestEntity
                {
                    FirstName = "Javad",
                    Lastname = "Hakimpanah",
                    Age = 28,
                    BirthDate = new DateTime(1990, 10, 19)
                },
            };
        }
    }
}
