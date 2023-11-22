using System.ComponentModel.DataAnnotations;

namespace SportMeApp.Models
{
    public class UserEvent
    {
        [Key]
        public int UserEventId { get; set; }
        public int UserId { get; set; }

        //navigation property for user:allow to easily related entities without need for an explicit join
        public User User { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }
        
    }

}

