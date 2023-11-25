using Microsoft.AspNetCore.Mvc;

namespace SportMeApp.Controllers.CreateEvent1
{
    public class EventCreationController : Controller
    {
        public IActionResult CreateForm()
        {
            return View();
        }
    }
}
