using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportMeApp.Models
{
    public class Locations
    {
        [Key]
        public int LocationId { get; set; }
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public double? Rating { get; set; }
        public string? Address { get; set; }

        public String? WeekdayText { get; set; }
        //public List<Weekday> Weekdays { get; set; }

        public Boolean IsTennis { get; set; }
        public Boolean IsVolleyball { get; set; }
        public Boolean IsBasketball { get; set; }
        public Boolean IsBaseball { get; set; }
        public Boolean IsSoccer { get; set; }
        public string? FormattedPhoneNumber { get; set; }
        public string? ImageUrl { get; set; }

        // Navigation property to represent the relationship
        public List<Event> Events { get; set; }

    }


}
