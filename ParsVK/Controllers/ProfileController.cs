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
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private IVkApiService _vkApiService;
        private IProfileRepository _profiles;
        private ParseVkService _parseVk;
        public ProfileController(IVkApiService vkApiService, IProfileRepository profiles, ParseVkService parseVk)
        {
            _parseVk = parseVk;
            _profiles = profiles;
            _vkApiService = vkApiService;
        }
     
        [Route("GetToken")]
        public async Task<IActionResult> GetToken(string code)
        {
            try
            {
                await _vkApiService.GetTokenAsync(code);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok();
        }

        [Route("ParseProfile")]
        public async Task<IActionResult> ParseProfile(string link)
        {
            if (String.IsNullOrEmpty(link))
                return StatusCode(StatusCodes.Status400BadRequest, "profile link");
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
                res = JsonConvert.DeserializeObject(json);
                string ProfileId= (string)res.response[0].id; ;
                if (type == "group")
                    ProfileId = "-" + ProfileId;
                profile = await _profiles.GetByIdAsync(ProfileId);
                if (profile != null)
                    await _profiles.DeleteAsync(profile.Id);
                profile = _parseVk.ParseProfile(json,type);

                wallItems = _parseVk.ParseWall(await _vkApiService.GetWallAsync(profile.Id));

            //---------------------------------------------------
                var likeUsers = new List<LikeUser>();
                for (int i = 0; i < wallItems.Count; i=i+12)
                {
                    int count = wallItems.Count - i > 12 ? 12 : wallItems.Count - i;
                    dynamic likes;
                    //do
                    //{
                    //    await Task.Delay(100);
                    //    var likesJson = await _vkApiService.GetLikesAsync(profile.Id, item.ItemId, "post");
                    //    likes = JsonConvert.DeserializeObject(likesJson);
                    //} while (likes.error?.error_code == 6);
                    await Task.Delay(350);
                    //var likesJson = await _vkApiService.GetLikesAsync(profile.Id, item.ItemId, "post");
                    // var likesJson = await _vkApiService.GetLikeUsersAsync(profile.Id, item.ItemId, "post");
                    var likesJson = await _vkApiService.GetLikeUsersAsync(profile.Id, String.Join(',', wallItems.GetRange(i, count).Select(wi => wi.ItemId)), "post");
                    likes = JsonConvert.DeserializeObject(likesJson);
                    Debug.WriteLine("wallItemsParseCount: " + i);
                    foreach (var ul in likes.response)
                    {
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
        }

        [HttpGet]
        public async Task<ActionResult<List<Profile>>> Get()
        {
            var r = await _profiles.GetAllAsync();
            return r;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var p = await _profiles.GetByIdAsync(id);
            if (p == null)
                return NotFound();
            return Ok(JsonConvert.SerializeObject(p));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var p = await _profiles.GetByIdAsync(id);
            if (p == null)
                return NotFound();
            await _profiles.DeleteAsync(id);
            return NoContent();
        }
    }
}