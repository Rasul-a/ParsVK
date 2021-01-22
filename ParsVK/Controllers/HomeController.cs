using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ParsVK.Models;
using ParsVK.Models.JsonModel;
using ParsVK.Repositories;
using ParsVK.Services;

namespace ParsVK.Controllers
{
    [Route("api")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private IVkApiService _vkApiService;
        private IProfileRepository _profiles;
        public HomeController(IVkApiService vkApiService, IProfileRepository profiles)
        {
            _profiles = profiles;
            _vkApiService = vkApiService;
        }
        [Route("gettoken")]
        public async Task<IActionResult> GetTokenAsync(string code)
        {
            await _vkApiService.GetTokenAsync(code);
            return Ok();
        }

        [Route("parseprofile")]
        public async Task<IActionResult> ParseProfile(string link)
        {
            if (_vkApiService.AccessToken == "")
                return StatusCode(StatusCodes.Status500InternalServerError, "no access_token");
            string id = link.Split('/').Last();
            var json = await _vkApiService.GetUsersAsync(id);
            dynamic res = JsonConvert.DeserializeObject(json);
            Profile profile = new Profile
            {
                Id = res.response[0].id,
                FirstName = res.response[0].first_name,
                LastName = res.response[0].last_name,
                City = res.response[0].city.title,
                Bdate = res.response[0].bdate,
                PhotoUrl = res.response[0].photo_100,
                Audios = res.response[0].counters.audios,
                Friends = res.response[0].counters.friends,
                Groups = res.response[0].counters.groups,
                Photos = res.response[0].counters.photos
            };
            if (profile != null)
            {
                var p = await _profiles.GetByIdAsync(profile.Id);
                if (p == null)
                    await _profiles.CreateAsync(profile);
 
                    
                //return Ok(profile);
            }
            var wallJson = await _vkApiService.GetWallAsync(profile.Id);
            var wallGet = JsonConvert.DeserializeObject<WallGet>(wallJson);
            List<WallItem> wallItems = new List<WallItem>();
            int index=1;
            foreach (var item in wallGet.response.items)
            {
                //wallItems.Add(new WallItem
                //{
                //    Id = item.id,
                //    Text = item.text,
                //    LikesCount = item.likes.count,
                //    CommentsCount = item.comments.count,
                //    HistoryText = item.copy_history.text,
                //    Type = item.copy_history.attachments[0].type,
                //    // Url = item.copy_history.attachments[0].toString()
                //});
                var a = new WallItem();
               // Attachment attach
                a.Id = item.id;
                a.Text = item.text;
                a.LikesCount = item.likes.count;
                a.CommentsCount = item.comments.count;

                if (item.copy_history != null)
                {
                    a.HistoryText = item.copy_history[0].text;
                    a.Type = item.copy_history[0].attachments?[0].type;
                    a.Url = GetUrlFromAttachment(item.copy_history[0].attachments?[0], a.Type);

                }
                else 
                    if (item.attachments != null)
                {
                    a.Type = item.attachments[0].type;
                    a.Url = GetUrlFromAttachment(item.attachments?[0], a.Type);
                }


                wallItems.Add(a);
                Debug.WriteLine(index+++": "+item.id);
            }


            List<WallItem> LikesItems = wallItems.OrderByDescending(a => a.LikesCount).Take(10).ToList();
            List<WallItem> CommentsItems = wallItems.OrderByDescending(a => a.CommentsCount).Take(10).ToList();
            //String.Join(',',LikesItems.Select(a=>a.Id+"="+a.LikesCount))+"---"+ String.Join(',', CommentsItems.Select(a => a.Id+"="+a.CommentsCount)) + "---" + String.Join(',', LikesItems.Union(CommentsItems).Select(a => a.Id))
            return Ok(LikesItems.Union(CommentsItems));
            //return StatusCode(StatusCodes.Status400BadRequest);
        }

        private string GetUrlFromAttachment(Attachment attachment, string type)
        {
            switch (type)
            {
                case "video":
                    return attachment.video.image[0].url;
                case "doc":
                    return attachment.doc.preview.photo.sizes[0].src;
                case "photo":
                    return attachment.photo.sizes[0].url;
                default:
                    return "";
            }
        }
    }
}