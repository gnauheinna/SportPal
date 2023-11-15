using Microsoft.AspNetCore.Mvc;

namespace SportMeApp.Controllers.GroupChat.View
{
    public class GroupChat : Controller
    {
        public IActionResult RegisterEvent()
        {
            return View();
        }
    }
}
