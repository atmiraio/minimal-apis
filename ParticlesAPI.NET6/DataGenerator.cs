using Microsoft.EntityFrameworkCore;

internal class DataGenerator
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new ParticlesDB(serviceProvider.GetRequiredService<DbContextOptions<ParticlesDB>>());
        if (context.Particles.Any())
        {
            return;   // Data was already seeded
        }

        context.Particles.AddRange(
            new Particle
            {
                Id = 1,
                Name = "up",
                Symbol = "u",
                Spin = "1/2",
                Charge = "+2/3",
                Mass = 2.2,
                Type = Type.Qurk,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 2,
                Name = "down",
                Symbol = "d",
                Spin = "1/2",
                Charge = "-1/3",
                Mass = 4.6,
                Type = Type.Qurk,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 3,
                Name = "charm",
                Symbol = "c",
                Spin = "1/2",
                Charge = "+2/3",
                Mass = 1280,
                Type = Type.Qurk,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 4,
                Name = "strange",
                Symbol = "s",
                Spin = "1/2",
                Charge = "-1/3",
                Mass = 96,
                Type = Type.Qurk,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 5,
                Name = "top",
                Symbol = "t",
                Spin = "1/2",
                Charge = "+2/3",
                Mass = 173100,
                Type = Type.Qurk,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 6,
                Name = "bottom",
                Symbol = "b",
                Spin = "1/2",
                Charge = "-1/3",
                Mass = 4180,
                Type = Type.Qurk,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 7,
                Name = "Electron",
                Symbol = "e⁻",
                Spin = "1/2",
                Charge = "-1",
                Mass = 0.511,
                Type = Type.Lepton,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 8,
                Name = "Electron neutrino",
                Symbol = "νₑ",
                Spin = "1/2",
                Charge = "0",
                Mass = 0.0000022,
                Type = Type.Lepton,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 9,
                Name = "Muon",
                Symbol = "μ⁻",
                Spin = "1/2",
                Charge = "-1",
                Mass = 105.7,
                Type = Type.Lepton,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 10,
                Name = "Muon neutrino",
                Symbol = "νᵤ",
                Spin = "1/2",
                Charge = "0",
                Mass = 0.170,
                Type = Type.Lepton,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 11,
                Name = "Tau",
                Symbol = "T⁻",
                Spin = "1/2",
                Charge = "-1",
                Mass = 1776.86,
                Type = Type.Lepton,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 12,
                Name = "Tau neutrino",
                Symbol = "νₜ",
                Spin = "1/2",
                Charge = "0",
                Mass = 15.5,
                Type = Type.Lepton,
                TypeName = Type.Lepton.ToString()
            },
            new Particle
            {
                Id = 13,
                Name = "Photon",
                Symbol = "Υ",
                Spin = "1",
                Charge = "0",
                Mass = 0,
                Type = Type.Boson,
                TypeName = Type.Boson.ToString()
            },
            new Particle
            {
                Id = 14,
                Name = "W boson",
                Symbol = "W⁻",
                Spin = "1",
                Charge = "-1",
                Mass = 80.385,
                Type = Type.Boson,
                TypeName = Type.Boson.ToString()
            },
            new Particle
            {
                Id = 15,
                Name = "Z boson",
                Symbol = "Z",
                Spin = "1",
                Charge = "0",
                Mass = 91.1575,
                Type = Type.Boson,
                TypeName = Type.Boson.ToString()
            },
            new Particle
            {
                Id = 16,
                Name = "Gluon",
                Symbol = "g",
                Spin = "1",
                Charge = "0",
                Mass = 0,
                Type = Type.Boson,
                TypeName = Type.Boson.ToString()
            },
            new Particle
            {
                Id = 17,
                Name = "Tau neutrino",
                Symbol = "H⁰",
                Spin = "1",
                Charge = "0",
                Mass = 125.9,
                Type = Type.Boson,
                TypeName = Type.Boson.ToString()
            }
            );

        context.SaveChanges();
    }
}