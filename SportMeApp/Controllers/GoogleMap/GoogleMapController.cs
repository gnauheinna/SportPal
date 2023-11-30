using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SportMeApp.Models;
using System.Reflection.Emit;

namespace SportMeApp.Controllers.GoogleMap
{
    public class GoogleMapController : Controller
    {
        private readonly SportMeContext _context;
        private readonly ILogger<GoogleMapController> _logger;
        public GoogleMapController(SportMeContext context, ILogger<GoogleMapController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q=42.3601,-71.0589";
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
            _logger.LogInformation("LOG:Starting search method. CourtType:({courtType},ZipCode: {zipcode})", courtType, zipcode);
            var location = await GetLocationFromZipcode(zipcode);
            if (location == null)
            {
                location = new Location { lat = 42.3601, lng = -71.0589 };
            }

            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q={location.lat},{location.lng}"; 
            string queryString = courtType switch
            {
                "tennis" => "Tennis Court",
                "basketball" => "Basketball Court",
                "volleyball" => "Volleyball Court",
                "baseball" => "Baseball Field",
                "soccer" => "Soccer Field",
                _ => "tenniscourt"
            };
            int distance = ViewBag.Distance != null ? (int)ViewBag.Distance : 5;
            var places = await GetPlacesNearby(location, queryString, distance);

            // Calculate distances for places
            places = CalculateDistances(location, places);

            // Order places by distance
            places = places.OrderBy(loc => loc.Distance).ToList();
            foreach (var place in places)
            {
                SetCourtTypeFlags(place, courtType);
            }


            await SaveLocationsAsync(places);
            ViewData["CourtType"] = courtType;

            ViewBag.Places = places;
            return View("Index");
        }



        private void SetCourtTypeFlags(Location location, string courtType)
        {
            // Assuming courtType is a string like "tennis", "baseball", etc.
            // Reset all court type properties to false
            location.IsTennis = false;
            location.IsBasketball = false;
            location.IsVolleyball = false;
            // Include other court types as necessary

            // Set the appropriate property to true based on the courtType
            switch (courtType.ToLower())
            {
                case "tennis":
                    location.IsTennis = true;
                    break;
                case "basketball":
                    location.IsBasketball = true;
                    break;
                case "volleyball":
                    location.IsVolleyball = true;
                    break;
                    // Add additional cases as necessary for other court types
            }
        }

        private async Task SaveLocationsAsync(List<Location> locations)
        {
            _logger.LogInformation("LOG: check locations: {locations}", locations);
            foreach (var location in locations)
            {
                // Check for an existing location. Here, I'm using PlaceId as an example.
                // You might use Coordinates, Name, or a combination of properties,
                // depending on how you define uniqueness in your application.
                var existingLocation = await _context.Locations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.PlaceId == location.PlaceId);
                
                // if we have stored this location before
                if (existingLocation != null)
                {
                    // Update existing location's properties
                    UpdateLocation(existingLocation, location);
                    _context.Update(existingLocation);
                }
                else
                {
                    // New location, add it to the context
                    await addLocation(location);
                   
                }
            }
            await _context.SaveChangesAsync();
        }

        private void UpdateLocation(Locations existingLocation, Location newLocation)
        {
            // Update general properties
            existingLocation.Name = newLocation.Name;
            existingLocation.lat = newLocation.lat;
            existingLocation.lng = newLocation.lng;
            //existingLocation.Distance = newLocation.Distance;
            existingLocation.FormattedPhoneNumber = newLocation.FormattedPhoneNumber;
            existingLocation.Rating = newLocation.Rating;
            existingLocation.ImageUrl = newLocation.ImageUrl;
            existingLocation.Address = newLocation.FormattedAddress;
            existingLocation.WeekdayText = newLocation.OpeningHours?.weekday_text;

            existingLocation.IsTennis |= newLocation.IsTennis;
            existingLocation.IsBaseball |= newLocation.IsBaseball;
            existingLocation.IsBasketball |= newLocation.IsBasketball;
            existingLocation.IsVolleyball |= newLocation.IsVolleyball;
            existingLocation.IsSoccer |= newLocation.IsSoccer;

        }
        private async Task addLocation(Location newLocation)
        {
            var location = new Locations
            {
                Name = newLocation.Name,
                lat = newLocation.lat,
                lng = newLocation.lng,
                FormattedPhoneNumber = newLocation.FormattedPhoneNumber,
                Rating = newLocation.Rating,
                ImageUrl = newLocation.ImageUrl,
                IsTennis = newLocation.IsTennis,
                IsBaseball = newLocation.IsBaseball,
                IsBasketball = newLocation.IsBasketball,
                IsVolleyball = newLocation.IsVolleyball,
                IsSoccer = newLocation.IsSoccer,
                Address = newLocation.FormattedAddress,
                PlaceId= newLocation.PlaceId,
                WeekdayText = newLocation.OpeningHours?.weekday_text
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

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

                location.Distance = Math.Round(dist, 3);
            }

            return locations;
        }

