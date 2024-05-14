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

    [HttpGet("current")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetCurrentUser()
    {
        string? id = HttpContext.User.FindFirstValue("SteamId");

        if (id != null)
        {
            var user = await _repository.GetBySteamIdAsync(id);

            string? name = user!.SteamName;;
            string? boughtServicesJson = user!.BoughtServicesJson;
            string? balance = Convert.ToString(user.Balance);
            
            user = new User(id, name, boughtServicesJson, Convert.ToDouble(balance));
            return Ok(user);
        }
        return BadRequest();
    }
}