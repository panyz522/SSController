using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json.Linq;
using SSController.Utils;
using System.IO;

namespace SSController.Services
{
    public class SecretProvider
    {
        private KeyVaultClient keyVaultClient;
        private JObject keyValuePairs;

        public SecretProvider()
        {
            this.Init();
        }

        public void Init()
        {
#if !DEBUG
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            this.keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
#else
            var text = File.ReadAllText(Path.Combine(IOUtils.GetDataFolderPath(), "testsecrets.json"));
            this.keyValuePairs = JObject.Parse(text);
#endif
        }

        public string GetSecret(string key)
        {
            string sec;
            try
            {
#if !DEBUG
                sec = this.keyVaultClient.GetSecretAsync($@"https://personalkeys0.vault.azure.net/secrets/{key}").Result.Value;
#else
                sec = this.keyValuePairs[key].ToString();
#endif
            }
            catch
            {
                return "";
            }

            return sec;
        }
    }
}
