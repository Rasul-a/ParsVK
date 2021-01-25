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
        private IRepository<LikeUser> _likeUsers;
        private IProfileRepository _profiles;
        private ParseVkService _parseVk;
        public HomeController(IVkApiService vkApiService, IProfileRepository profiles, IRepository<LikeUser> likeUsers, ParseVkService parseVk)
        {
            _parseVk = parseVk;
            _likeUsers = likeUsers;
            _profiles = profiles;
            _vkApiService = vkApiService;
        }
        [Route("gettoken")]
        public async Task<IActionResult> GetTokenAsync(string code)
        {
            await _vkApiService.GetTokenAsync(code);
            //return BadRequest("bad");
            return Ok();
        }

        //public IActionResult Create(string link)
        //{

        //}



        [Route("parseprofile")]
        public async Task<IActionResult> ParseProfile(string link)
        {
            if (_vkApiService.AccessToken == "")
                return StatusCode(StatusCodes.Status500InternalServerError, "5 User authorization failed: access_token has expired.");
            string id = link.Split('/').Last();
            string json;
            Profile profile;
            List<WallItem> wallItems;
            try
            {
                json = await _vkApiService.ResolveScreenNameAsync(id);
                dynamic res = JsonConvert.DeserializeObject(json);

                string type = res.response?.type;
                if (type== "application")
                    return StatusCode(StatusCodes.Status500InternalServerError, "application profile");
                json = await _vkApiService.GetProfileAsync(id, type);
                

                // var json = await _vkApiService.GetUsersAsync(id);
                //парсить ошибку
                res = JsonConvert.DeserializeObject(json);
                //if (res.error != null)
                //{
                //    if (res.error.error_code == 5)
                //        return StatusCode(StatusCodes.Status500InternalServerError, "no access_token");
                //    return StatusCode(StatusCodes.Status500InternalServerError, res.error.error_code+" "+ res.error.error_message);
                //}
                string ProfileId= (string)res.response[0].id; ;
                if (type == "group")
                    ProfileId = "-" + ProfileId;
                profile = await _profiles.GetByIdAsync(ProfileId);
                if (profile != null)
                    await _profiles.Delete(profile.Id);
                profile = _parseVk.ParseProfile(json,type);

                //json = await _vkApiService.GetWallAsync(profile.Id);

                wallItems = _parseVk.ParseWall(await _vkApiService.GetWallAsync(profile.Id));

            //---------------------------------------------------
            var likeUsers = new List<LikeUser>();
            int index = 1;
            foreach (var item in wallItems)
            {
                dynamic likes;
                //do
                //{
                //    await Task.Delay(100);
                //    var likesJson = await _vkApiService.GetLikesAsync(profile.Id, item.ItemId, "post");
                //    likes = JsonConvert.DeserializeObject(likesJson);
                //} while (likes.error?.error_code == 6);





                    await Task.Delay(350);
                    //var likesJson = await _vkApiService.GetLikesAsync(profile.Id, item.ItemId, "post");
                    var likesJson = await _vkApiService.GetLikeUsersAsync(profile.Id, item.ItemId, "post");
                    likes = JsonConvert.DeserializeObject(likesJson);


                Debug.WriteLine("Wall "+index+++": " +item.ItemId);
                foreach (var ul in likes.response)
                {
                    
                    //dynamic ul;

                    //    await Task.Delay(350);
                    //    var userLikeJson = await _vkApiService.GetProfileAsync(userId.ToString(),"user");
                    //    ul = JsonConvert.DeserializeObject(userLikeJson);


                    var likeUser = likeUsers.FirstOrDefault(l => l.OwnerId == ul.id.ToString());
                    if (likeUser == null)
                    {
                        likeUser = new LikeUser
                        {
                            OwnerId = ul.id.ToString(),
                            ProfileId = profile.Id,
                            FullName = ul.first_name.ToString() + " " + ul.last_name.ToString(),
                            PhotoUrl = ul.photo_100,
                            LikeCount = 1
                        };
                        likeUsers.Add(likeUser);
                    }
                    else
                    {
                        likeUser.LikeCount++;
                        Debug.WriteLine("yyyyy:");
                    }
                    

                    //item.WallItemLikes.Add(new WallItemLike
                    //{
                    //    LikeUser = likeUser,
                    //    WallItem = item
                    //});
                }

            }

            profile.WallItems = wallItems;
            profile.LikeUsers = likeUsers;
            await _profiles.CreateAsync(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(JsonConvert.SerializeObject(profile));
            //return StatusCode(StatusCodes.Status400BadRequest);
        }

        [Route("getAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            var r = await _profiles.GetAllAsync();
            return Ok(JsonConvert.SerializeObject(r));
        }

        [Route("getProfile")]
        public async Task<IActionResult> GetProfileAsync(string id)
        {
            return Ok(JsonConvert.SerializeObject(await _profiles.GetByIdAsync(id)));
        }

        [Route("delete")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _profiles.Delete(id);
            return Ok();
        }

        private string GetUrlFromAttachment(Attachment attachment, string type)
        {
            switch (type)
            {
                case "video":
                    return attachment.video.image[2].url;
                case "doc":
                    return attachment.doc.preview.photo.sizes[2].src;
                case "photo":
                    return attachment.photo.sizes[2].url;
                default:
                    return "";
            }
        }
    }
}