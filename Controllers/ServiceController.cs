using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    public class ServiceController : Controller
    {
        private readonly IServiceRepository _repository;

        public ServiceController(IServiceRepository _repository)
        {
            this._repository = _repository;
        }

        [HttpGet]
        [Route("/api/services")]
        public async Task<IActionResult> GetServices() =>  Ok(await _repository.GetServicesAsync());
    }
}