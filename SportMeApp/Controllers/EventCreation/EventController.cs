
using Microsoft.AspNetCore.Mvc;
using SportMeApp.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using SportMeApp.Services;



namespace SportMeApp.Controllers.EventCreation
{
    [ApiController]
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
        
        // function to create event in database EZ as 123
        [HttpPost ("CreateEvent")]
        public async Task<IActionResult> CreateEvent(CreateEvent eventData)
        {
            try
            {   
                if (ModelState.IsValid) ///checks if the pass in eventData is valid
                {
                    // 1. Create a new Event instance based on the input event
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
                    _logger.LogInformation("LOG: New Event Details - EventName: {EventName}, StartTime: {StartTime}, EndTime: {EndTime}, Fee: {Fee}, LocationId: {LocationId}, SportId: {SportId}, PaypalAccount: {PaypalAccount}",
                    newEvent.EventName, newEvent.StartTime, newEvent.EndTime, newEvent.Fee, newEvent.LocationId, newEvent.SportId, newEvent.PaypalAccount);
                    // 2. Add the event to the Event table in database
                    _context.Event.Add(newEvent);
                    // 3. await db to save changes
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("LOG:Event created successfully.");
                    // 4. return ok!!
                    return Ok(newEvent);
                }

                _logger.LogError("LOG:Model validation failed.");
                // Model validation failed
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                _logger.LogError($"LOG: An error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }



        public IActionResult CreateEvent()
        {
            return View();
        }
    }
}
