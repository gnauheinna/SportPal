using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using RestSharp;
using NuGet.Common;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using SportMeApp.Models;
using SportMeApp.Clients;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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
                // User Login EZ as 1234
                // 1. Process the authorization code
                var tokens = GetTokens(code);
                // 2. Retrieve the user's email address
                var user = GetUserEmail();
          

                // 3. save user email in session storage
                HttpContext.Session.SetString("UserEmailSession", user[1]);
               this.AddUser(user);
                
                // 4. Redirect to the "Index" action
                return RedirectToAction("Index","GoogleMap");
            }
            else
            {
                // Handle the case where an error is present
                return View("Error");
            }
        }
        // funtion to get User session tokens from code returned from Oauth Ez as 123
        public ActionResult GetTokens(string code)
        {
            try
            {
                // 1. initializes variables
                var contentRootPath = _hostingEnvironment.ContentRootPath;
                var filePath = Path.Combine(contentRootPath, "files", "tokens.json");
                var tokensFile = filePath;
                var client_id = "378858678415-the1gfbovl66l9jbmobcufom12a5kche.apps.googleusercontent.com";
                var client_secret = "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";

                // 2. initialize restClient instance
                RestClient restClient = new RestClient("https://oauth2.googleapis.com/token");
                RestRequest request = new RestRequest();
                // 3. add appropreate parameters
                request.AddQueryParameter("client_id", client_id);
                request.AddQueryParameter("client_secret", client_secret);
                request.AddQueryParameter("code", code);
                request.AddQueryParameter("grant_type", "authorization_code");
                request.AddQueryParameter("redirect_uri", "https://localhost:7203/oauth/callback");
                // 4. gets response from HTTP post
                var response = restClient.Post(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    // 5. writes response to tokens.JSON
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
        // function to get userEmail EZ as 123
        private string[] GetUserEmail()
        {
            // 1. initializes variables
            var contentRootPath = _hostingEnvironment.ContentRootPath;
            var filePath = Path.Combine(contentRootPath, "files", "tokens.json");
            var IDfilePath = Path.Combine(contentRootPath, "files", "Idtoken.json");
            var tokensFile = filePath;
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokensFile));
            var idToken = tokens["id_token"]?.ToString();
            string[] user = new string[2];
            // 2. initialzes restClient instance
            RestClient restClient = new RestClient("https://oauth2.googleapis.com/tokeninfo");
            RestRequest request = new RestRequest();

            // 3. add appropriate params to the query
            request.AddQueryParameter("id_token", idToken);
            // 4. gets response from  HTTP post
            var response = restClient.Post(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.File.WriteAllText(IDfilePath, response.Content);


                // 5. Parse the response to get the email address and name
                var userEmail = JObject.Parse(response.Content)["email"]?.ToString();
                user[1] = JObject.Parse(response.Content)["email"]?.ToString();
                user[0] = JObject.Parse(response.Content)["name"]?.ToString();

                return user;
            }
            return user;
        }


        // function to add User to database EZ as 123
        [HttpPost("{userName}/{userEmail}/AddUser")]
        public IActionResult AddUser(string[] user)
        {
            // 1. check if user exists based on userEmail
            var existingUser = _context.User.FirstOrDefault(u => u.Email == user[1]);
            
            if (existingUser != null)
            {
                // 2a. if user exists return ok
                return Ok(existingUser);
            }
            else
            {
                // 2b. if user doesn't exists add new user to database
                var newUser = new User { Username = user[0], Email = user[1] };
                _context.User.Add(newUser);
                _context.SaveChanges();
               
                return Ok(newUser);
            }
        }



        // function to refresh token EZ as 123
        public ActionResult RefreshToken()
        {
            // 1. initialize variables
            var tokenFile = "C:\\Users\\annie\\Desktop\\PC\\SportMe\\SportMeApp\\files\\tokens.json";
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));
            var client_id = "378858678415-the1gfbovl66l9jbmobcufom12a5kche.apps.googleusercontent.com";
            var client_secret = "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";

            // 2. initialize RestClient
            RestClient restClient = new RestClient("https://oauth2.googleapis.com/token");
            RestRequest request = new RestRequest();

            // 3. add appropriate Query Parameters
            request.AddQueryParameter("client_id", client_id);
            request.AddQueryParameter("client_secret", client_secret);
            request.AddQueryParameter("grant_type", "refresh_token");
            request.AddQueryParameter("refresh_token", tokens["refresh_token"].ToString());
            // 4. gets respons from HTTP post
            var response = restClient.Post(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // 5. parses response and updates the refresh token in tokens file
                JObject newTokens = JObject.Parse(response.Content);
                newTokens["refresh_token"] = tokens["refresh_token"].ToString();
                System.IO.File.WriteAllText(tokenFile, newTokens.ToString());
                // 6. redirects to index
                return RedirectToAction("Index", "Home", new { status = "success" });
            }
            return View("Error");
        }
        // function to revoke user token EZ as 123
        public ActionResult RevokeToken()
        {
            // 1. initialize variables
            var tokenFile = "C:\\Users\\annie\\Desktop\\PC\\SportMe\\SportMeApp\\files\\tokens.json";
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));
            // 2. initialize RestClient
            RestClient restClient = new RestClient("https://oauth2.googleapis.com/revoke");
            RestRequest request = new RestRequest();
            // 3. add appropriate Query Parameters
            request.AddQueryParameter("token", tokens["access_token"].ToString());
            // 4. gets respons from HTTP post
            var response = restClient.Post(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // 5. redirects to index
                return RedirectToAction("Index", "Home", new { status = "success" });
            }

            return View("Error");
        }
    }
}
