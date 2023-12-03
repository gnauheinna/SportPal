using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using RestSharp;
using NuGet.Common;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using SportMeApp.Models;

namespace SportMeApp.Controllers
{

    public class OAuthController : Controller
    {
        private SportMeContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public OAuthController(IWebHostEnvironment hostingEnvironment, SportMeContext context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
        }

        public IActionResult Index()
        {
            
            return View();
        }

        public ActionResult Callback(string code, string error, string state)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                // Process the authorization code
                var tokens = GetTokens(code);
                // Retrieve the user's email address
                var userEmail = GetUserEmail();

                this.AddUser(userEmail);
                // Redirect to the "Index" action
                return RedirectToAction("Index","GoogleMap");
            }
            else
            {
                // Handle the case where an error is present
                return View("Error");
            }
        }

        public ActionResult GetTokens(string code)
        {
            try
            {
                var contentRootPath = _hostingEnvironment.ContentRootPath;
                var filePath = Path.Combine(contentRootPath, "files", "tokens.json");

                var tokensFile = filePath;
                var client_id = "378858678415-the1gfbovl66l9jbmobcufom12a5kche.apps.googleusercontent.com";
                var client_secret = "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";

                RestClient restClient = new RestClient("https://oauth2.googleapis.com/token");
                RestRequest request = new RestRequest();

                request.AddQueryParameter("client_id", client_id);
                request.AddQueryParameter("client_secret", client_secret);
                request.AddQueryParameter("code", code);
                request.AddQueryParameter("grant_type", "authorization_code");
                request.AddQueryParameter("redirect_uri", "https://localhost:7203/oauth/callback");

                var response = restClient.Post(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    System.IO.File.WriteAllText(tokensFile, response.Content);
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            return View("Error");
        }
        private string GetUserEmail()
        {

            var contentRootPath = _hostingEnvironment.ContentRootPath;
            var filePath = Path.Combine(contentRootPath, "files", "tokens.json");
            var IDfilePath = Path.Combine(contentRootPath, "files", "Idtoken.json");
            var tokensFile = filePath;
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokensFile));
            var idToken = tokens["id_token"]?.ToString();

            RestClient restClient = new RestClient("https://oauth2.googleapis.com/tokeninfo");
            RestRequest request = new RestRequest();
            request.AddQueryParameter("id_token", idToken);
            var response = restClient.Post(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.File.WriteAllText(IDfilePath, response.Content);
               
                // Parse the response to get the email address
                var userEmail = JObject.Parse(response.Content)["email"]?.ToString();

                return userEmail;
            }
            return "";
        }



        [HttpPost("{userName}/AddUser")]
        public IActionResult AddUser(string userEmail)
        {
            // check if user exists
            var existingUser = _context.User.FirstOrDefault(u => u.Email == userEmail);

            if (existingUser != null)
            {
                return Ok(existingUser);
            }
            else
            {
                var newUser = new User { Username = "user1", Email = userEmail };
                _context.User.Add(newUser);
                _context.SaveChanges();
                return Ok(newUser);
            }
        }




        public ActionResult RefreshToken()
        {

            var tokenFile = "C:\\Users\\annie\\Desktop\\PC\\SportMe\\SportMeApp\\files\\tokens.json";
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));
            var client_id = "378858678415-the1gfbovl66l9jbmobcufom12a5kche.apps.googleusercontent.com";
            var client_secret = "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";


            RestClient restClient = new RestClient("https://oauth2.googleapis.com/token");
            RestRequest request = new RestRequest();

            request.AddQueryParameter("client_id", client_id);
            request.AddQueryParameter("client_secret", client_secret);
            request.AddQueryParameter("grant_type", "refresh_token");
            request.AddQueryParameter("refresh_token", tokens["refresh_token"].ToString());

            var response = restClient.Post(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                JObject newTokens = JObject.Parse(response.Content);
                newTokens["refresh_token"] = tokens["refresh_token"].ToString();
                System.IO.File.WriteAllText(tokenFile, newTokens.ToString());
                return RedirectToAction("Index", "Home", new { status = "success" });
            }
            return View("Error");
        }

        public ActionResult RevokeToken()
        {
            var tokenFile = "C:\\Users\\annie\\Desktop\\PC\\SportMe\\SportMeApp\\files\\tokens.json";
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));

            RestClient restClient = new RestClient("https://oauth2.googleapis.com/revoke");
            RestRequest request = new RestRequest();

            request.AddQueryParameter("token", tokens["access_token"].ToString());

            var response = restClient.Post(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return RedirectToAction("Index", "Home", new { status = "success" });
            }

            return View("Error");
        }
    }
}
