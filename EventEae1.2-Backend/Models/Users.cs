using System.ComponentModel.DataAnnotations;

namespace EventEae1._2_Backend.Models
{
   
        public class User
        {
            
            public string Id { get; set; }

            
            public string Firstname { get; set; }

           
            public string Lastname { get; set; }

           
            public string Email { get; set; }

            public string Password { get; set; }

           
            [RegularExpression("user|manager|admin")]
            public string Role { get; set; } = "user";

         
            public string? Organization { get; set; }

         
            [RegularExpression("approved|pending|rejected")]
            public string Status { get; set; } = "approved";
        }
    
}
