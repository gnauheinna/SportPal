using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SportMeApp.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string Coordinates { get; set; }
        public string Address { get; set; }
        public Boolean IsTennis { get; set; }
        public Boolean IsVolleyball { get; set; }
        public Boolean IsBasketball { get; set; }

        public byte[]? ImageData { get; set; }

        // Navigation property to represent the relationship
        public List<Event> Events { get; set; }

    }
}
