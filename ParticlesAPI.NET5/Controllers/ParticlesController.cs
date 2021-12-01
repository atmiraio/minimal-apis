using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace ParticlesAPI.NET5.Controllers
{
    [ApiController]
    [Route("particles")]
    public class ParticlesController : ControllerBase
    {
        private readonly ILogger<ParticlesController> _logger;
        private ParticlesDBContext _context;

        public ParticlesController(ILogger<ParticlesController> logger, ParticlesDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Particle> Get()
        {
            return _context.Particles.ToList();
        }

        [HttpGet("/particles/{id}")]
        public Particle GetById(int id)
        {
            return _context.Particles.Where(p => p.Id == id).FirstOrDefault();
        }

        [HttpPost("/particles")]
        public Particle Post([FromBody] Particle particle)
        {
            var entity = _context.Particles.Add(particle).Entity;
            _context.SaveChanges();
            return entity;
        }

        [HttpDelete("/particles/{id}")]
        public bool Delete(int id)
        {
            var entity = _context.Particles.Where(p => p.Id == id).FirstOrDefault();
            if (entity != null)
            {
                _context.Particles.Remove(entity);
                _context.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
