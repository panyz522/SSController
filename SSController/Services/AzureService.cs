using Microsoft.Azure.Management.ContainerInstance.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using SSController.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SSController.Services
{
    public class AzureService
    {
        private readonly BlobClientProvider blobClientProvider;

        private readonly string resourceGroupName = "gp-sscg";
        private readonly string containerImage = "panyz522/ss";
        private readonly string containerInstanceName = "ss";
        private readonly int port = 7894;
        private readonly Region region = Region.JapanEast;
        private readonly string regionStr;
        private readonly string containerGroupName;

        public AzureService(BlobClientProvider blobClientProvider)
        {
            this.blobClientProvider = blobClientProvider;

            this.regionStr = this.region.ToString().ToLower().Replace(" ", "");
            this.containerGroupName = $"sscg-{this.regionStr}";
        }

        public async Task<IContainerGroup> GetContainerAsync()
        {
            var azure = await this.FetchAuthAndGetAzure();
            return this.GetContainerGroupsInResourceGroup(azure).FirstOrDefault();
        }

        public async Task<IContainerGroup> CreateContainerAsync()
        {
            var azure = await this.FetchAuthAndGetAzure();

            // Make sure resource group exists.
            if (!await azure.ResourceGroups.ContainAsync(this.resourceGroupName))
            {
                await azure.ResourceGroups
                    .Define(this.resourceGroupName)
                    .WithRegion(Region.JapanEast)
                    .CreateAsync();
            }

            // Exit if container group exists. Assume a container group must have one ss instance.
            if (this.GetContainerGroupsInResourceGroup(azure).Count() > 0)
            {
                throw new Exception($"Container group exists in {this.resourceGroupName}");
            }

            return await azure.ContainerGroups.Define(this.containerGroupName)
                .WithRegion(this.region)
                .WithExistingResourceGroup(this.resourceGroupName)
                .WithLinux()
                .WithPublicImageRegistryOnly()
                .WithoutVolume()
                .DefineContainerInstance(this.containerInstanceName)
                    .WithImage(this.containerImage)
                    .WithExternalTcpPorts(new int[] { this.port })
                    .WithCpuCoreCount(1.0)
                    .WithMemorySizeInGB(1)
                    .Attach()
                .WithDnsPrefix(this.containerGroupName)
                .CreateAsync();
        }

        public async Task RemoveContainerAsync()
        {
            var azure = await this.FetchAuthAndGetAzure();
            var container = await azure.ContainerGroups.GetByResourceGroupAsync(this.resourceGroupName, this.containerGroupName);
            if (container != null)
            {
                azure.ContainerGroups.DeleteById(container.Id);
            }
        }

        private IEnumerable<IContainerGroup> GetContainerGroupsInResourceGroup(IAzure azure)
        {
            try
            {
                return azure.ContainerGroups.ListByResourceGroup(this.resourceGroupName);
            }
            catch
            {
                return Enumerable.Empty<IContainerGroup>();
            }
        }

        private async Task<IAzure> FetchAuthAndGetAzure()
        {
            // Get Azure auth file for login.
            var client = this.blobClientProvider.Create("personal0");
            string authFilePath = Path.Combine(IOUtils.GetDataFolderPath(), ".azureauth");
            await client.DownloadAllTextAsync("secret/azure.azureauth", authFilePath);

            return Azure.Authenticate(authFilePath).WithDefaultSubscription();
        }
    }
}
