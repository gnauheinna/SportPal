
using System.ComponentModel.DataAnnotations;

namespace SportMeApp.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        //navitation for many-to-many relationship
        public ICollection<UserGroup> UserGroups { get; set; }
    }
}
