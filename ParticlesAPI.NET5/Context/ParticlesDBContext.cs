using Microsoft.EntityFrameworkCore;

namespace ParticlesAPI.NET5
{
    public class ParticlesDBContext : DbContext
    {
        public ParticlesDBContext(DbContextOptions<ParticlesDBContext> options)
        : base(options) { }

        public DbSet<Particle> Particles { get; set; }
    }
}
