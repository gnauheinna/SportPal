using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GoogleMapss.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q=40.730610,-73.935242";
            ViewBag.Distance = 5;
            return View();
        }

        private double Deg2Rad(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        private double Rad2Deg(double radians)
        {
            return radians * (180.0 / Math.PI);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string courtType, string zipcode)
        {
            var location = await GetLocationFromZipcode(zipcode);
            if (location == null)
            {
                location = new Location { lat = 40.730610, lng = -73.935242 };
            }

            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q={location.lat},{location.lng}";
            string queryString = courtType switch
            {
                "tennis" => "Tennis Court",
                "basketball" => "Basketball Court",
                "volleyball" => "Volleyball Court",
                "baseball" => "Baseball Field",
                "soccer" => "Soccer Field",
                _ => "Park"
            };
            int distance = ViewBag.Distance != null ? (int)ViewBag.Distance : 5;
            var places = await GetPlacesNearby(location, queryString, distance);

            // Calculate distances for places
            places = CalculateDistances(location, places);

            // Order places by distance
            places = places.OrderBy(loc => loc.Distance).ToList();

            ViewBag.Places = places;
            return View("Index");
        }

        private List<Location> CalculateDistances(Location origin, List<Location> locations)
        {
            foreach (var location in locations)
            {
                double lat1 = origin.lat;
                double lon1 = origin.lng;
                double lat2 = location.lat;
                double lon2 = location.lng;

                double theta = lon1 - lon2;
                double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
                dist = Math.Acos(dist);
                dist = Rad2Deg(dist);
                dist = dist * 60 * 1.1515;

                location.Distance = dist;
            }

            return locations;
        }

        public async Task<IActionResult> SearchByLocation(double lat, double lng)
        {
            string queryString = "Tennis Court";
            int distance = 5;
            var places = await GetPlacesNearby(new Location { lat = lat, lng = lng }, queryString, distance);
            ViewBag.Places = places;
            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q={lat},{lng}";
            return View("Index");
        }

        private async Task<Location> GetLocationFromZipcode(string zipcode)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiKey = "AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU";
                string requestUrl = $"https://maps.googleapis.com/maps/api/geocode/json?key={apiKey}&components=postal_code:{zipcode}";
                var response = await client.GetStringAsync(requestUrl);
                var geocodeResponse = JsonSerializer.Deserialize<GeocodeResponse>(response);
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
                    return null;
                }
            }
        }

        private async Task<List<Location>> GetPlacesNearby(Location center, string query, int distance)
        {
            List<Location> locations = new List<Location>();

            using (HttpClient client = new HttpClient())
            {
                string apiKey = "AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU"; // Replace with your Google Places API key
                string placesRequest = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={center.lat},{center.lng}&radius={distance * 1609.34}&keyword={query}&key={apiKey}";
                string response = await client.GetStringAsync(placesRequest);
                var placesResponse = JsonConvert.DeserializeObject<PlacesApiQueryResponse>(response);

                // Your existing code to add locations
                foreach (var result in placesResponse.results)
                {
                    locations.Add(new Location
                    {
                        lat = result.geometry.location.lat,
                        lng = result.geometry.location.lng,
                        Name = result.name,
                        PlaceId = result.place_id
                    });
                }

                // Fetch additional details (phone number, hours, rating) for each location
                foreach (var location in locations)
                {
                    var placeDetails = await GetPlaceDetails(location.PlaceId, apiKey);
                    if (placeDetails != null)
                    {
                        location.FormattedPhoneNumber = placeDetails.formatted_phone_number;
                        location.OpeningHours = placeDetails.opening_hours;
                        location.Rating = placeDetails.rating;
                    }
                }
            }

            return locations;
        }


        private async Task<Result> GetPlaceDetails(string placeId, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                string placeDetailsRequest = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&fields=name,formatted_phone_number,opening_hours,rating&key={apiKey}";
                string response = await client.GetStringAsync(placeDetailsRequest);
                var placeDetailsResponse = JsonSerializer.Deserialize<PlaceDetailsResponse>(response);
                if (placeDetailsResponse.status == "OK")
                {
                    return placeDetailsResponse.result;
                }
                else
                {
                    return null;
                }
            }
        }

        class GeocodeResponse
        {
            public string status { get; set; }
            public GeocodeResult[] results { get; set; }
        }

        class GeocodeResult
        {
            public GeocodeGeometry geometry { get; set; }
        }

        class GeocodeGeometry
        {
            public Location location { get; set; }
        }

        class GooglePlacesResponse
        {
            public List<object> html_attributions { get; set; }
            public List<Result> results { get; set; }
            public string status { get; set; }
        }

        class PlaceDetailsResponse
        {
            public string status { get; set; }
            public Result result { get; set; }
        }
        public class Geometry
        {
            public Location location { get; set; }
        }

        public class OpeningHours
        {
            public bool open_now { get; set; }
            public List<object> weekday_text { get; set; }
        }

        public class Result
        {
            public Geometry geometry { get; set; }
            public string name { get; set; }
            public string formatted_phone_number { get; set; }
            public OpeningHours opening_hours { get; set; }
            public double rating { get; set; }
            public string place_id { get; set; }
        }

        public class Location
        {
            public string Name { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
            public double Distance { get; set; }
            public string PlaceId { get; set; }
            public string FormattedPhoneNumber { get; set; }
            public OpeningHours OpeningHours { get; set; }
            public double Rating { get; set; }
        }

        public class PlacesApiQueryResponse
        {
            public List<object> html_attributions { get; set; }
            public List<Result> results { get; set; }
            public string status { get; set; }
        }



    }
}
