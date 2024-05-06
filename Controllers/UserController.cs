using System.Security.Claims;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Steam.Models;

namespace API.Controllers;

[Route("api/users")]
public class UserController : Controller
{
    private readonly IUserRepository _repository;


    public UserController(IUserRepository _userRepository)
    {
        this._repository = _userRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _repository.GetUsersAsync());
    }

    // [HttpGet("{steamId}")]
    // [Authorize(AuthenticationSchemes = "Bearer")]
    // public async Task<IActionResult> GetUserBySteamId(string steamId)
    // {
    //     if(steamId.Contains("current"))
    //         return Redirect("/current");
    //     var headers = Request.Headers;
    //     System.Console.WriteLine(headers.FirstOrDefault(z => z.Key == "Authorization"));
    //     var user = await _repository.GetBySteamIdAsync(steamId);
    //     if (user == null)
    //         return NotFound("User could not be found");

    //     return Ok(user);
    // }

    [HttpGet("current")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public IActionResult GetCurrentUser()
    {
        string? name = HttpContext.User.FindFirstValue("SteamName");
        string? id = HttpContext.User.FindFirstValue("SteamId");
        string? boughtServicesJson = HttpContext.User.FindFirstValue("BoughtServicesJson");
        string? balance = HttpContext.User.FindFirstValue("Balance");

        if (name != null && id != null)
        {
            var user = new User(id, name, boughtServicesJson, Convert.ToDouble(balance));
            return Ok(user);
        }
        return BadRequest();
    }
}