using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository _repository)
        {
            this._repository = _repository;
        }

        [HttpGet]
        [Route("/api/users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _repository.GetUsersAsync());
        }

        [HttpPost]
        [Route("/api/create")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _repository.CreateUserAsync(user);
                    return Ok("User has been successfully created");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException);
            }
        }
    }
}