using Microsoft.AspNetCore.Mvc;

namespace SportMeApp.Controllers.EventSpecification
{
    public class EventSpecification : Controller
    {
        // this function returns the location specification view
        public IActionResult GymDetail()
        {
            return View();
        }
    }
}
