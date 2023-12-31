﻿using SportMeApp.Models;
using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;


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
        // funtion to redirect to Google Oauth EZ as 12
        public ActionResult OauthRedirect()
        {
            // 1. client id of our google cloud project 
            var client_id = "378858678415-the1gfbovl66l9jbmobcufom12a5kche.apps.googleusercontent.com";
            // 2. redirect url for Oauth
            var redirectUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                "scope=https://www.googleapis.com/auth/calendar+https://www.googleapis.com/auth/calendar.events+https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/userinfo.profile" +
                "&access_type=offline&" +
                "include_granted_scopes=true&" +
                "response_type=code&" +
                "state=there&" +
                "redirect_uri=https://localhost:7203/oauth/callback&" +
                "client_id=" + client_id;
            
            return Redirect(redirectUrl);
        }
        public IActionResult SignOut()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                GoogleOpenIdConnectDefaults.AuthenticationScheme);
        }

    }
}