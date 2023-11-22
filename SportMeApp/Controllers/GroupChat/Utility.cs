using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMeApp.Models;
using System.Text.RegularExpressions;

namespace SportMeApp.Controllers
{
    [ApiController]
    [Route("api/group/")]
    public class Utility : ControllerBase
    {
        private readonly SportMeContext _context;
        public Utility(SportMeContext context)
        {
            _context = context;
        }

        [HttpGet("{groupId}/GetGroupName")]
        public async Task<IActionResult> GetGroupName([FromRoute] int groupId)
        {
            // Retrieve the group from the database using the provided groupId
            var group = await _context.Event.FindAsync(groupId);

            if (group != null)
            {
                return Ok(group.EventName);
            }
            else
            {
                return NotFound(); // Or any appropriate response for a group not found
            }
        }

        [HttpGet("{userName}/GetUserByName")]
        public async Task<IActionResult> GetUserByName(string userName)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Username == userName);

            if (user != null)
            {
                return Ok(user);
            }

            return NotFound("User not found.");
        }


  
        [HttpGet("{userId}/GetUserById")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("User not found.");
        }

        

        [HttpGet("{EventId}/UsersInGroup")]
        public async Task<IActionResult> UsersInGroup(int EventId)
        {
            // Check if the group exists
            var groupExists = await _context.Event.AnyAsync(g => g.EventId == EventId);
            if (!groupExists)
            {
                return NotFound("Group not found.");
            }

            // Count the number of users in the specified group
            var UsersInGroup = await _context.UserEvent
                .Where(ug => ug.EventId == EventId)
                .Select(ug => ug.User.Username)
                .Distinct()
                .ToListAsync();

            return Ok(UsersInGroup);
        }
        
    }
}
