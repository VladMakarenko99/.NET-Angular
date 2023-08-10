using Microsoft.AspNetCore.Mvc;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace API.Controllers
{
    [Route("[controller]")]
    public class SteamController : Controller
    {
        private readonly IConfiguration _configuration;
        public SteamController(IConfiguration _configuration) => this._configuration = _configuration;
        

        [HttpGet]
        [Route("/api/games/{steamId}")]
        public async Task<IActionResult> Interact(ulong steamId)
        {
            try
            {
                string apiKey = _configuration["SteamApiKey"] ?? "";
                var steamWebInterfaceFactory = new SteamWebInterfaceFactory(apiKey);
                var playerService = steamWebInterfaceFactory.CreateSteamWebInterface<SteamUser>();
                var user =  await playerService.GetCommunityProfileAsync(steamId);

                Console.WriteLine(user.Summary);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException);
            }
        }
    }
}