using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParsVK.Models.JsonModel
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Image
    {
        public string url { get; set; }
       // public int width { get; set; }
    }

    public class Video
    {
        public List<Image> image { get; set; }
    }

    public class Size
    {
        public string src { get; set; }
        public string url { get; set; }
       // public int width { get; set; }
    }

    public class Photo
    {
        public int id { get; set; }
        public List<Size> sizes { get; set; }
      //  public string text { get; set; }
    }

    public class Preview
    {
        public Photo photo { get; set; }
        public Video video { get; set; }
    }

    public class Doc
    {
        public int id { get; set; }
        public int owner_id { get; set; }
        public int date { get; set; }
        public int type { get; set; }
        public string url { get; set; }
        public Preview preview { get; set; }
    }

    public class Attachment
    {
        public string type { get; set; }
        public Video video { get; set; }
        public Photo photo { get; set; }
        public Doc doc { get; set; }
    }

    public class CopyHistory
    {
        public int id { get; set; }
        public int owner_id { get; set; }
        public string text { get; set; }
        public List<Attachment> attachments { get; set; }
    }

    public class Comments
    {
        public int count { get; set; }
    }

    public class Likes
    {
        public int count { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public int owner_id { get; set; }
        public string post_type { get; set; }
        public string text { get; set; }
        public List<CopyHistory> copy_history { get; set; }
        public Comments comments { get; set; }
        public Likes likes { get; set; }
        public List<Attachment> attachments { get; set; }
    }

    public class Response
    {
        public int count { get; set; }
        public List<Item> items { get; set; }
    }


    public class WallGet
    {
        public Response response { get; set; }
    }


}