        public async Task<IActionResult> SearchByLocation(double lat, double lng)
        {
            string queryString = "Tennis Court";
            int distance = 5;
            var places = await GetPlacesNearby(new Location { lat = lat, lng = lng }, queryString, distance);
            _logger.LogInformation("LOG: fetch places: {places}", places);
            ViewBag.Places = places;
            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q={lat},{lng}"; // Replace with your API key
            return View("Index");
        }

        private async Task<Location> GetLocationFromZipcode(string zipcode)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiKey = "AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU"; // Replace with your Google Maps API key
                    string requestUrl = $"https://maps.googleapis.com/maps/api/geocode/json?key={apiKey}&components=postal_code:{zipcode}";
                    var response = await client.GetStringAsync(requestUrl);
                    var geocodeResponse = JsonConvert.DeserializeObject<GeocodeResponse>(response);
                    if (geocodeResponse.status == "OK" && geocodeResponse.results.Any())
                    {
                        _logger.LogInformation("LOG:geocodeResponse.status: {status}", geocodeResponse.results);
                        return new Location
                        {
                            lat = geocodeResponse.results[0].geometry.location.lat,
                            lng = geocodeResponse.results[0].geometry.location.lng
                        };
                    }
                    else
                    {
                        _logger.LogError("LOG:Geocode response status not OK or no results, Status:{Status}, zipcode: {zipcode}", geocodeResponse.status, zipcode);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LOG:error getting location from zip code: {zipcode}", zipcode);
                return null;
            }
        }



        private async Task<List<Location>> GetPlacesNearby(Location center, string query, int distance)
 {
     List<Location> locations = new List<Location>();

     using (HttpClient client = new HttpClient())
     {
         string apiKey = "AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU";
         string placesRequest = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={center.lat},{center.lng}&radius={distance * 1609.34}&keyword={query}&key={apiKey}";
         string response = await client.GetStringAsync(placesRequest);
         var placesResponse = JsonConvert.DeserializeObject<PlacesApiQueryResponse>(response);

         foreach (var result in placesResponse.results)
         {
             var placeDetails = await GetPlaceDetails(result.place_id, apiKey);
             var location = new Location
             {
                 lat = result.geometry.location.lat,
                 lng = result.geometry.location.lng,
                 Name = result.name,
                 PlaceId = result.place_id,
                 FormattedAddress = placeDetails?.formatted_address,
                 FormattedPhoneNumber = placeDetails?.formatted_phone_number,
                 Rating = placeDetails?.rating ?? 0,
                 OpeningHours = placeDetails?.opening_hours,
                 ImageUrl = placeDetails.photos != null && placeDetails.photos.Any() ? GetPhotoUrl(placeDetails.photos.First().photo_reference, apiKey) : null
             };

             locations.Add(location);
             //add to the databse 
         }
     }

     return locations;
 }

         private async Task<Result> GetPlaceDetails(string placeId, string apiKey)
 {
     using (HttpClient client = new HttpClient())
     {
         string placeDetailsRequest = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&fields=name,formatted_address,formatted_phone_number,opening_hours,rating,photos&key={apiKey}";
         string response = await client.GetStringAsync(placeDetailsRequest);
         var placeDetailsResponse = JsonConvert.DeserializeObject<PlaceDetailsResponse>(response);
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

        public string GetPhotoUrl(string photoReference, string apiKey)
        {
            return $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference={photoReference}&key={apiKey}";
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

        public class Result
        {
            public Geometry geometry { get; set; }
            public string name { get; set; }
            public string formatted_address { get; set; }
            public string formatted_phone_number { get; set; }
            public string OpeningHours { get; set; }
            public double rating { get; set; }
            public string place_id { get; set; }
            public GooglePlaceApiOpeningHours opening_hours { get; set; }
            public List<Photo> photos { get; set; }
            public string FormattedAddress { get; set; }
        }

        public class Location
        {
            public string Name { get; set; }
            public double lat { get; set; }
            public double lng { get; set; }
            public double Distance { get; set; }
            public string PlaceId { get; set; }
            public string FormattedAddress { get; set; }
            public string FormattedPhoneNumber { get; set; }
            public GooglePlaceApiOpeningHours OpeningHours { get; set; }
            public double Rating { get; set; }
            public List<DailyHours> DailyOpeningHours { get; set; }
            public string ImageUrl { get; set; }
            public string Address { get; set; }
            public bool IsVolleyball { get; set; }
            public bool IsTennis { get; set; }
            public bool IsBasketball { get; set; }
            public bool IsSoccer { get; set; }
            public bool IsBaseball { get; set; }
            public List<string> WeekdayText { get; set; }

        }

        


        public class GooglePlaceApiOpeningHours
        {
            
            public List<string> weekday_text { get; set; } 
        }

        public class PlacesApiQueryResponse
        {
            public List<object> html_attributions { get; set; }
            public List<Result> results { get; set; }
            public string status { get; set; }
        }

        public class Photo
        {
            public string photo_reference { get; set; }
        }
    }
}
