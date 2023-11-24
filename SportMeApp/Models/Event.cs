
using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;

namespace SportMeApp.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string EventName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Fee { get; set; }

        // Foreign key 
        public int LocationId { get; set; }
        //public Location Location { get; set; }

        public int SportId { get; set; }
        //public Sport Sport { get; set; }

        // indicating many-to-many relationship between events and users
        //public List<UserEvent> UserEvents { get; set; }
       // public List<Message> Messages { get; set; }

    }
}
