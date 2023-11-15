
using System.ComponentModel.DataAnnotations;

namespace SportMeApp.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Sport { get; set; }
        public string EventTime { get; set; }
        public string Location { get; set; }
        public double fee { get; set; }
        public List<UserGroup> UserGroups { get; set; }
        public List<Message> Messages { get; set; }

    }
}
