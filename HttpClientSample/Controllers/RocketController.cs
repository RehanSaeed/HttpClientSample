namespace HttpClientSample.Controllers
{
    using System.Threading.Tasks;
    using HttpClientSample.Clients;
    using HttpClientSample.Models;
    using Microsoft.AspNetCore.Mvc;

    [Route("[controller]")]
    [ApiController]
    public class RocketController : ControllerBase
    {
        private readonly IRocketClient rocketClient;

        public RocketController(IRocketClient rocketClient) => this.rocketClient = rocketClient;

        [HttpGet("takeoff")]
        public Task<TakeoffStatus> Takeoff(bool working = false) => this.rocketClient.GetStatus(working);
    }
}
