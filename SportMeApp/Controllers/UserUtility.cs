using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SportMeApp.Controllers
{
    [ApiController]
    [Route("api/")]
    public class UserUtility : ControllerBase
    {
        private readonly SportMeContext _context;
        public UserUtility(SportMeContext context)
        {
            _context = context;
        }

      
    }
}
