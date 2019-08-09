using Microsoft.AspNetCore.Mvc;
using SSController.Utils;
using System.IO;
using System.Linq;

namespace SSController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerController : ControllerBase
    {
        // Get container ip
        [HttpGet]
        public ActionResult<string> Get()
        {
            if (!System.IO.File.Exists(this.GetIpFilePath()))
            {
                return "";
            }

            var lines = System.IO.File.ReadLines(GetIpFilePath()).ToArray();
            if (lines.Length == 0)
            {
                return "";
            }

            return lines[0];
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
