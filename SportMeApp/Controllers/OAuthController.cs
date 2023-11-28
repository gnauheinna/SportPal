using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace SportMeApp.Controllers
{

    public class OAuthController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;


        public OAuthController(IWebHostEnvironment hostingEnvironment)
        {
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
                this.GetTokens(code);

                // Redirect to the "Index" action
                return RedirectToAction("Index","Home");
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
