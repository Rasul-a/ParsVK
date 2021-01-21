using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParsVK.Services
{
    public class VkApiService : IVkApiService
    {
        private string accessToken = "8bb8d8d28eadd7bff2aa8459ce40d5a1ad8ef0531aa0af32254a0d21440b64fb08fb3b33db1e19526a985";
        // private HttpContext _context;
        private HttpClient _client = new HttpClient();
        private IConfiguration _configuration;
        private IHttpClientFactory _httpClientFactory;
        public string AccessToken => accessToken;
        public VkApiService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
           // _context = context;
        }

        public async Task GetTokenAsync(string code)
        {         
            var clientId = _configuration.GetValue<string>("Client_Id");
            var clientSecret = _configuration.GetValue<string>($"Client_Secret");
            
            var url = $"https://oauth.vk.com/access_token?client_id={clientId}&client_secret={clientSecret}&redirect_uri=http://localhost:11671/GetToken&code={code}";
            var response = await _client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            dynamic token = JsonConvert.DeserializeObject(body);
            accessToken = token.access_token;
        }

        public async Task<string> GetUsersAsync(string Id)
        {
            var client = _httpClientFactory.CreateClient();
            //var url = $"https://api.vk.com/method/users.get?user_ids={Id}&v=5.89&access_token={accessToken}&fields=bdate,city";
            var url = $"https://api.vk.com/method/users.get?user_ids={Id}&v=5.89&access_token={accessToken}&fields=bdate,counters,photo_100,city";
            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            return body;
        }
    }
}
