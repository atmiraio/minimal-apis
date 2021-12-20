using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Services
#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ParticlesAPI.NET6", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
});
#endregion

#region Context
builder.Services.AddDbContext<ParticlesDB>(options => options.UseInMemoryDatabase("Particles"));
#endregion

#region Authorization & Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5001";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    });
});
#endregion

builder.Services.AddScoped<IParticlesService, ParticlesService>();

var app = builder.Build();

//Routes
app.MapGet("/particles",
    async (IParticlesService srv) =>
        await srv.GetAllParticles()
    )
    .Produces<List<Particle>>(StatusCodes.Status200OK)
    .WithName("GetAllParticles")
    .RequireAuthorization();

app.MapGet("/particles/{id}",
    async (int id, IParticlesService srv) =>
        await srv.GetParticle(id) is Particle particle
            ? Results.Ok(particle)
            : Results.NotFound()
    )
    .Produces<Particle>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetParticleById");

app.MapPost("/particles",
    async (Particle particle, IParticlesService srv) =>
    {
        await srv.AddParticle(particle);

        return Results.Created($"/particles/{particle.Id}", particle);
    })
    .Accepts<Particle>("application/json")
    .Produces<Particle>(StatusCodes.Status201Created)
    .WithName("CreateParticle");

app.MapPut("/particles",
    async (Particle particle, IParticlesService srv) =>
        await srv.UpdateParticle(particle) is Particle entity
            ? Results.Ok(entity)
            : Results.NotFound()
    )
    .Accepts<Particle>("application/json")
    .Produces<Particle>(StatusCodes.Status200OK)
    .Produces<Particle>(StatusCodes.Status404NotFound)
    .WithName("UpdateParticle");

app.MapDelete("/particles/{id}",
    async (int id, IParticlesService srv) =>
    {
        var result = await srv.RemoveParticle(id);

        return Results.Ok(result);
    })
    .Produces<Particle>(StatusCodes.Status200OK);
//.ExcludeFromDescription();


//Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ParticlesDB>();
    DataGenerator.Initialize(scope.ServiceProvider);
}

app.UseAuthentication();
app.UseAuthorization();

app.Run();

class ParticlesDB : DbContext
{
    public ParticlesDB(DbContextOptions options) : base(options) { }
    public DbSet<Particle> Particles => Set<Particle>();
}

class Particle
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Symbol { get; set; }
    public string? Spin { get; set; }
    public string? Charge { get; set; }
    public double Mass { get; set; }
    public Type Type { get; set; }
    public string? TypeName { get; set; }
}

enum Type
{
    Qurk,
    Lepton,
    Boson
}

interface IParticlesService
{
    public Task<IEnumerable<Particle>> GetAllParticles();
    public Task<Particle> GetParticle(int id);
    public Task<Particle> AddParticle(Particle particle);
    public Task<Particle> UpdateParticle(Particle particle);
    public Task<bool> RemoveParticle(int id);
}

class ParticlesService : IParticlesService
{
    private readonly ParticlesDB _context;

    public ParticlesService(ParticlesDB context)
    {
        _context = context;
    }

    public async Task<Particle> AddParticle(Particle particle)
    {
        var entity = _context.Particles.Add(particle).Entity;
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<IEnumerable<Particle>> GetAllParticles()
    {
        return await _context.Particles.ToListAsync();
    }

    public async Task<Particle> GetParticle(int id)
    {
        return await _context.Particles.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> RemoveParticle(int id)
    {
        var entity = _context.Particles.FirstOrDefault(p => p.Id == id);
        if (entity != null)
        {
            _context.Particles.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<Particle> UpdateParticle(Particle particle)
    {
        var entity = _context.Particles.FirstOrDefaultAsync(p => p.Id == particle.Id).Result;

        if (entity != null)
        {
            entity.Name = particle.Name;
            entity.Symbol = particle.Symbol;
            entity.Type = particle.Type;
            entity.Mass = particle.Mass;
            entity.Charge = particle.Charge;
            entity.Spin = particle.Spin;

            _context.Particles.Update(entity);
            await _context.SaveChangesAsync();
        }

        return entity;
    }
}
