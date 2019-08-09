using Microsoft.AspNetCore.Mvc;
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
            // Get Azure auth file for login.
            var client = this.blobClientProvider.Create("personal0");
            string authFilePath = Path.Combine(IOUtils.GetDataFolderPath(), ".azureauth");
            await client.DownloadAllTextAsync("secret/azure.azureauth", authFilePath);

            // Create IAzure and check the container.
            var azure = Azure.Authenticate(authFilePath).WithDefaultSubscription();
            var container = azure.ContainerGroups.ListByResourceGroup("gp-japaneast-ctnr").FirstOrDefault();
            if (container == null)
            {
                return this.BadRequest("No container found.");
            }

            return container.IPAddress;
        }

        // Set ip
        [HttpPost]
        public ActionResult Post([FromQuery] string ip)
        {
            System.IO.File.WriteAllText(this.GetIpFilePath(), ip);
            return this.Ok();
        }

        private string GetIpFilePath()
        {
            return Path.Combine(IOUtils.GetDataFolderPath(), "ip.txt");
        }
    }
}
