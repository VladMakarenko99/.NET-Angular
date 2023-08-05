using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SteamWebAPI2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;

namespace API.Controllers
{
    [Route("[controller]")]
    public class SteamController : Controller
    {
        private readonly string apiKey = "B85693F3DAA739B3C53C95ABC6E63B64";

        [HttpGet]
        [Route("/api/games/{steamId}")]
        public async Task<IActionResult> Interact(ulong steamId)
        {
            try
            {
                var steamWebInterfaceFactory = new SteamWebInterfaceFactory(apiKey);
                var playerService = steamWebInterfaceFactory.CreateSteamWebInterface<PlayerService>();
                var games = await playerService.GetOwnedGamesAsync(steamId, true);

                return Ok(games);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}