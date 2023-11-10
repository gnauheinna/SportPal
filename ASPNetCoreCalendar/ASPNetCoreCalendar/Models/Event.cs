using System.Runtime.CompilerServices;
using ASPNetCoreCalendar.Models;


namespace ASPNetCoreCalendar.Models
{
    public class Event
    {
        public Event() 
        {
            this.Start = new EventDateTime()
            {
                TimeZone = "Eastern Time - New York"

            };

            this.End = new EventDateTime()
            {
                TimeZone = "Eastern Time - New York"

            };

        }

        public string Summary {  get; set; }
        public string Description { get; set; }
        public EventDateTime Start { get; set; }
        public EventDateTime End { get; set; }

    }
    public class EventDateTime
    {
        public string DateTime { get; set; }
        public string TimeZone { get; set; }
    }
}
