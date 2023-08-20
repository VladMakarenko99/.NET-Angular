using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNet.Security.OpenId.Steam;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.Models;
using Newtonsoft.Json;

[Route("[controller]")]
public class AuthController : Controller
{
    private readonly IUserRepository _repository;
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository _repository, IConfiguration _configuration)
    {
        this._repository = _repository;
        this._configuration = _configuration;
    }

    [HttpGet]
    [Route("/steam-signin")]
    public IActionResult SteamSignIn()
    {
        if (User!.Identity!.IsAuthenticated)
            return BadRequest("User is already signed in");

        var authProperties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("SteamResponse")
        };
        return Challenge(authProperties, SteamAuthenticationDefaults.AuthenticationScheme);
    }

    [Route("/steam-response")]
    [HttpGet]
    public async Task<IActionResult> SteamResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
            return BadRequest("Not Authenticated");

        var steamIdClaim = result.Principal.Claims.FirstOrDefault(c => c.Issuer == "Steam");

        if (steamIdClaim == null)
            return BadRequest("SteamId claim not found.");

        int lastSlashIndex = steamIdClaim.Value.LastIndexOf('/');
        string steamId = steamIdClaim.Value.Substring(lastSlashIndex + 1);

        string apiKey = _configuration["SteamApaKey"] ?? "";
        HttpResponseMessage httpResponseMessage = await new HttpClient()
        .GetAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamId}");
        var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        string steamName = json!.response.players[0].personaname;

        if (await _repository.GetBySteamIdAsync(steamId) == null)
        {
            var user = new User(steamId, steamName, null, 0);

            await _repository.CreateUserAsync(user);
            return Ok($"User has been created and successfully authenticated. SteamId: {steamId}; SteamName: {steamName}");
        }

        return Ok($"Authenticated. SteamId: {steamId}; SteamName: {steamName}");
    }

    [HttpGet]
    [Route("/check-auth")]
    [Authorize(AuthenticationSchemes = SteamAuthenticationDefaults.AuthenticationScheme)]
    public IActionResult CheckAuthentication() => Ok("User is authenticated.");

    [HttpGet]
    [Route("/signout")]
    public IActionResult SignOutCurrentUser() => SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme);

}
