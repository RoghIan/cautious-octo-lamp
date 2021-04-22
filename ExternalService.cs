using System;
using System.Net.Http;
using System.Threading.Tasks;
using CSRecruit.Service.Helpers;
using CSRecruit.Service.Interfaces;
using Newtonsoft.Json;

namespace CSRecruit.Service.Services
{
  public class ExternalService : IDisposable
    {
        protected readonly HttpClient Client;

        protected string BaseUrl => GetBaseUrl();

        protected ExternalService()
        {
            Client = new HttpClient();
        }

        protected virtual async Task<TResponse> Get<TResponse>(string endpoint, bool isSnakeCase = false)
            //where TResponse : IExternalServiceResponse
        {
            var response = await Client.GetAsync(endpoint);
            return await ParseResponse<TResponse>(response, isSnakeCase);
        }

        protected virtual async Task<TResponse> Post<TRequest, TResponse>
            (string endpoint, TRequest body, bool isSnakeCase = false)
            where TRequest : IExternalServiceRequest
            where TResponse : IExternalServiceResponse
        {
            var response = await Client.PostAsJsonAsync(endpoint, body);
            return await ParseResponse<TResponse>(response, isSnakeCase);
        }

        protected virtual async Task<TResponse> Put<TRequest, TResponse>(string endpoint, TRequest body)
            where TRequest : IExternalServiceRequest
            where TResponse : IExternalServiceResponse
        {
            var response = await Client.PutAsJsonAsync(endpoint, body);
            return await ParseResponse<TResponse>(response);
        }

        protected virtual async Task<TResponse> Delete<TResponse>(string endpoint)
            where TResponse : IExternalServiceResponse
        {
            var response = await Client.DeleteAsync(endpoint);
            return await ParseResponse<TResponse>(response);
        }

        protected virtual async Task<T> ParseResponse<T>(HttpResponseMessage response, bool snakeCase = false) //where T : IExternalServiceResponse
        {
            var responseAsString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (!snakeCase) return JsonConvert.DeserializeObject<T>(responseAsString);
                    
                var snakeCaseJson = new JsonSerializerSettings
                {
                    ContractResolver = new JsonSerializerHelper()
                };
                        
                return JsonConvert.DeserializeObject<T>(responseAsString, snakeCaseJson);
            }
            else
            {
                throw new Exception(responseAsString);
            }
        }

        protected virtual string GetBaseUrl()
        {
            return "";
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}