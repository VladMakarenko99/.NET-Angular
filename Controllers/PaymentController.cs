using System.Security.Cryptography;
using System.Text;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API.Controllers;

[Route("api/payment")]
public class PaymentController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;


    public PaymentController(
        IConfiguration _configuration,
        IOrderRepository _orderRepository,
        IUserRepository _userRepository)
    {
        this._configuration = _configuration;
        this._orderRepository = _orderRepository;
        this._userRepository = _userRepository;
    }

    [HttpPost("top-up")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> TopUpBalance([FromBody] TopUpPurchase purchase)
    {
        try
        {
            if (!await _userRepository.IsUserExistAsync(purchase.SteamId))
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
        var publicKey = _configuration["LiqPay:PublicKey"];
        var privateKey = _configuration["LiqPay:PrivateKey"];
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
            result_url = $"https://localhost:7234/api/payment-response",
            //result_url = $"https://asmodeus.bsite.net/api/payment-response",
        };

        var requestDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

        var requestDataEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestDataJson));

        string signature = CalculateSignature(privateKey!, requestDataEncoded);

        var order = Order.CreateWithCurrentDateTimeInKyiv(orderId, purchase.Amount, purchase.SteamId, "processing");
        await _orderRepository.CreateOrderAsync(order);

        var paymentUrl = $"https://www.liqpay.ua/api/3/checkout?data={requestDataEncoded}&signature={signature}";

        return Ok(paymentUrl);
    }

    [HttpPost("response")]
    public async Task<IActionResult> SuccessPayment()
    {
        try
        {
            string? postData = Request.Form["data"];
            string? postSignature = Request.Form["signature"];
            var privateKey = _configuration["LiqPay:PrivateKey"];

            if (postData != null || postSignature != null)
            {
                string originalSignature = CalculateSignature(privateKey!, postData!);
                if (originalSignature == postSignature)
                {
                    var decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(postData!));
                    JObject jsonResponse = JObject.Parse(decodedData);

                    string statusValue = jsonResponse["status"]!.ToString();

                    if (statusValue == "success")
                    {
                        await _orderRepository.CompleteOrderAsync(jsonResponse["order_id"]!.ToString());
                        return Ok($"Payment completed. \n Status: {statusValue}");
                    }
                    return BadRequest($"Payment failed. Status: \n {statusValue}");
                }
                return BadRequest("Invalid signature");
            }
            return BadRequest("Post data are null");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private string RandomString(int length)
    {
        var random = new Random();
        const string chars = "0123456789";
        return new(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    private string CalculateSignature(string privateKey, string postData)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            var dataBytes = Encoding.UTF8.GetBytes(privateKey + postData + privateKey);
            var hashBytes = sha1.ComputeHash(dataBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}