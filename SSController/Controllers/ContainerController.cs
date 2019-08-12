using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.Fluent;
using SSController.Services;
using SSController.Utils;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SSController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        private readonly AzureService azureService;

        public ContainerController(AzureService azureService)
        {
            this.azureService = azureService;
        }

        // Get container ip
        [HttpGet]
        public async Task<ActionResult<string>> GetAsync()
        {
            // Create IAzure and check the container.
            var container = await this.azureService.GetContainerAsync();
            if (container == null)
            {
                return this.BadRequest("No container found.");
            }

            return container.IPAddress;
        }

        // Control container
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromQuery] string command)
        {
            IContainerGroup container = null;
            if (new string[] { "restart", "stop" }.Contains(command))
            {
                container = await this.azureService.GetContainerAsync();
                if (container == null)
                {
                    return this.BadRequest("No container found.");
                }
            }

            switch (command)
            {
                case "restart":
                    await container.RestartAsync();
                    return "Successfully Started.";
                case "stop":
                    await container.StopAsync();
                    return "Successfully Stopped.";
                case "create":
                    var ct = await this.azureService.CreateContainerAsync();
                    return ct.IPAddress;
                case "remove":
                    await this.azureService.RemoveContainerAsync();
                    return "Successfully Removed.";
                default:
                    return "No Command.";
            }
        }
    }
}
