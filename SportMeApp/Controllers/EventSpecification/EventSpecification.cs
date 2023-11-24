using Microsoft.AspNetCore.Mvc;

namespace SportMeApp.Controllers.EventSpecification
{
    public class EventSpecification : Controller
    {
        public IActionResult GymDetail()
        {
            return View();
        }
    }
}
