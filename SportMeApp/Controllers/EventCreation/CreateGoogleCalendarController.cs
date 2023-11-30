using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Mvc;
using SportMeApp.Services;
using SportMeApp.Models;
using Newtonsoft.Json;
using SportMeApp.Controllers.EventCreation;


namespace SportMeApp.Controllers.CreateEvent1
{
    [Route("CreateGoogleCalendar")]
    public class CreateGoogleCalendarController : Controller
    {
        private readonly ILogger<CreateGoogleCalendarController> _logger;

        public CreateGoogleCalendarController(ILogger<CreateGoogleCalendarController> logger)
        {
            _logger = logger;
        }


        [HttpPost ("CreateGoogleCalendarEvent")]
        public ActionResult CreateGoogleCalendarEvent(CreateEvent eventData)
        {
            _logger.LogInformation($"Received data: {JsonConvert.SerializeObject(eventData)}");
            // Get the Google Calendar service.
            var service = GoogleCalendarService.GetCalendarService();

            // Create an event.
            Google.Apis.Calendar.v3.Data.Event newEvent = new Google.Apis.Calendar.v3.Data.Event()
            {
                Summary = eventData.EventName,
                Location = eventData.LocationId.ToString(),
                Description = eventData.EventName,
                Start = new EventDateTime()
                {
                    DateTime = eventData.StartTime,
                    TimeZone = "America/New_York",
                },
                End = new EventDateTime()
                {
                    DateTime =eventData.EndTime,
                    TimeZone = "America/New_York",
                },
                Recurrence = new List<string> { },
                Attendees = new List<EventAttendee>
                {},
                Reminders = new Google.Apis.Calendar.v3.Data.Event.RemindersData()
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
            Google.Apis.Calendar.v3.Data.Event createdEvent = request.Execute();

            ViewBag.HtmlLink = createdEvent.HtmlLink;

            return Ok(newEvent);

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
