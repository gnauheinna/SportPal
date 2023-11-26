using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Mvc;
using SportMeApp.Services;

namespace SportMeApp.Controllers.CreateEvent1
{
    public class CreateGoogleCalendarController : Controller
    {

        [HttpPost]
        public ActionResult CreateGoogleCalendarEvent()
        {
            // Get the Google Calendar service.
            var service = GoogleCalendarService.GetCalendarService();

            // Create an event.
            Event newEvent = new Event()
            {
                Summary = "Google I/O 2023",
                Location = "800 Howard St., San Francisco, CA 94103",
                Description = "A chance to hear more about Google's developer products.",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2023-11-28T09:00:00-07:00"),
                    TimeZone = "America/Los_Angeles",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2023-11-28T17:00:00-07:00"),
                    TimeZone = "America/Los_Angeles",
                },
                Recurrence = new List<string> { "RRULE:FREQ=DAILY;COUNT=2" },
                Attendees = new List<EventAttendee>
            {
                new EventAttendee() { Email = "lpage@example.com" },
                new EventAttendee() { Email = "sbrin@example.com" },
            },
                Reminders = new Event.RemindersData()
                {
                    UseDefault = false,
                    Overrides = new List<EventReminder>
                {
                    new EventReminder() { Method = "email", Minutes = 24 * 60 },
                    new EventReminder() { Method = "sms", Minutes = 10 },
                }
                }
            };

            // Insert the event into the user's primary calendar.
            string calendarId = "bc7495931fde2bcd23662e0c4b828c317cd7667a38bd28caec5f45b33da3a313@group.calendar.google.com";
            EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
            Event createdEvent = request.Execute();

            ViewBag.HtmlLink = createdEvent.HtmlLink;

            return RedirectToAction("Index", "Home");

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
