using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController(ILogger<TestController> logger) : ControllerBase
    {
        [HttpGet("Receive")]
        [SwaggerOperation(Summary = "This service will allow us to test to make sure we can connect to apis")]
        [SwaggerResponse(200, "Will always return Ok", typeof(OkResult))]
        public IActionResult Get()
        {
            logger.LogInformation("You have successfully tested the endpoint!");
            return Ok("Hi");
        }

    }
}
