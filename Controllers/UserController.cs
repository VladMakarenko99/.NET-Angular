using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetUserBySteamId(string steamId)
        {
            var user = await _repository.GetBySteamIdAsync(steamId);
            if (user == null)
                return BadRequest("User could not be found");

            return Ok(user);
        }
    }
}