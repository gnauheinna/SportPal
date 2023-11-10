namespace GoogleMapss.Models
{
    // this has the structure of the response 
    public class GooglePlacesResponse
    { 
        // this shows the array by the 
        //google places api
        public Result[] results { get; set; }
    }
    // this will have results from
    // the google places api

    public class Result
    {

        // this shows lat and long of the place
        public PlaceGeometry geometry { get; set; }
        public string name { get; set; }
        
    }

    public class PlaceGeometry
    {
        // this shows the name and title 
        //of the place
        public Location location { get; set; }
    }

    // this shows exact location lat and lng and the name
    public class Location
    {
        // the name is also found here
        public string Name { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
