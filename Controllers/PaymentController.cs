using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    [Route("[controller]")]
    public class PaymentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IServiceRepository _serviceRepository;
        private readonly IOrderRepository _orderRepository;


        public PaymentController(HttpClient _httpClient, IConfiguration _configuration, IServiceRepository _serviceRepository, IOrderRepository _orderRepository)
        {
            this._httpClient = _httpClient;
            this._configuration = _configuration;
            this._serviceRepository = _serviceRepository;
            this._orderRepository = _orderRepository;
        }

        [HttpPost]
        [Route("/test-payment")]
        public async Task<IActionResult> Pay([FromBody] TopUpPurchase purchase)
        {
            try
            {
                if (!await _serviceRepository.IsUserExistAsync(purchase.SteamId))
                    ModelState.AddModelError(nameof(purchase.SteamId), "Invalid SteamId!");
                if (purchase.Amount <= 0)
                    ModelState.AddModelError(nameof(purchase.Amount), "The sum must be above zero!");
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                    return BadRequest(errors);
                }
            }
            catch (Exception) { return BadRequest("Invalid json"); }


            var orderId = RandomString(10);
            var publicKey = _configuration["LiqPayPublicKey"];
            var privateKey = _configuration["LiqPayPrivateKey"];
            var requestData = new
            {
                public_key = publicKey,
                private_key = privateKey,
                version = 3,
                action = "pay",
                amount = purchase.Amount,
                currency = "UAH",
                description = "Оплата послуги на Asmodeus Project",
                order_id = orderId,
                result_url = $"https://asmodeus.bsite.net/api/success-payment/{orderId}",
                //result_url = $"https://localhost:7234/api/success-payment/{orderId}",
                customer_user_id = purchase.SteamId,
            };

            var requestDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

            var requestDataEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestDataJson));
            var signString = privateKey + requestDataEncoded + privateKey;

            string signature = "";
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(signString));
                signature = Convert.ToBase64String(hashBytes);
            }

            var order = new Order(orderId, purchase.Amount, purchase.SteamId, "processing");
            await _orderRepository.CreateOrderAsync(order);

            var paymentUrl = $"https://www.liqpay.ua/api/3/checkout?data={requestDataEncoded}&signature={signature}";

            return Ok(paymentUrl);
        }

        [HttpGet]
        [Route("/api/success-payment/{orderId}")]
        public async Task<IActionResult> SuccessPayment(string orderId)
        {
            var publicKey = _configuration["LiqPayPublicKey"];
            var privateKey = _configuration["LiqPayPrivateKey"];
            var requestData = new
            {
                action = "status",
                version = 3,
                public_key = publicKey,
                order_id = orderId
            };
            var requestDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

            var requestDataEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestDataJson));
            var signString = privateKey + requestDataEncoded + privateKey;

            string signature = "";
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(signString));
                signature = Convert.ToBase64String(hashBytes);
            }

            var encodedRequestData = Uri.EscapeDataString(requestDataEncoded);

            var formData = new StringContent($"data={encodedRequestData}&signature={signature}");
            formData.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync("https://www.liqpay.ua/api/request", formData);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseContent);
                System.Console.WriteLine(jsonResponse.ToString());

                string resultValue = jsonResponse["status"]!.ToString();

                if (resultValue == "success")
                {
                    await _orderRepository.CompleteOrderAsync(orderId);
                    return Ok($"Payment completed. \n {responseContent}");
                }
                return BadRequest($"Request failed. Result is not success. Body: \n {responseContent}");
            }
            return BadRequest();
        }

        private string RandomString(int length)
        {
            var random = new Random();
            const string chars = "0123456789";
            return new (Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}