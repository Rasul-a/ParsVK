using Newtonsoft.Json;
using ParsVK.Models;
using ParsVK.Models.JsonModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ParsVK.Services
{
    public class ParseVkService
    {
        public Profile ParseProfile(string json, string type)
        {
            dynamic res = JsonConvert.DeserializeObject(json);
            var profile = new Profile();
            if (type == "user")
            {
                profile.Id = (string)res.response[0].id;
                profile.FirstName = res.response[0].first_name;
                profile.LastName = res.response[0].last_name;
                profile.City = res.response[0].city?.title;
                profile.Bdate = res.response[0].bdate;
                profile.PhotoUrl = res.response[0].photo_100;
                profile.Audios = res.response[0].counters?.audios ?? 0;
                profile.Friends = res.response[0].counters?.friends ?? 0;
                profile.Groups = res.response[0].counters?.groups ?? 0;
                profile.Photos = res.response[0].counters?.photos ?? 0;
                profile.Type = "user";     
            }
            else
            {
                profile.Id = "-"+(string)res.response[0].id;
                profile.FirstName = res.response[0].name;
                profile.PhotoUrl = res.response[0].photo_100;
                profile.MembersCount = res.response[0].members_count ?? 0;
                profile.Type = "group";
            }
            return profile;

        }

        public List<WallItem> ParseWall(string json)
        {
            var wallGet = JsonConvert.DeserializeObject<WallGet>(json);
            List<WallItem> wallItems = new List<WallItem>();
   
            int index = 1;
            foreach (var item in wallGet.response.items)
            {
                var wi = new WallItem();
                wi.ItemId = item.id.ToString();
                wi.Text = item.text;
                wi.LikesCount = item.likes.count;
                wi.CommentsCount = item.comments.count;

                if (item.copy_history != null)
                {
                    wi.HistoryText = item.copy_history[0].text;
                    wi.Type = item.copy_history[0].attachments?[0].type;
                    wi.Url = GetUrlFromAttachment(item.copy_history[0].attachments?[0], wi.Type);
                }
                else
                if (item.attachments != null)
                {
                    wi.Type = item.attachments[0].type;
                    wi.Url = GetUrlFromAttachment(item.attachments?[0], wi.Type);
                }
                wallItems.Add(wi);
                Debug.WriteLine(index++ + ": " + item.id);
            }
            return wallItems;
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
