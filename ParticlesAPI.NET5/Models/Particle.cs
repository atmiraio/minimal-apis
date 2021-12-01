using System;

namespace ParticlesAPI.NET5
{
    public class Particle
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Spin { get; set; }
        public string Charge { get; set; }
        public double Mass { get; set; }
        public Type Type { get; set; }
        public string TypeName { get; set; }
    }
}
