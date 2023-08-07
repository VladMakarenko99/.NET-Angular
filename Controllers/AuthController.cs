using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using AspNet.Security.OpenId.Steam;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[Route("[controller]")]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("/steam-signin")]
    public IActionResult SteamSignIn()
    {
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
        {
            return BadRequest("Not Authenticated");
        }

        var steamIdClaim = result.Principal.Claims.FirstOrDefault(c => c.Issuer == "Steam");
        if (steamIdClaim == null)
        {
            return BadRequest("SteamId claim not found.");
        }

        int lastSlashIndex = steamIdClaim.Value.LastIndexOf('/');
        string steamId = steamIdClaim.Value.Substring(lastSlashIndex + 1);

        return Ok($"Authenticated. SteamId: {steamId}");
    }
    
    [Authorize]
    [HttpGet]
    [Route("/secure")]
    public IActionResult Secure()
    {
        return Ok($"Authenticadet");
    }

    // [HttpGet("~/signout"), HttpPost("~/signout")]
    // public IActionResult SignOutCurrentUser()
    // {
    //     // Instruct the cookies middleware to delete the local cookie created
    //     // when the user agent is redirected from the external identity provider
    //     // after a successful authentication flow (e.g Google or Facebook).
    //     return SignOut(new AuthenticationProperties { RedirectUri = "/" },
    //         CookieAuthenticationDefaults.AuthenticationScheme);
    // }
}
