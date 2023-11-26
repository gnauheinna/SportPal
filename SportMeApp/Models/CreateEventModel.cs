
using NuGet.Protocol.Core.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportMeApp.Models
{
    public class CreateEvent
    {
        [Key]
        public int EventId { get; set; }
        public string EventName { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Fee { get; set; }
        public string PaypalAccount { get; set; }
        // Foreign key 
        [ForeignKey("Locations")] 
        public int LocationId { get; set; }
      

        [ForeignKey("Sport")]
        public int SportId { get; set; }

   

    }
}
