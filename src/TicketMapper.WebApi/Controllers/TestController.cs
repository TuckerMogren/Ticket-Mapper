using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TestController(ILogger<TestController> logger) : ControllerBase
    {
        [HttpGet("Test")]
        [SwaggerOperation(Summary = "This service will generate and allow user to download file.")]
        [SwaggerResponse(200, "Will always return Ok", typeof(OkResult))]
        public IActionResult Get()
        {
            logger.LogInformation("You have successfully tested the endpoint!");
            return Ok("Hi");
        }

    }
}
