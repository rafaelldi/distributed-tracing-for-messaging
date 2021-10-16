using System.Threading.Tasks;
using contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace api
{
    [ApiController]
    [Route("[controller]")]
    public class GreetingController : ControllerBase
    {
        private readonly IRequestClient<GreetingRequest> _client;

        public GreetingController(IRequestClient<GreetingRequest> client)
        {
            _client = client;
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetGreeting(string name)
        {
            var response = await _client.GetResponse<GreetingResponse>(new GreetingRequest(name));
            return Ok(response.Message.Greeting);
        }
    }
}