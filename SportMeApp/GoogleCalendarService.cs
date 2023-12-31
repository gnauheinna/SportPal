﻿using Google.Apis.Auth.OAuth2;
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
     

        public static CalendarService GetCalendarService(string tokenFile)
        {
            //Setting up a googleCalendarService
            // EZ as 123
            // 1. reads user access token from file

            
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));
          
            // creates new tokenResponse instance
            var token = new TokenResponse
            {
                AccessToken = tokens["access_token"].ToString(),
                ExpiresInSeconds = (long)tokens["expires_in"], // 1 hour expiration
                RefreshToken = tokens["refresh_token"].ToString(),
                TokenType = "Bearer"
            };
            // 2. Initializes new user credential instance
            var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
              new GoogleAuthorizationCodeFlow.Initializer
              {     //initializes with our google calendar api secret keys
                  ClientSecrets = new ClientSecrets
                  {
                      ClientId = "378858678415-the1gfbovl66l9jbmobcufom12a5kche.apps.googleusercontent.com",
                      ClientSecret = "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"
                  }

              }), "user", token);

            // 3. Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = ApplicationName,
            });

            return service;
        }
    }
}
