using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ParticlesAPI.NET5.Controllers
{
    [ApiController]
    [Route("particles")]
    public class ParticlesController : ControllerBase
    {
        private readonly ILogger<ParticlesController> _logger;

        public ParticlesController(ILogger<ParticlesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            _logger.LogInformation("Call Get method");

            return new[] { "Particle1", "Particle2", "Particle3" };
        }
    }
}
