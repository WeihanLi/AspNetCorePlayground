using System;
using Microsoft.AspNetCore.Mvc;

namespace TestGateway.Controllers
{
    [Route("/api/[controller]")]
    public class TestController : ControllerBase
    {
        public IActionResult Get()
        {
            return Ok(new
            {
                Tick = DateTime.UtcNow.Ticks,
                Msg = "Hello Ocelot",
            });
        }
    }
}
