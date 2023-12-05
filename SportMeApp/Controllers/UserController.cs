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

        [HttpGet("{userEmail}/GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string userEmail)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == userEmail);

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

        [HttpPost("AddSport")]
        public IActionResult AddSportToDb()
        {
            
            var sportsToAdd = new List<Sport>
            {
                    new Sport { SportName = "volleyball" },
                    new Sport { SportName = "tennis" },
                    new Sport { SportName = "basketball" },
                    new Sport { SportName = "baseball" },
                     new Sport { SportName = "soccer" },
            };

            try
            {
                foreach (var sport in sportsToAdd)
                {
                    var existingSport = _context.Sport.FirstOrDefault(s => s.SportName == sport.SportName);

                    if (existingSport == null)
                    {
                        _context.Sport.Add(sport);
                    }
             
                }

                _context.SaveChanges();

                return Ok("Sports added or updated successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding or updating sports: {ex.Message}");
            }
        }
    }
}
