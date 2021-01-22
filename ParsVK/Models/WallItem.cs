using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParsVK.Models
{
    public class WallItem
    {
        public int Id { get; set; }
        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
        public string Text { get; set; }
        public string HistoryText { get; set; }
        public string HistoryId { get; set; }
        public string Url { get; set; }
        public string Type { get; set; }
    }
}
