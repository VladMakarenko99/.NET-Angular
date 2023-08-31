using API.Interfaces;
using API.Models;
using AspNet.Security.OpenId.Steam;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace API.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository _userRepository)
        {
            this._repository = _userRepository;
        }

        [HttpGet]
        [Route("/api/users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _repository.GetUsersAsync());
        }
        [HttpGet]
        [Route("/api/users/{steamId}")]
        [Authorize(AuthenticationSchemes = SteamAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserBySteamId(string steamId)
        {
            var user = await _repository.GetBySteamIdAsync(steamId);
            if (user == null)
                return BadRequest("User could not be found");

            return Ok(user);
        }

        [HttpGet("/api/user")]
        public IActionResult GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var id = User.FindFirst(ClaimTypes.Surname)?.Value;
            return Ok(id);
        }
    }
}