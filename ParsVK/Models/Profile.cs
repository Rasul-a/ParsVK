
namespace ParsVK.Models
{

    public class Profile
    {
        //[JsonProperty("id")]
        public string Id { get; set; }
    
        public string FirstName { get; set; }

     
        public string LastName { get; set; }

        public string Bdate { get; set; }

        public string City { get; set; }
      
        public string PhotoUrl { get; set; }
        public int Audios { get; set; }
        public int Photos { get; set; }
        public int Friends { get; set; }
        public int Groups { get; set; }
    }
}
