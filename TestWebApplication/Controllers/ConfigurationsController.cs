using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TestWebApplication.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationsController : Controller
    {
        private readonly IConfigurationRoot _configuration;

        public ConfigurationsController(IConfiguration configuration)
        {
            _configuration = configuration as IConfigurationRoot;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                RootUser = _configuration.GetAppSetting("RootUser"),
                ConfigurationId = _configuration.GetAppSetting<int>("ConfigurationId")
            });
        }

        [HttpPut]
        public IActionResult Put()
        {
            _configuration.Reload();
            return Ok();
        }
    }
}
