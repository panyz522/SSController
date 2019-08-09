using Microsoft.AspNetCore.Mvc;
using SSController.Services;
using System.Threading.Tasks;

namespace SSController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly SecretProvider secretProvider;
        private readonly BlobClientProvider blobClientProvider;

        public TestController(SecretProvider secretProvider, BlobClientProvider blobClientProvider)
        {
            this.secretProvider = secretProvider;
            this.blobClientProvider = blobClientProvider;
        }

        // GET: api/Test
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return this.Ok();
        }

        // GET: api/Test/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Test
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
