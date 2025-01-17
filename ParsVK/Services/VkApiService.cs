﻿using Microsoft.AspNetCore.Http;
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
        //669a3d01d120680cde70b5885f1f19a0b47923eb5fd4886fbac2f9e873e6c36232fb66d486a8fd5057d30
        private string accessToken = "";
        private IConfiguration _configuration;
        private IHttpClientFactory _httpClientFactory;
        public string AccessToken => accessToken;
        public VkApiService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
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

        public async Task<string> GetWallAsync(string ownerId, int count)
        {
            return await GetAsync($"https://api.vk.com/method/wall.get?v=5.126&access_token={accessToken}&count={count}&owner_id={ownerId}");
        }

        public async Task<string> GetLikesAsync(string ownerId, string itemId, string type, int count)
        {
            return await GetAsync($"https://api.vk.com/method/likes.getList?v=5.126&access_token={accessToken}&count={count}&owner_id={ownerId}&item_id={itemId}&type={type}");
        }

        public async Task<string> ResolveScreenNameAsync(string name)
        {
            return await GetAsync($"https://api.vk.com/method/utils.resolveScreenName?v=5.126&access_token={accessToken}&screen_name={name}");
        }

        public async Task<string> GetLikeUsersAsync(string ownerId, string itemIds, string type, int count)
        {
            string code = $"var items=[{itemIds}]; var i =0; var res=[]; while (i < items.length){{res = res %2B API.users.get({{'fields':'photo_100', 'user_ids': API.likes.getList({{ 'type':'{type}','owner_id':'{ownerId}','item_id':items[i],'count':'{count}'}}).items}});i = i %2B 1;}}; return res; ";
            //string code = $"return API.users.get({{'fields':'photo_100','user_ids': API.likes.getList({{'type':'{type}','owner_id':'{ownerId}','item_id':'{itemId}','count':'1000'}}).items}});";
            return await GetAsync($"https://api.vk.com/method/execute?v=5.126&access_token={accessToken}&code={code}");
        }

        public async Task<string> GetSubscriptions(string id)
        {
            return await GetAsync($"https://api.vk.com/method/users.getSubscriptions?v=5.126&access_token={accessToken}&user_id={id}");
        }

        public async Task<string> GetNewsfeed(string sourceIds)
        {
            return await GetAsync($"https://api.vk.com/method/newsfeed.get?v=5.126&access_token={accessToken}&source_ids={sourceIds}");
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
            }
            return json;
        }
    }
}
