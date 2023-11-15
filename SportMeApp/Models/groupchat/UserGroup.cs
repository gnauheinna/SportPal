using System.ComponentModel.DataAnnotations;

namespace SportMeApp.Models
{
    public class UserGroup
    {
        [Key]
        public int UserGroupId { get; set; }
        public int UserId { get; set; }

        //navigation property for user:allow to easily related entities without need for an explicit join
        public User User { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
        
    }

}

