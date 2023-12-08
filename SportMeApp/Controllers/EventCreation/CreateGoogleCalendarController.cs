using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using SportMeApp.Services;
using SportMeApp.Models;
using Newtonsoft.Json;
using SportMeApp.Controllers.EventCreation;



namespace SportMeApp.Controllers.CreateEvent1
{
    [ApiController]
    [Route("CreateGoogleCalendar")]
    public class CreateGoogleCalendarController : Controller
    {
        private readonly ILogger<CreateGoogleCalendarController> _logger;
        private readonly SportMeContext _context;

        public CreateGoogleCalendarController(ILogger<CreateGoogleCalendarController> logger, SportMeContext context)
        {
            _logger = logger;
            _context = context;
        }

        // function to create event on google calendar
        [HttpPost ("CreateGoogleCalendarEvent")]
        public ActionResult CreateGoogleCalendarEvent(CreateEvent eventData)
        {
            // Combine the base path of the application with the specific file path
            var tokensFileName = "tokens.json";
            var tokensFile = System.IO.Path.Combine(AppContext.BaseDirectory, "files", tokensFileName);

            string sport = GetCalendarIdBySportId(eventData.SportId);

            // Inserting a Google Calendar Event 
            // EZ as 1234
            _logger.LogInformation($"Received data: {JsonConvert.SerializeObject(eventData)}");
            // 1. Get the Google Calendar service instance.
            var service = GoogleCalendarService.GetCalendarService(tokensFile);

            // 2. Create a new google calendar event from user's input.

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
                    DateTime = eventData.EndTime,
                    TimeZone = "America/New_York",
                },
                Recurrence = new List<string> { },
                Attendees = new List<EventAttendee>
                { },
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

            // default calendar ID
            string calendarId = "e4c1ddafe23c0856595af11ded6dfeef3b4fe9dc51cfae87b9a9c0d97bf2d4c7@group.calendar.google.com";


            // 3. Choose correct calendarId based on sports.
            // Use a switch statement based on the sportName
            switch (sport)
            {
                case "tennis":
                    calendarId = "e4c1ddafe23c0856595af11ded6dfeef3b4fe9dc51cfae87b9a9c0d97bf2d4c7@group.calendar.google.com";
                    break;
                case "baseball":
                    calendarId = "dcba7de1c5f8598fe103723b195c485848a97e53285972f8a9882726cb541039@group.calendar.google.com";
                    break;
                case "basketball":
                    calendarId = "cdd08cff32ab443727d461d9f138d150ee13bbff82733a6e061c310a901f513b@group.calendar.google.com";
                    break;
                case "volleyball":
                    calendarId = "63b33d59f80d137b9977e4384bb9ca59a7fcc225a33243ab242882b1aa08ca6b@group.calendar.google.com";
                    break;
                case "soccer":
                    calendarId = "dd10b96a0e8d139319ce6ea458dba2931cd382a3a0ffe9086f526edd26fcd54d@group.calendar.google.com";

                    break;
                default:
                    // Default logic for unknown sportName
                    calendarId = "cdd08cff32ab443727d461d9f138d150ee13bbff82733a6e061c310a901f513b@group.calendar.google.com";
                    break;
            }
          

            // 4. Insert the event into the Calendar
            EventsResource.InsertRequest request = service.Events.Insert(newEvent, calendarId);
            Google.Apis.Calendar.v3.Data.Event createdEvent = request.Execute();

            ViewBag.HtmlLink = createdEvent.HtmlLink;

            return Ok(newEvent);

        }

        // function to return the correct sport name based on sportID
        private string GetCalendarIdBySportId(int sportId)
        {
            // Query the Sport table using the provided context to get sportName based on sportId
            var sport = _context.Sport.FirstOrDefault(s => s.SportId == sportId);

           // initialize string to hold sportname
            string sportName = "";

            if (sport != null)
            {
                // Map sportId to the corresponding sportname based on your sportID
                sportName = sport.SportName ?? "";
            }

            return sportName;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
