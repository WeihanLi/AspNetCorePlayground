using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestWebApplication.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ILogger _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var msg = $"IsAuthenticated: {User.Identity.IsAuthenticated} ,UserName: {User.Identity.Name}";
            _logger.LogInformation(msg);
            return new string[] { msg };
        }

        // GET api/values/5
        [Authorize]
        [HttpGet("{id:int}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        [Authorize]
        public void Post([FromBody] TestModel model)
        {
            _logger.LogInformation($"UserName: {User.Identity.Name}");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] TestModel model)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class TestModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
