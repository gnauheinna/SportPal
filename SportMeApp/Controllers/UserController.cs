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


        // function to add user to event
        [HttpPost("{userId}/{EventId}/addUserToEvent")]
        public IActionResult addUserToEvent([FromBody] int userId,int EventId)
        {
            return Ok();
        }

        // function to add user to database
        [HttpPost("{userName}/AddUser")]
        public IActionResult AddUser([FromBody] string userName)
        {
            //1.  check if user exists base on username
            var existingUser = _context.User.FirstOrDefault(u => u.Username == userName);

            if (existingUser != null)
            {
                return Ok(existingUser);
            }
            else
            {
                var newUser = new User { Username = userName, Email=""};
                _context.User.Add(newUser);
                _context.SaveChanges();
                return Ok(newUser);
            }
        }

        [HttpGet("{userName}/GetUserByName")]
        public async Task<IActionResult> GetUserByName(string userName)
        {
            // 1. checks if user exist based on username
            var user = await _context.User.FirstOrDefaultAsync(u => u.Username == userName);
            // 2a. if user exist return ok and user
            if (user != null)
            {
                return Ok(user);
            }
            // 2b. if user doesn't exist return user not found
            return NotFound("User not found.");
        }


        // function to get user by email 
        [HttpGet("{userEmail}/GetUserByEmail")]
        public async Task<IActionResult> GetUserByEmail(string userEmail)
        {
            // 1. checks if user exists based on user email
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == userEmail);
            // 2a. if user exists
            if (user != null)
            {
                return Ok(user);
            }
            // 2b if user doesnt exists
            return NotFound("User not found.");
        }

        // function to get all users from database
        [HttpGet("GetUsers")]
        public IActionResult GetAllUsers()
        {
            // 1. get all users from database
            var users = _context.User.ToList();
            return Ok(users);
        }

        // function to populate the sports table EZ as 123
        [HttpPost("AddSport")]
        public IActionResult AddSportToDb()
        {
            // 1. initialize the list of sportnames to add
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
                // 2. loop through the sport list
                foreach (var sport in sportsToAdd)
                {
                    // 3. check if the sport exists
                    var existingSport = _context.Sport.FirstOrDefault(s => s.SportName == sport.SportName);

                    if (existingSport == null)
                    {
                        // 4a. if sport doesn't exists add to database
                        _context.Sport.Add(sport);
                    }
             
                }
                // 5. save changes
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
