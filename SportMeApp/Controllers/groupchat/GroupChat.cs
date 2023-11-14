using Microsoft.AspNetCore.Mvc;

namespace SportMeApp.Controllers
{
    public class GroupChat : Controller
    {
        public IActionResult RegisterEvent()
        {
            return View();
        }
    }
}
