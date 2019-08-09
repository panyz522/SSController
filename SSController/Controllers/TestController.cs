using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SSController.Services;

namespace SSController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly SecretProvider secretProvider;

        public TestController(SecretProvider secretProvider)
        {
            this.secretProvider = secretProvider;
        }

        // GET: api/Test
        [HttpGet]
        public string Get()
        {
            return secretProvider.GetSecret("test");
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
