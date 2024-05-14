using System.Text.Json;
using System.Text.RegularExpressions;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/services")]
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
    public async Task<IActionResult> GetServices() => Ok(await _serviceRepository.GetServicesAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetServiceseById(int id) => Ok(await _serviceRepository.GetServiceByIdAsync(id));

    [HttpPost("buy")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> BuyService([FromBody] Purchase purchase)
    {
        try
        {
            if (!await _userRepository.IsUserExistAsync(purchase.SteamId))
                ModelState.AddModelError(nameof(purchase.SteamId), "Invalid SteamId!");

            if (string.IsNullOrEmpty(purchase.Service.SelectedOption))
                ModelState.AddModelError(nameof(purchase.Service.SelectedOption), "Selected option is null!");

            if (!Array.Exists(purchase.Service.OptionsToSelect, opt => opt == purchase.Service.SelectedOption))
                ModelState.AddModelError(nameof(purchase.Service.SelectedOption), "Invalid selected option!");

            double price = ExtractPriceFromSelectedOption(purchase.Service.SelectedOption!);
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

        purchase.Service.ExpireDate = CalculateExpireDate(purchase.Service.SelectedOption!);
        await _serviceRepository.BuyServiceAsync(purchase.Service, purchase.SteamId);
        return Ok();
    }
    private double ExtractPriceFromSelectedOption(string selectedOption)
    {
        string pattern = @"\d+";
        MatchCollection matches = Regex.Matches(selectedOption, pattern);
        return Convert.ToDouble(matches[0].Value);
    }
    private string CalculateExpireDate(string selectedOption)
    {
        Regex regex = new Regex(@"- (\d+)дн");
        Match match = regex.Match(selectedOption);
        double value = Convert.ToDouble(match.Groups[1].Value);
        return DateTime.Now.ToLocalTime().AddDays(value).ToString();
    }
}