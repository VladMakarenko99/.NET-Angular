using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SteamWebAPI2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Mappings;
using SteamWebAPI2.Utilities;

namespace API.Controllers
{
    [Route("[controller]")]
    public class SteamController : Controller
    {
        private const string apiKey = "B85693F3DAA739B3C53C95ABC6E63B64";

        [HttpGet]
        [Route("/api/games/{steamId}")]
        public async Task<IActionResult> Interact(ulong steamId)
        {
            try
            {
                var steamWebInterfaceFactory = new SteamWebInterfaceFactory(apiKey);
                var playerService = steamWebInterfaceFactory.CreateSteamWebInterface<SteamUser>();
                var games =  await playerService.GetCommunityProfileAsync(steamId);

                Console.WriteLine(games.RealName);
                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException);
            }
        }
    }
}