namespace BrokenApi.Controllers
{
    using System.Threading.Tasks;
    using BrokenApi.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ILogger<ValuesController> logger;

        public ValuesController(ILogger<ValuesController> logger) => this.logger = logger;

        [HttpGet("/status-failing")]
        public async Task<IActionResult> GetFailing()
        {
            this.logger.LogDebug("X-Correlation-ID: {0}", this.Request.Headers["X-Correlation-ID"]);
            this.logger.LogDebug("User-Agent: {0}", this.Request.Headers["User-Agent"]);
            await Task.Delay(1000);
            return this.StatusCode(500, new TakeoffStatus() { Status = "Takeoff Failed" });
        }

        [HttpGet("/status-working")]
        public async Task<IActionResult> GetWorking()
        {
            this.logger.LogDebug("X-Correlation-ID: {0}", this.Request.Headers["X-Correlation-ID"]);
            this.logger.LogDebug("User-Agent: {0}", this.Request.Headers["User-Agent"]);
            await Task.Delay(1000);
            return this.Ok(new TakeoffStatus() { Status = "Takeoff Successful" });
        }
    }
}
