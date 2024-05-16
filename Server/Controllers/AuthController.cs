using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNet.Security.OpenId.Steam;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.Models;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers;
[Route("api")]
public class AuthController : Controller
{
    private readonly IUserRepository _repository;
    private readonly IConfiguration _configuration;

    public AuthController(IUserRepository _repository, IConfiguration _configuration)
    {
        this._repository = _repository;
        this._configuration = _configuration;
    }

    [HttpGet("steam-signin")]
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

    [HttpGet("steam-response")]
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

        string apiKey = _configuration["SteamApiKey"] ?? "";
        HttpResponseMessage httpResponseMessage = await new HttpClient()
        .GetAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamId}");
        var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync();
        var json = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

        string steamName = json!.response.players[0].personaname;

        var user = await _repository.GetBySteamIdAsync(steamId);
        if (user == null)
        {
            user = new User(steamId, steamName, null, 0);

            await _repository.CreateUserAsync(user);
            return Ok(new
            {
                token = GenerateToken(user),
                user_created = true
            });
        }
        //return Ok(GenerateToken(user));
        var token = GenerateToken(user);
        System.Console.WriteLine(token);
        return Redirect($"{_configuration["ClientUrlProd"]}?token=" + token);
    }

    [HttpGet("check-auth")]
    [Authorize(AuthenticationSchemes = SteamAuthenticationDefaults.AuthenticationScheme)]
    public IActionResult CheckAuthentication() => Ok("User is authenticated.");

    [HttpGet("signout")]
    public IActionResult SignOutCurrentUser() => SignOut(new AuthenticationProperties { RedirectUri = $"{_configuration["ClientUrlProd"]}" }, CookieAuthenticationDefaults.AuthenticationScheme);

    private string GenerateToken(User user)
    {
        var key = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("SteamId", user.SteamId),
            new Claim("SteamName", user.SteamName),
            new Claim("BoughtServicesJson", user.BoughtServicesJson ?? ""),
            new Claim("Balance", user.Balance.ToString()),
        };

        var token = new JwtSecurityToken(
         issuer,
          audience,
          claims,
          null,
          DateTime.Now.AddHours(12),
          credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
