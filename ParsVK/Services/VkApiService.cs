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
        private string accessToken = "9a7a5108ef735365cf388f61aa0dbb8933c4394f53b1b85670df8f87f621e3b9974235c693ba5276df98a";
        // private HttpContext _context;
        //private HttpClient _client = new HttpClient();
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

            var body = await GetAsync($"https://oauth.vk.com/access_token?client_id={clientId}&client_secret={clientSecret}&redirect_uri=http://localhost:5000/gettoken&code={code}");
            dynamic token = JsonConvert.DeserializeObject(body);
            accessToken = token.access_token;
        }

        public async Task<string> GetProfileAsync(string Id, string type)
        {
            if (type == "user")
                return await GetAsync($"https://api.vk.com/method/users.get?user_ids={Id}&v=5.126&access_token={accessToken}&fields=bdate,counters,photo_100,city");
            else
                return await GetAsync($"https://api.vk.com/method/groups.getById?&v=5.126&access_token={accessToken}&group_id={Id}&fields=members_count");
        }

        public async Task<string> GetWallAsync(string ownerId)
        {
            return await GetAsync($"https://api.vk.com/method/wall.get?v=5.126&access_token={accessToken}&count=50&owner_id={ownerId}");
        }

        public async Task<string> GetLikesAsync(string ownerId, string itemId, string type)
        {
            return await GetAsync($"https://api.vk.com/method/likes.getList?v=5.126&access_token={accessToken}&count=100&owner_id={ownerId}&item_id={itemId}&type={type}");
        }

        public async Task<string> ResolveScreenNameAsync(string name)
        {
            return await GetAsync($"https://api.vk.com/method/utils.resolveScreenName?v=5.126&access_token={accessToken}&screen_name={name}");
        }

        public async Task<string> GetLikeUsersAsync(string ownerId, string itemId, string type)
        {
            string code = $"return API.users.get({{'fields':'photo_100','user_ids': API.likes.getList({{'type':'{type}','owner_id':'{ownerId}','item_id':'{itemId}'}}).items}});";
            return await GetAsync($"https://api.vk.com/method/execute?v=5.126&access_token={accessToken}&code={code}");
        }

        private async Task<string> GetAsync(string url)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            dynamic res = JsonConvert.DeserializeObject(json);
            if (res.error != null)
            {
                string s = res.error.error_code + " " + res.error.error_msg;
                throw new Exception(s);
               // return StatusCode(StatusCodes.Status500InternalServerError, res.error.error_code + " " + res.error.error_message);
            }
            return json;
        }
    }
}
