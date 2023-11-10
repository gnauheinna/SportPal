using Microsoft.AspNetCore.Mvc;
using ASPNetCoreCalendar.Models;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;


namespace ASPNetCoreCalendar.Controllers
{
    public class CalendarEventController : Controller
    {
        [HttpPost]
        public ActionResult CreateEvent(Event calendarEvent)
        {
            var tokenFile = "C:\\Users\\annie\\Downloads\\ASPNetCoreCalendar-20231110T170912Z-001\\ASPNetCoreCalendar\\ASPNetCoreCalendar\\Files\\tokens.json";
            var tokens = JObject.Parse(System.IO.File.ReadAllText(tokenFile));
         
            RestClient restClient = new RestClient("https://www.googleapis.com/calendar/v3/calendars/bc7495931fde2bcd23662e0c4b828c317cd7667a38bd28caec5f45b33da3a313@group.calendar.google.com/events?key=AIzaSyDL3bVkU9hIabTo2wiRqSv8rrA26f__PIk");
            RestRequest request = new RestRequest();
     

            calendarEvent.Start.DateTime = DateTime.Parse(calendarEvent.Start.DateTime).ToString("yyyy-MM-dd'T'HH:mm:ss.fffk");
            calendarEvent.End.DateTime = DateTime.Parse(calendarEvent.End.DateTime).ToString("yyyy-MM-dd'T'HH:mm:ss.fffk");

            var model = JsonConvert.SerializeObject(calendarEvent, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            request.AddHeader("Authorization", "Bearer " + tokens["access_token"]);
            request.AddParameter("application/json", model, ParameterType.RequestBody);

            var response = restClient.Post(request);

            Console.WriteLine($"Response Content: {response.Content}");


            if (response.StatusCode == System.Net.HttpStatusCode.OK) 
            {
                return RedirectToAction("Index", "Home", new { status = "success" });
            }

            return View("Error");

        }
        public IActionResult Index()
        {
            return View();
        }

       
    }
}
 