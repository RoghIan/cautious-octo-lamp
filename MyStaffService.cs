using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CSRecruit.Service.Models;
using CSRecruit.Service.ViewModels;
using CSRecruit.Service.ViewModels.MyStaff;
using Newtonsoft.Json;

namespace CSRecruit.Service.Services
{
    public class MyStaffService : ExternalService
    {
        public MyStaffService()
        {
            Client.BaseAddress = new Uri(BaseUrl);
        }

        protected override string GetBaseUrl()
        {
            return ConfigurationManager.AppSettings["MyStaffClientURL"];
        }

        private static async Task<string> GetMyStaffTokenAsync()
        {
            var client = new HttpClient {BaseAddress = new Uri(ConfigurationManager.AppSettings["MyStaffAPITokenURL"])};

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var vm = new Token()
            {
                app_id = ConfigurationManager.AppSettings["AppId"],
                app_key = ConfigurationManager.AppSettings["AppKey"]
            };
            var response = await client.PostAsJsonAsync("/api/token.json", vm);
            var jsonString = await response.Content.ReadAsStringAsync();

            return jsonString;
        }

        private static async Task<string> GetToken()
        {
            var accessToken = await GetMyStaffTokenAsync();
            if(string.IsNullOrEmpty(accessToken)) return string.Empty;
            
            var objToken = JsonConvert.DeserializeObject<TokenVm>(accessToken);
            return objToken.token;
        }
        
        public async Task<MyStaffPostPocResponseDto> PostPoc(MyStaffPostPocRequestDto requestBody)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetToken());

            var response = await Post<MyStaffPostPocRequestDto, MyStaffPostPocResponseDto>("/api/point_of_contact/save.json", requestBody);

            return response;
        }

        public async Task<MyStaffGetPocResponseRootDto> GetPoc(MyStaffGetPocRequestDto requestBody)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetToken());

            const bool isSnakeCaseResponse = true;
            var response = await Post<MyStaffGetPocRequestDto, MyStaffGetPocResponseRootDto>
                ("/api/point_of_contact/lists.json", requestBody, isSnakeCaseResponse);

            return response;
        }
        
    }
}