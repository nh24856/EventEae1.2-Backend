using System.ComponentModel.DataAnnotations;

namespace EventEae1._2_Backend.Models
{
   
        public class User
        {
            
            public string Id { get; set; }

            
            public string FirstName { get; set; }

           
            public string LastName { get; set; }

           
            public string Email { get; set; }

            public string Password { get; set; }

           
            [RegularExpression("client|manager|admin")]
            public string Role { get; set; } = "client";

         
            public string? Organization { get; set; }

         
            [RegularExpression("approved|pending|rejected")]
            public string Status { get; set; } = "approved";
        }
    
}
