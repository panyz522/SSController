using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SSController.Services
{
    public class BlobClientProvider
    {
        private readonly SecretProvider secretProvider;

        public BlobClientProvider(SecretProvider secretProvider)
        {
            this.secretProvider = secretProvider;
        }

        public BlobClient Create(string storageName)
        {
            return new BlobClient(this.secretProvider.GetSecret($"store-{storageName}"));
        }
    }

    public class BlobClient
    {
        private readonly CloudBlobClient cloudBlobClient;

        public BlobClient(string connectionString)
        {
            CloudStorageAccount storageAccount;
            if (!CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                throw new Exception("Cannot parse connection string.");
            }

            this.cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

        public async Task DownloadAllTextAsync(string remotePath, string localPath)
        {
            string[] pathElements = remotePath.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (pathElements.Length < 2)
            {
                return;
            }

            var cloudBlobContainer = this.cloudBlobClient.GetContainerReference(pathElements[0]);
            var block = cloudBlobContainer.GetBlockBlobReference(string.Join("/", pathElements.Skip(1)));
            await block.DownloadToFileAsync(localPath, FileMode.Create);
        }
    }
}
