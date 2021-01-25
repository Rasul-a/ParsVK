
using Newtonsoft.Json;

namespace ParsVK.Models
{
    public class LikeUser
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string ProfileId { get; set; }
        public string FullName { get; set; }
        public string PhotoUrl { get; set; }
        public int LikeCount { get; set; }
        [JsonIgnore]
        public Profile Profile { get; set; }

    }
}
