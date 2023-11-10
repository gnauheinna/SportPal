using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PaypalTest.Clients
{
    public sealed class PaypalClient
    {
        //This sets the mode of the paypal api, sandbox(currently in use)
        //or live
        public string Mode { get; }
        
        public string ClientId { get; }
        public string ClientSecret { get; }
        //This is the base url that we use to send out for API calls.
        //We access different function of the API by adding the appropriate url fragment at the end of the base url
        public string BaseUrl = "https://api-m.sandbox.paypal.com";

        public PaypalClient(string clientId, string clientSecret, string mode)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Mode = mode;
        }
        //Paypal REST API use OAuth 2.0 access token to authenticate requests
        //Here we send out an authentication request, and receive a response
        //Documentation: https://developer.paypal.com/api/rest/authentication/
        private async Task<AuthResponse> Authenticate()
        {
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"));

            var content = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials")
            };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{BaseUrl}/v1/oauth2/token"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "Authorization", $"Basic {auth}" }
                },
                Content = new FormUrlEncodedContent(content)
            };

            var httpClient = new HttpClient();
            //Send out the request
            var httpResponse = await httpClient.SendAsync(request);
            //receive response
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            //Deserialize the response from json to an AuthResponse we defined
            var response = JsonSerializer.Deserialize<AuthResponse>(jsonResponse);

            return response;
        }
        //Create an order
        //Documentation: https://developer.paypal.com/docs/api/orders/v2/
        public async Task<CreateOrderResponse> CreateOrder(string value, string currency, string reference, string payee = "sb-noqau27840329@personal.example.com")
        {
            
            var auth = await Authenticate();

            var request = new CreateOrderRequest
            {
                intent = "CAPTURE",
                purchase_units = new List<PurchaseUnit>
                {
                    new()
                    {
                        reference_id = reference,
                        amount = new Amount
                        {
                            currency_code = currency,
                            value = value
                        },
                        payee = new Payee
                        {
                            email_address = payee

                        }

                    }
                }
            };

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {auth.access_token}");
            //URL for order checkout
            //full url https://api-m.sandbox.paypal.com/v2/checkout/orders
            var httpResponse = await httpClient.PostAsJsonAsync($"{BaseUrl}/v2/checkout/orders", request);
            //same step as authenticate(), receive a response in json then convert json to CreateOrderResponse object
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<CreateOrderResponse>(jsonResponse);

            return response;
        }
        //Capture the payment for the order
        //To successfully capture payment for an order, the buyer must first approve the order or a valid payment source
        //Full documentation: https://developer.paypal.com/docs/api/orders/v2/#orders_capture
        public async Task<CaptureOrderResponse> CaptureOrder(string orderId)
        {
            var auth = await Authenticate();

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {auth.access_token}");

            var httpContent = new StringContent("", Encoding.Default, "application/json");
            //send out an request
            var httpResponse = await httpClient.PostAsync($"{BaseUrl}/v2/checkout/orders/{orderId}/capture", httpContent);
            //receive a response
            var jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            //Deserialize json into CaptureOrderResponse object
            var response = JsonSerializer.Deserialize<CaptureOrderResponse>(jsonResponse);

            return response;
        }
    }
    //Store deserialized information from the json response
    public sealed class AuthResponse
    {
        public string scope { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string app_id { get; set; }
        public int expires_in { get; set; }
        public string nonce { get; set; }
    }
    
    public sealed class CreateOrderRequest
    {
        public string intent { get; set; }
        public List<PurchaseUnit> purchase_units { get; set; } = new();
    }
    //Store deserialized information from the json response
    public sealed class CreateOrderResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public List<Link> links { get; set; }
    }
    //Store deserialized information from the json response
    public sealed class CaptureOrderResponse
    {
        public string id { get; set; }
        public string status { get; set; }
        public PaymentSource payment_source { get; set; }
        public List<PurchaseUnit> purchase_units { get; set; }
        public Payer payer { get; set; }
        public List<Link> links { get; set; }
    }
    //used to create order request
    public sealed class PurchaseUnit
    {
        public Amount amount { get; set; }
        public string reference_id { get; set; }
        public Shipping shipping { get; set; }
        public Payments payments { get; set; }
        public Payee payee {  get; set; }
     }

    public sealed class Payments
    {
        public List<Capture> captures { get; set; }
    }

    public sealed class Shipping
    {
        public Address address { get; set; }
    }

    public class Capture
    {
        public string id { get; set; }
        public string status { get; set; }
        public Amount amount { get; set; }
        public SellerProtection seller_protection { get; set; }
        public bool final_capture { get; set; }
        public string disbursement_mode { get; set; }
        public SellerReceivableBreakdown seller_receivable_breakdown { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public List<Link> links { get; set; }
    }

    public class Amount
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Payee
    {
        public string email_address { get; set; }
    }

    public sealed class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string method { get; set; }
    }

    public sealed class Name
    {
        public string given_name { get; set; }
        public string surname { get; set; }
    }

    public sealed class SellerProtection
    {
        public string status { get; set; }
        public List<string> dispute_categories { get; set; }
    }

    public sealed class SellerReceivableBreakdown
    {
        public Amount gross_amount { get; set; }
        public PaypalFee paypal_fee { get; set; }
        public Amount net_amount { get; set; }
    }

    public sealed class Paypal
    {
        public Name name { get; set; }
        public string email_address { get; set; }
        public string account_id { get; set; }
    }

    public sealed class PaypalFee
    {
        public string currency_code { get; set; }
        public string value { get; set; }
    }

    public class Address
    {
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public string admin_area_2 { get; set; }
        public string admin_area_1 { get; set; }
        public string postal_code { get; set; }
        public string country_code { get; set; }
    }

    public sealed class Payer
    {
        public Name name { get; set; }
        public string email_address { get; set; }
        public string payer_id { get; set; }
    }

    public sealed class PaymentSource
    {
        public Paypal paypal { get; set; }
    }
}
