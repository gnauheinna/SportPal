
using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? PaypalAccount { get; set; }

        // Foreign key 
        [ForeignKey("Locations")]
        public int LocationId { get; set; }
        public Locations Locations { get; set; }

        [ForeignKey("Sport")]
        public int SportId { get; set; }
        public Sport Sport { get; set; }

        //indicating many-to-many relationship between events and users
        public List<UserEvent> UserEvents { get; set; }
        public List<Message> Messages { get; set; }

    }
}
