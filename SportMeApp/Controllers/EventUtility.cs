using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace SportMeApp.Controllers
{
    [ApiController]
    [Route("api/")]
    public class EventUtility : ControllerBase
    {
        private readonly SportMeContext _context;
        public EventUtility(SportMeContext context)
        {
            _context = context;
        }

        [HttpGet("{UserId}/GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(int UserId)
        {
            // Check if the user exists
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.UserId == UserId);

            if (user == null)
            {
                return NotFound($"User with ID {UserId} not found.");
            }

            // Project user data
            var userData = new
            {
                UserId = user.UserId,
                UserName = user.Username,
                Email = user.Email
            };

        

            // Project events data for the specific user
            var eventsData = await _context.UserEvent
                .Where(ue => ue.UserId == UserId)
                .Select(  ue => new {
                    EventId = ue.Event.EventId,
                    EventName = ue.Event.EventName,
                    StartTime = ue.Event.StartTime,
                    EndTime = ue.Event.EndTime,
                    FormattedTime = ue.Event.StartTime.Date == ue.Event.EndTime.Date ?
                                   ue.Event.StartTime.ToString("yyyy/MM/dd h") + ue.Event.StartTime.ToString("tt").ToLower() + " - " +
                                    ue.Event.EndTime.ToString("h") + ue.Event.EndTime.ToString("tt").ToLower() :
                                    ue.Event.StartTime.ToString("M/d h") + ue.Event.StartTime.ToString("tt").ToLower() + " - " +
                                    ue.Event.EndTime.ToString("M/d h") + ue.Event.EndTime.ToString("tt").ToLower() + " " +
                                    ue.Event.EndTime.ToString("yyyy"),
                    EventFee = ue.Event.Fee,
                    UsersInGroup = _context.UserEvent
                        .Where(ug => ug.EventId == ue.Event.EventId)
                        .Select(ug => ug.User.Username)
                        .Distinct()
                        .ToList(),
                    Sport = new
                    {
                        SportId = ue.Event.Sport.SportId,
                        SportName = ue.Event.Sport.SportName
                    },
                    Location = new
                    {
                        LocationId = ue.Event.Locations.LocationId,
                        Name = ue.Event.Locations.Name,
                        PlaceId = ue.Event.Locations.PlaceId,
                        lat = ue.Event.Locations.lat,
                        lng = ue.Event.Locations.lng,
                        Address = ue.Event.Locations.Address,
                        isTennis = ue.Event.Locations.IsTennis,
                        isVolleyball = ue.Event.Locations.IsVolleyball,
                        isBasketball = ue.Event.Locations.IsBasketball,
                        IsBaseball = ue.Event.Locations.IsBaseball,
                        IsSoccer = ue.Event.Locations.IsSoccer,
                        ImageData = ue.Event.Locations.ImageUrl
                    }
                    
                })
                .ToListAsync();

            // Combine user and events data
            var result = new
            {
                User = userData,
                Events = eventsData
            };

            return Ok(result);
        }



      

    }
}