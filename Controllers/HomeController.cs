using GoogleMapss.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


// the getlocation by nearby was inspired by stackover flow the code was changed 
// to work for this assingment
// the format of the code was inspired by youtbe on razor and google map api
namespace GoogleMapss.Controllers
{
    public class HomeController : Controller
    {

        // to check if the zipcode works instead of choosing boston as my default location,
       // I used google api and set newywork as default, and then I also set 5 miles that means
       // I used 5 miles, so everytime user puts their zipcode all the courts that are under 5 miles will show up
       // intially, I had the geolocation and then just calculated the distance, but in class we figure out that 
       //geo location does not work with windows, so I have been trying to figure out how to calculated 
       // the distance.so for I will be using zipcode.but I hope to meet you with you guys and discuss if I can use
       //another  approach I know other groups are working on it too. 
        public async Task<IActionResult> Index()
        {
            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q=40.730610,-73.935242"; 
            ViewBag.Distance = 5;
            // the view shows a default place which of new york.
            return View();
        }

        [HttpPost]
        [HttpPost]
        [HttpPost]

        // I used post method to handel user response 
        public async Task<IActionResult> Search(string courtType, string zipcode)
        {
            // here I am storing the location usinge GeoLocationFrom Zipocde function
            var location = await GetLocationFromZipcode(zipcode);
            if (location == null)
            {
                // After running my code several times.I decided I need default place because 
                // Handle error. For this example, we'll default back to New York.
                location = new Location { lat = 40.730610, lng = -73.935242 };
            }

            //  the map will nowdisplay the area of the provided zipcode, here I am using the api
            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q={location.lat},{location.lng}";
            // here we decided either to show tennis court or basketball court. for future I need to add 
            //more opitions
            var queryString = courtType == "tennis" ? "Tennis Court" : "Basketball Court";
            // default distance will be 
            int distance = ViewBag.Distance != null ? (int)ViewBag.Distance : 5;
            // GetPlacNearby finds the places nearby 
            var places = await GetPlacesNearby(location, queryString, distance);
           //and  all the places are then populated on the left side
            ViewBag.Places = places;
            // finally, here code new calculated information will show up
            return View("Index");
        }

        // this method will get the location for given zipcode, I tried to add distance
        //but the distance is not working correctly
        private async Task<Location> GetLocationFromZipcode(string zipcode)
        {
            using (HttpClient client = new HttpClient())
            {
                // here put the apikey to use them these will be the request or the endpoints
                string apiKey = "AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU";
                string requestUrl = $"https://maps.googleapis.com/maps/api/geocode/json?key={apiKey}&components=postal_code:{zipcode}";
                // here the request is made, I used getstring because I was getting errors if it was 
                //not a string 
                var response = await client.GetStringAsync(requestUrl);
                //here the api was deserilzied for the model
                var geocodeResponse = JsonSerializer.Deserialize<GeocodeResponse>(response);
                // once their is valid zip code, we simply return it 
                if (geocodeResponse.status == "OK" && geocodeResponse.results.Any())
                {
                    return new Location
                    {
                        lat = geocodeResponse.results[0].geometry.location.lat,
                        lng = geocodeResponse.results[0].geometry.location.lng
                    };
                }
                else
                {
                    //by default we are not showing any locations
                    return null;
                }
            }
        }

        // this method is to get the nearby courts
        private async Task<List<Location>> GetPlacesNearby(Location center, string query, int distance)
        { 
            // list for location is created
            List<Location> locations = new List<Location>();

            using (HttpClient client = new HttpClient())
            {
                //api response to actual input 
                string placesRequest = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={center.lat},{center.lng}&radius={distance * 1609.34}&keyword={query}&key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU";
                string response = await client.GetStringAsync(placesRequest);
                //show location list ont the side of the website 
                var placesResponse = JsonSerializer.Deserialize<GooglePlacesResponse>(response);
                // a for loop to fill the list with the location that matches with user input
                foreach (var result in placesResponse.results)
                {
                    locations.Add(new Location
                    {
                        lat = result.geometry.location.lat,
                        lng = result.geometry.location.lng,
                        Name = result.name // This sets the name for the location
                    });
                }
            }

            return locations;
        }


        // this is the model class to for api response
        // I have setters and getters here
        class GeocodeResponse
        {
            public string status { get; set; }
            public GeocodeResult[] results { get; set; }
        }
        //this class is to hold the results
        class GeocodeResult
        {
            public GeocodeGeometry geometry { get; set; }
        }
        // here we getter and setter for the actual geocoding results 
        class GeocodeGeometry
        {
            public Location location { get; set; }
        }
    }
}

