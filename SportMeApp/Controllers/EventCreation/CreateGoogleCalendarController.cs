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
                Summary = "Annie's Birthday BasketBall Game",
                Location = "800 Howard St., San Francisco, CA 94103",
                Description = "A chance to play with annie and the folks.",
                Start = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2023-11-28T09:00:00-07:00"),
                    TimeZone = "America/New_York",
                },
                End = new EventDateTime()
                {
                    DateTime = DateTime.Parse("2023-11-28T17:00:00-07:00"),
                    TimeZone = "America/New_York",
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
            string calendarId = "dcba7de1c5f8598fe103723b195c485848a97e53285972f8a9882726cb541039@group.calendar.google.com";
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
