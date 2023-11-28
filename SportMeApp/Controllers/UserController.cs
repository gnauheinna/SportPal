// GetUserGroups,GetUserName,GetUserId,
// not used: AddUser,GetUser
using SportMeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SportMeApp.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {

        private readonly SportMeContext _context;

        public UserController(SportMeContext context)
        {
            _context = context;
        }



        [HttpPost("{userId}/{EventId}/addUserToEvent")]
        public IActionResult addUserToEvent([FromBody] int userId,int EventId)
        {
            return Ok();
        }

        [HttpPost("{userName}/AddUser")]
        public IActionResult AddUser([FromBody] string userName)
        {
            // check if user exists
            var existingUser = _context.User.FirstOrDefault(u => u.Username == userName);

            if (existingUser != null)
            {
                return Ok(existingUser);
            }
            else
            {
                var newUser = new User { Username = userName, Email="hi"};
                _context.User.Add(newUser);
                _context.SaveChanges();
                return Ok(newUser);
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

        [HttpGet("GetUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _context.User.ToList();
            return Ok(users);
        }
    }
}
