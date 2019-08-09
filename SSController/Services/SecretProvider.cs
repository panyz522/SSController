using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SSController.Utils;
using System.IO;
using System.Text;

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
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
#else
            var text = File.ReadAllText(Path.Combine(IOUtils.GetDataFolderPath(), "testsecrets.json"));
            keyValuePairs = JObject.Parse(text);
#endif
        }

        public string GetSecret(string key)
        {
            string sec;
            try
            {
#if !DEBUG
                sec = keyVaultClient.GetSecretAsync("https://personalkeys0.vault.azure.net/secrets/store-personal0").Result.Value;
#else
                sec = keyValuePairs[key].ToString();
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
