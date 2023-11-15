using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportMeApp.Models;
using System.Text.RegularExpressions;

namespace SportMeApp.Controllers
{
    [ApiController]
    [Route("api/")]
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
            var group = await _context.Group.FindAsync(groupId);

            if (group != null)
            {
                return Ok(group.GroupName);
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

        // Endpoint to get groups for a specific user
        [HttpGet("{userId}/GetUserGroups")]
        public async Task<IActionResult> GetUserGroups(int userId)
        {
            // Check if the user exists
            var userExists = await _context.User.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                return NotFound("User not found.");
            }

            // Retrieve all groups associated with the specified user
            var userGroups = await _context.UserGroup
                .Where(ug => ug.UserId == userId)
                .Include(ug => ug.Group)
                .Select(ug => ug.Group)
                .ToListAsync();

            return Ok(userGroups);
        }

        [HttpGet("{GroupId}/UsersInGroup")]
        public async Task<IActionResult> UsersInGroup(int GroupId)
        {
            // Check if the group exists
            var groupExists = await _context.Group.AnyAsync(g => g.GroupId == GroupId);
            if (!groupExists)
            {
                return NotFound("Group not found.");
            }

            // Count the number of users in the specified group
            var UsersInGroup = await _context.UserGroup
                .Where(ug => ug.GroupId == GroupId)
                .Select(ug => ug.User.Username)
                .Distinct()
                .ToListAsync();

            return Ok(UsersInGroup);
        }
    }
}
