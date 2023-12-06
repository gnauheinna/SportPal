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
using static SportMeApp.Controllers.GoogleMap.GoogleMapController;

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
        // EZ 1: shows the maps using url 
        public async Task<IActionResult> Index()
        {
            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q=42.3601,-71.0589";
            ViewBag.Distance = 5;

            return View();
        }
        //helper functions to calcualte the distance, converting to them radians
        private double Deg2Rad(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        private double Rad2Deg(double radians)
        {
            return radians * (180.0 / Math.PI);
        }
        // 2. Search method is called
        [HttpPost]
        public async Task<IActionResult> Search(string courtType, string zipcode)
        {
            // 1. calls the zipcode function to convernt zipcode to lat and lng
            _logger.LogInformation("LOG:Starting search method. CourtType:({courtType},ZipCode: {zipcode})", courtType, zipcode);
            var location = await GetLocationFromZipcode(zipcode);
            if (location == null)
            {
                location = new Location { lat = 42.3601, lng = -71.0589 };
            }
            //2. shows the map based on the zip code 
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
            // 3. shows places under 5 miles distance 
            int distance = ViewBag.Distance != null ? (int)ViewBag.Distance : 5;
            var places = await GetPlacesNearby(location, queryString, distance);

            //4.  Calculate distances for places
            places = CalculateDistances(location, places);

            // 5. Order places by distance
            places = places.OrderBy(loc => loc.Distance).ToList();
            foreach (var place in places)
            {
                SetCourtTypeFlags(place, courtType);
            }

            // 6. added to the databse 
            await SaveLocationsAsync(places);
            ViewData["CourtType"] = courtType;

            ViewBag.Places = places;
            return View("Index");
        }


        // set court type for either false or true based on the query
        private void SetCourtTypeFlags(Location location, string courtType)
        {
            //set them false in starting
            location.IsTennis = false;
            location.IsBasketball = false;
            location.IsVolleyball = false;
            // checks what court type it is and then changes it true in the databse 
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

            }
        }
    
        private async Task SaveLocationsAsync(List<Location> locations)
        {
            _logger.LogInformation("LOG: check locations: {locations}", locations);
            foreach (var location in locations)
            {
                // Check for an existing location using PlaceId 

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
        // if the location is already in the databse and if information that needs to 
        // it will be updated here 
        private void UpdateLocation(Locations existingLocation, Location newLocation)
        {
            // if a place is for tennis and baseball, it will just change the field either true 
            // or false instead of adding it again
            existingLocation.Name = newLocation.Name;
            existingLocation.lat = newLocation.lat;
            existingLocation.lng = newLocation.lng;
            //existingLocation.Distance = newLocation.Distance;
            existingLocation.FormattedPhoneNumber = newLocation.FormattedPhoneNumber;
            existingLocation.Rating = newLocation.Rating;
            existingLocation.ImageUrl = newLocation.ImageUrl;
            existingLocation.Address = newLocation.FormattedAddress;
            existingLocation.WeekdayText = string.Join("?", newLocation.OpeningHours?.weekday_text?.DefaultIfEmpty() ?? Array.Empty<string>());

            existingLocation.IsTennis |= newLocation.IsTennis;
            existingLocation.IsBaseball |= newLocation.IsBaseball;
            existingLocation.IsBasketball |= newLocation.IsBasketball;
            existingLocation.IsVolleyball |= newLocation.IsVolleyball;
            existingLocation.IsSoccer |= newLocation.IsSoccer;

        }

        // This function  adds the data front end to the database
        private async Task addLocation(Location newLocation)
        {  
            // for each location it adds the all the attributes from the google places api to the databse 
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
                PlaceId = newLocation.PlaceId,
                WeekdayText = string.Join("?", newLocation.OpeningHours?.weekday_text?.DefaultIfEmpty() ?? Array.Empty<string>())


            };
            // it is being added here
            _context.Locations.Add(location);
            
            await _context.SaveChangesAsync();

        }

        // using harversian method that was insipired by stackoverflow and google research
        // I believe their is google javascript function but I tried to use c sharp
        // the margin of error is pretyy low 
        private List<Location> CalculateDistances(Location origin, List<Location> locations)
        {
            foreach (var location in locations)
            {
                // gets user location lat and lng, and the court found 
                double lat1 = origin.lat;
                double lon1 = origin.lng;
                double lat2 = location.lat;
                double lon2 = location.lng;
                //Then here it being calulcated
                double theta = lon1 - lon2;
                double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
                dist = Math.Acos(dist);
                dist = Rad2Deg(dist);
                dist = dist * 60 * 1.1515;
                // the result is being saved in the distance 
                location.Distance = Math.Round(dist, 3);
            }

            return locations;
        }

        // by default  tennis court is showed if the user don't pick anything
        // and place in 5 miles are showed 
        // after the user input is convernted to lat and lng from getlocation fromZipCode
        public async Task<IActionResult> SearchByLocation(double lat, double lng)
        {
            string queryString = "Tennis Court";
            int distance = 5;
            // the ge t places nearby method is called and based on type of code distance and the user lat and lng all the courts
            //are showed
            var places = await GetPlacesNearby(new Location { lat = lat, lng = lng }, queryString, distance);
            _logger.LogInformation("LOG: fetch places: {places}", places);
            //these places are then showed 
            ViewBag.Places = places;
           //user zipcode location is showed
            ViewBag.MapUrl = $"https://www.google.com/maps/embed/v1/place?key=AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU&q={lat},{lng}"; // Replace with your API key
            return View("Index");
        }
        // Here the zipcode is convertedto actual lat and lat from the Google API 
        // synchronous method 
        private async Task<Location> GetLocationFromZipcode(string zipcode)
        {
            try
            {
                // instance of HttpClient to send HTTP requests.     
                using (HttpClient client = new HttpClient())
                {
                    // API key and request URL for the Google Geocoding API.
                    string apiKey = "AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU";
                    string requestUrl = $"https://maps.googleapis.com/maps/api/geocode/json?key={apiKey}&components=postal_code:{zipcode}";
                    // asynchronous request to the API and await the response as a string
                    var response = await client.GetStringAsync(requestUrl);
                    // Deserialize the JSON response into a GeocodeResponse object.
                    var geocodeResponse = JsonConvert.DeserializeObject<GeocodeResponse>(response);
                   //here we checking if it valid and there is actual response 
                    if (geocodeResponse.status == "OK" && geocodeResponse.results.Any())
                    {
                       //Here a new object is being returned based on the response 
                        _logger.LogInformation("LOG:geocodeResponse.status: {status}", geocodeResponse.results);
                        return new Location
                        {
                            lat = geocodeResponse.results[0].geometry.location.lat,
                            lng = geocodeResponse.results[0].geometry.location.lng
                        };
                    }
                    //if it is not correct then an error is being showed 
                    else
                    {
                        _logger.LogError("LOG:Geocode response status not OK or no results, Status:{Status}, zipcode: {zipcode}", geocodeResponse.status, zipcode);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                // we log any other expection that might occured 
                _logger.LogError(ex, "LOG:error getting location from zip code: {zipcode}", zipcode);
                return null;
            }
        }
        // error cases and log are implemented to look for any errors 

        // this function actually get infromation of the api by call getplace details 
        // for all the places that around the radius
        private async Task<List<Location>> GetPlacesNearby(Location center, string query, int distance)
        {
            List<Location> locations = new List<Location>();

            using (HttpClient client = new HttpClient())
            {
                //http again request being sent 
                string apiKey = "AIzaSyDf0RqSbMr-WJVk8LF_D1Hnhucbr4t8HMU";
                string placesRequest = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={center.lat},{center.lng}&radius={distance * 1609.34}&keyword={query}&key={apiKey}";
                string response = await client.GetStringAsync(placesRequest);
                var placesResponse = JsonConvert.DeserializeObject<PlacesApiQueryResponse>(response);
                // It deserlizaed and convernted to the google classess that are defined 
                foreach (var result in placesResponse.results)
                {
                    
                    var placeDetails = await GetPlaceDetails(result.place_id, apiKey);
                    var location = new Location
                    {
                        /// all the information is being fetched and being stored here 
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
        //using the api we get all the details for each place using the palce ID
        private async Task<Result> GetPlaceDetails(string placeId, string apiKey)
        {
            using (HttpClient client = new HttpClient())
            {
                // sends an asynchronous GET request to the Google Places API using the constructed URL.
                string placeDetailsRequest = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&fields=name,formatted_address,formatted_phone_number,opening_hours,rating,photos&key={apiKey}";
                string response = await client.GetStringAsync(placeDetailsRequest);
                //JSON response from the API is deserialized into a PlaceDetailsResponse object. 
                var placeDetailsResponse = JsonConvert.DeserializeObject<PlaceDetailsResponse>(response);
                //result property of the PlaceDetailsResponse object, which contains the detailed information about the place.
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
        // this for the getting the picture
        public string GetPhotoUrl(string photoReference, string apiKey)
        {
            return $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference={photoReference}&key={apiKey}";
        }

        // The next three classess are for  zipcode and search. 
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
        // This for looking for place
        class GooglePlacesResponse
        {
            public List<object> html_attributions { get; set; } //this by default when use google api
            public List<Result> results { get; set; }
            public string status { get; set; } //this is also by default 
        }
        // this get all the result from the databse 
        class PlaceDetailsResponse
        {
            public string status { get; set; }
            public Result result { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
        }
        // this how we storing them 
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
        // this important for the database, the fields that need 
        // to be the databse 
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
            public string ImageUrl { get; set; }
            public string Address { get; set; }
            public bool IsVolleyball { get; set; }
            public bool IsTennis { get; set; }
            public bool IsBasketball { get; set; }
            public bool IsSoccer { get; set; }
            public bool IsBaseball { get; set; }
            public List<string> WeekdayText { get; set; }

        }



        // This for the hours 
        public class GooglePlaceApiOpeningHours
        {

            public List<string> weekday_text { get; set; }
        }
        // for getting respones 
        public class PlacesApiQueryResponse
        {
            public List<object> html_attributions { get; set; }
            public List<Result> results { get; set; }
            public string status { get; set; }
        }
        // this for the photo 
        public class Photo
        {
            public string photo_reference { get; set; }
        }
    }
}
