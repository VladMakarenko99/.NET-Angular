using System.Text.RegularExpressions;
using API.Interfaces;
using API.Models;
using CloudIpspSDK;
using CloudIpspSDK.Checkout;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IUserRepository _userRepository;

        public ServiceController(IServiceRepository _repository, IUserRepository _userRepository)
        {
            this._serviceRepository = _repository;
            this._userRepository = _userRepository;
        }

        [HttpGet]
        [Route("/api/services")]
        public async Task<IActionResult> GetServices() => Ok(await _serviceRepository.GetServicesAsync());

        [HttpPost]
        [Route("/api/buy-service")]
        public async Task<IActionResult> BuyService([FromBody] Purchase purchase)
        {
            try
            {
                if (!await _serviceRepository.IsUserExistAsync(purchase.SteamId))
                    ModelState.AddModelError(nameof(purchase.SteamId), "Invalid SteamId!");
                if (string.IsNullOrEmpty(purchase.Service.SelectedOption))
                    ModelState.AddModelError(nameof(purchase.Service.SelectedOption), "Selected option is null!");
                if (!Array.Exists(purchase.Service.OptionsToSelect, opt => opt == purchase.Service.SelectedOption))
                    ModelState.AddModelError(nameof(purchase.Service.SelectedOption), "Invalid selected option!");
                string option = purchase.Service!.SelectedOption!;
                string pattern = @"\d+";
                MatchCollection matches = Regex.Matches(option, pattern);
                double price = Convert.ToDouble(matches[0].Value);
                var user = await _userRepository.GetBySteamIdAsync(purchase.SteamId);
                if (user!.Balance < price)
                    ModelState.AddModelError("UserBalance", "Not enough money on userʼs balance!");
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage));
                    return BadRequest(errors);
                }
            }
            catch (Exception) { return BadRequest("Invalid json"); }

            await _serviceRepository.BuyServiceAsync(purchase.Service, purchase.SteamId);
            return Ok("Service was bought successfully");
        }

        [HttpDelete]
        [Route("/api/delete-services/{steamId}")]
        public async Task<IActionResult> DeleteServices(string steamId)
        {
            try
            {
                if (!string.IsNullOrEmpty(steamId))
                {
                    await _serviceRepository.DeleteBoughtServicesAsync(steamId);
                    return Ok("Services have been successfully deleted");
                }
                else
                    return BadRequest("Incorrect service id or user id");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException);
            }
        }
    }
}