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

        

      

        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] string userName)
        {
            // check if user exists
            var existingUser = _context.User.FirstOrDefault(u => u.Username == userName);

            if (existingUser != null)
            {
                return Ok("user already exists");
            }
            else
            {
                var newUser = new User { Username = userName };
                _context.User.Add(newUser);
                _context.SaveChanges();
                return Ok("User added succesfully");
            }
        }

        [HttpGet("GetUsers")]
        public IActionResult GetAllUsers()
        {
            var users = _context.User.ToList();
            return Ok(users);
        }
    }
}
