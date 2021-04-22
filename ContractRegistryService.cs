using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CSRecruit.Service.ViewModels.ContractRegistry;

namespace CSRecruit.Service.Services
{
    public class ContractRegistryService : ExternalService
    {
        public ContractRegistryService()
        {
            Client.BaseAddress = new Uri(BaseUrl);
        }
        
        protected override string GetBaseUrl()
        {
            return ConfigurationManager.AppSettings["ContractRegistryUrl"];
        }
        
        private async Task<string> GetContractRegistryTokenAsync()
        {

            var client = new HttpClient {BaseAddress = new Uri(ConfigurationManager.AppSettings["ContractRegistryUrl"])};
             var uriString = $"api/Token/v1/integration?appid={ConfigurationManager.AppSettings["ContractRegistryAppId"]}" +
                             $"&key={ConfigurationManager.AppSettings["ContractRegistryAppKey"]}";

            var response = await client.GetStringAsync(uriString);

            return response;
        }

        public async Task<IReadOnlyList<CxGetStaffResponseDto>> GetStaffContractByClientCode(string clientCode)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetContractRegistryTokenAsync());
            var uriString = $"api/Contracts/GetStaffContractByClientCode?clientCode={clientCode}";

            var response = await Get<IReadOnlyList<CxGetStaffResponseDto>>(uriString);

            return response;
        }
    }
}