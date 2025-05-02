using System.ComponentModel.DataAnnotations;

namespace EventEae1._2_Backend.Models
{
   
        public class User
        {

          [Key]
          public Guid Id { get; set; } = Guid.NewGuid();


           public string FirstName { get; set; }

           
            public string LastName { get; set; }

           
            public string Email { get; set; }

            public string Password { get; set; }

           
            [RegularExpression("client|manager|admin")]
            public string Role { get; set; } = "client";

         
            public string? Organization { get; set; }

         
            [RegularExpression("approved|pending|rejected")]
            public string Status { get; set; } = "approved";

            public ICollection<UserPermission> UserPermissions { get; set; }
            public ICollection<Event> OrganizedEvents { get; set; }
    }
    
}
