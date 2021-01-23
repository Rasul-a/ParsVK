
using Newtonsoft.Json;

namespace ParsVK.Models
{
    public class WallItem
    {
        public string Id { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
        public string Text { get; set; }
        public string HistoryText { get; set; }
        public string HistoryId { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }

        public string ProfileId{ get; set; }
        [JsonIgnore]
        public Profile Profile { get; set; }

        
    }
}
