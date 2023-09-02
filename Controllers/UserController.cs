using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("{steamId}")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> GetUserBySteamId(string steamId)
    {
        var headers = Request.Headers;
        System.Console.WriteLine(headers.FirstOrDefault(z => z.Key == "Authorization"));
        var user = await _repository.GetBySteamIdAsync(steamId);
        if (user == null)
            return NotFound("User could not be found");

        return Ok(user);
    }
}