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
        private readonly BlobClientProvider blobClientProvider;

        public ContainerController(BlobClientProvider blobClientProvider)
        {
            this.blobClientProvider = blobClientProvider;
        }

        // Get container ip
        [HttpGet]
        public async Task<ActionResult<string>> GetAsync()
        {
            // Create IAzure and check the container.
            var container = await GetContainerAsync();
            if (container == null)
            {
                return this.BadRequest("No container found.");
            }

            return container.IPAddress;
        }

        // Set ip
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromQuery] string command)
        {
            // Create IAzure and check the container.
            var container = await this.GetContainerAsync();
            if (container == null)
            {
                return this.BadRequest("No container found.");
            }

            string res;
            switch (command)
            {
                case "restart":
                    await container.RestartAsync();
                    res = "Success";
                    break;
                default:
                    res = "No command";
                    break;
            }

            return res;
        }

        private string GetIpFilePath()
        {
            return Path.Combine(IOUtils.GetDataFolderPath(), "ip.txt");
        }

        private async Task<IContainerGroup> GetContainerAsync()
        {
            // Get Azure auth file for login.
            var client = this.blobClientProvider.Create("personal0");
            string authFilePath = Path.Combine(IOUtils.GetDataFolderPath(), ".azureauth");
            await client.DownloadAllTextAsync("secret/azure.azureauth", authFilePath);

            var azure = Azure.Authenticate(authFilePath).WithDefaultSubscription();
            return azure.ContainerGroups.ListByResourceGroup("gp-japaneast-ctnr").FirstOrDefault();
        }
    }
}
