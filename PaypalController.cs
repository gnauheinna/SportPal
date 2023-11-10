using Microsoft.AspNetCore.Mvc;
using PaypalTest.Clients;


namespace PaypalTest.Controllers
{
    public class PaypalController : Controller
    {
        private readonly PaypalClient _paypalClient;

        public PaypalController(PaypalClient paypalClient)
        {
            this._paypalClient = paypalClient;
        }

        public IActionResult Index()
        {
            // ViewBag.ClientId is used to get the Paypal Checkout javascript SDK
            ViewBag.ClientId = _paypalClient.ClientId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Order(CancellationToken cancellationToken)
        {
            try
            {
                // set the transaction price and currency
                var price = "1240.25";
                var currency = "USD";

                // "reference" is the transaction key. In the backend it show
                var reference = "ThisIsMyTransactionReference";
                //Send out order request
                var response = await _paypalClient.CreateOrder(price, currency, reference);

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

        public async Task<IActionResult> Capture(string orderId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderId);

                var reference = response.purchase_units[0].reference_id;

               

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}