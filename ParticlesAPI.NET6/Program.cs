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

var app = builder.Build();

//Routes
app.MapGet("/particles",
    async (ParticlesDB db) =>
        await db.Particles.ToListAsync()
    )
    .Produces<List<Particle>>(StatusCodes.Status200OK)
    .WithName("GetAllParticles")
    .RequireAuthorization();

app.MapGet("/particles/{id}",
    async (int id, ParticlesDB db) =>
        await db.Particles.FirstOrDefaultAsync(p => p.Id == id) is Particle particle
            ? Results.Ok(particle)
            : Results.NotFound()
    )
    .Produces<Particle>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetParticleById");

app.MapPost("/particles",
    async (Particle particle, ParticlesDB db) =>
    {
        db.Particles.Add(particle);
        await db.SaveChangesAsync();

        return Results.Created($"/particles/{particle.Id}", particle);
    })
    .Accepts<Particle>("application/json")
    .Produces<Particle>(StatusCodes.Status201Created)
    .WithName("CretetParticle");

app.MapDelete("/particles/{id}",
    async (int id, ParticlesDB db) =>
    {
        var entity = db.Particles.FirstOrDefault(p => p.Id == id);
        if (entity != null)
        {
            db.Particles.Remove(entity);
            await db.SaveChangesAsync();
            return Results.Ok(true);
        }

        return Results.Ok(false);
    });
//.ExcludeFromDescription();

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
