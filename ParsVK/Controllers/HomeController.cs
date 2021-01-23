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
        public HomeController(IVkApiService vkApiService, IProfileRepository profiles, IRepository<LikeUser> likeUsers)
        {
            _likeUsers = likeUsers;
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
            //парсить ошибку
            dynamic res = JsonConvert.DeserializeObject(json);
               Profile profile = await _profiles.GetByIdAsync((string)res.response[0].id);
            //Profile profile = null;
            if (profile == null)
                profile = new Profile();

            profile.Id = (string)res.response[0].id;
            profile.FirstName = res.response[0].first_name;
            profile.LastName = res.response[0].last_name;
            profile.City = res.response[0].city?.title;
            profile.Bdate = res.response[0].bdate;
            profile.PhotoUrl = res.response[0].photo_100;
            profile.Audios = res.response[0].counters?.audios;
            profile.Friends = res.response[0].counters?.friends;
            profile.Groups = res.response[0].counters?.groups;
            profile.Photos = res.response[0].counters?.photos;

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
                a.Id = item.id.ToString();
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



            // List<WallItem> LikesItems = wallItems.OrderByDescending(a => a.LikesCount).Take(10).ToList();
            // List<WallItem> CommentsItems = wallItems.OrderByDescending(a => a.CommentsCount).Take(10).ToList();

            List<LikeUser> likeUsers;
            likeUsers = new List<LikeUser>();
            index = 1;
            foreach (var item in wallItems)
            {
                
                
                dynamic likes;
                do
                {
                    await Task.Delay(100);
                    var likesJson = await _vkApiService.GetLikesAsync(profile.Id, item.Id, "post");
                    likes = JsonConvert.DeserializeObject(likesJson);
                } while (likes.error?.error_code==6);
                
                
                Debug.WriteLine("Wall "+index+++": " +item.Id);
                foreach (var userId in likes.response?.items)
                {
                    
                    dynamic ul;
                    do
                    {
                        await Task.Delay(100);
                        var userLikeJson = await _vkApiService.GetUsersAsync(userId.ToString());
                        ul = JsonConvert.DeserializeObject(userLikeJson);
                    } while (ul.error?.error_code == 6);

                    var likeUser = likeUsers.FirstOrDefault(l => l.OwnerId == userId.ToString());
                    if (likeUser == null)
                    {
                        likeUser = new LikeUser
                        {
                            OwnerId = userId.ToString(),
                            ProfileId = profile.Id,
                            FullName = ul.response[0].first_name.ToString() + " " + ul.response[0].last_name.ToString(),
                            PhotoUrl = ul.response[0].photo_100,
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
            //if (await _profiles.GetByIdAsync(profile.Id) == null)
              //  await _profiles.CreateAsync(profile);
            //else
                await _profiles.UpdateAsync(profile);
            //String.Join(',',LikesItems.Select(a=>a.Id+"="+a.LikesCount))+"---"+ String.Join(',', CommentsItems.Select(a => a.Id+"="+a.CommentsCount)) + "---" + String.Join(',', LikesItems.Union(CommentsItems).Select(a => a.Id))
            return Ok(JsonConvert.SerializeObject(profile));
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