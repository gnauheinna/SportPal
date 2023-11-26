
using Microsoft.AspNetCore.Mvc;
using SportMeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SportMeApp.Services;



namespace SportMeApp.Controllers.CreateEvent1
{
    [Route("Event")]
    public class EventController : Controller
    {
        private readonly ILogger<EventController> _logger;
        private readonly SportMeContext _context;
        public EventController(SportMeContext context, ILogger<EventController> logger)
        {
            _context = context;
            _logger = logger;
        }
        

        [HttpPost ("CreateEvent")]
        public async Task<IActionResult> CreateEvent(CreateEvent eventData)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var newEvent = new Event
                    {
                        EventName = eventData.EventName,
                        StartTime = eventData.StartTime,
                        EndTime = eventData.EndTime,
                        Fee = eventData.Fee,
                        LocationId = eventData.LocationId,
                        SportId = eventData.SportId,
                         PaypalAccount = eventData.PaypalAccount
                    };

                    _context.Event.Add(newEvent);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Event created successfully.");

                    return RedirectToAction("CreateGoogleCalendarEvent", " CreateGoogleCalendar");
                }

                _logger.LogError("Model validation failed.");
                // Model validation failed
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



        public IActionResult CreateEvent()
        {
            return View();
        }
    }
}
