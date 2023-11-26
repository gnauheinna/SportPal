using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;


namespace SportMeApp.Services
{
    public class GoogleCalendarService
    {
        private static string[] Scopes = { CalendarService.Scope.Calendar };
        private static string ApplicationName = "Calendar";

        public static CalendarService GetCalendarService()
        {

            var tokenFile = "|DataDirectory|files\\tokens.json";
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));

            var token = new TokenResponse
            {
                AccessToken = tokens["access_token"].ToString(),
                ExpiresInSeconds = (long)tokens["expires_in"], // 1 hour expiration
                RefreshToken = tokens["refresh_token"].ToString(),
                TokenType = "Bearer"
            };

            var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
              new GoogleAuthorizationCodeFlow.Initializer
              {
                  ClientSecrets = new ClientSecrets
                  {
                      ClientId = "378858678415-the1gfbovl66l9jbmobcufom12a5kche.apps.googleusercontent.com",
                      ClientSecret = "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"
                  }

              }), "user", token);

            // using (var stream =
            //   new FileStream("C:\\Users\\annie\\Desktop\\ASPNetCoreCalendar-20231110T170912Z-001\\ASPNetCoreCalendar\\ASPNetCoreCalendar\\client_secret_378858678415-the1gfbovl66l9jbmobcufom12a5kche.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
            //{
            //  string credPath = "token.json";
            //credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            //  GoogleClientSecrets.Load(stream).Secrets,
            // Scopes,
            //  "user",
            //  CancellationToken.None,
            //        new FileDataStore(credPath, true)).Result;
            //     Console.WriteLine($"Credential file saved to: {credPath}");
            // }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });

            return service;
        }
    }
}
