using Microsoft.AspNetCore.Mvc;
using SportMeApp.Models;
using System.Diagnostics;

namespace SportMeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public ActionResult OauthRedirect()
        {
            var client_id = "378858678415-the1gfbovl66l9jbmobcufom12a5kche.apps.googleusercontent.com";

            var redirectUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                "scope=https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events" +
                "&access_type=online&" +
                "include_granted_scopes=true&" +
                "response_type=code&" +
                "state=there&" +
                "redirect_uri=https://localhost:7292/oauth/callback&" +
                "client_id=" + client_id;

            return Redirect(redirectUrl);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}