using Microsoft.AspNetCore.Mvc;

namespace SportMeApp.Controllers
{
    public class SportMeAppContext : Controller
    {
        public IActionResult RegisterEvent()
        {
            return View();
        }
    }
}
