// createGroup, AddUserToGroup,GetGroupName

using SportMeApp.Controllers.GroupChat.dataTransferObject;
using SportMeApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace SportMeApp.Controllers
{
    [ApiController]
    [Route("api/Group/")]
    public class GroupController : ControllerBase
    {
        private readonly SportMeContext _context;
        public GroupController(SportMeContext context)
        {
            _context = context;
        }

        [HttpPost("CreateGroup")]
        public async Task<IActionResult> CreateGroup([FromBody] GroupCreationModel groupData)
        {
            if (string.IsNullOrEmpty(groupData.EventName))
            {
                return BadRequest("Group name cannot be empty.");
            }

            var group = new Models.Event { EventName = groupData.EventName };
            _context.Event.Add(group);

            if (groupData.SelectedUsers != null && groupData.SelectedUsers.Count > 0)
            {
                var users = _context.User.Where(u => groupData.SelectedUsers.Contains(u.Username)).ToList();
                foreach (var user in users)
                {
                    if (group != null && user != null)
                    {
                        if (group.UserEvents == null)
                        {
                            group.UserEvents = new List<UserEvent>();
                        }

                        group.UserEvents.Add(new UserEvent { User = user, Event = group });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Group added: " + group.EventName);
        }

        [HttpPost("AddUserToGroup")]
        public async Task<IActionResult> AddUserToGroup([FromBody] int userId, int EventId)
        {
            var userGroup = new UserEvent { UserId = userId, EventId = EventId };
            _context.UserEvent.Add(userGroup);
            await _context.SaveChangesAsync();
            return Ok("User added to group: " + userGroup);
        }

       
    }
}
