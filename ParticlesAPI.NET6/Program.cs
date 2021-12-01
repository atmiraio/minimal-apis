using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ParticlesAPI.NET6", Version = "v1" });
});
builder.Services.AddDbContext<ParticlesDB>(options => options.UseInMemoryDatabase("Particles"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

//Routes
app.MapGet("/particles", async (ParticlesDB db) => await db.Particles.ToListAsync()).RequireAuthorization();
app.MapGet("/particles/{id}", async (int id, ParticlesDB db) => await db.Particles.FirstOrDefaultAsync(p => p.Id == id) is Particle particle ? Results.Ok(particle) : Results.NotFound());
app.MapPost("/particles",
    async (Particle particle, ParticlesDB db) =>
    {
        db.Particles.Add(particle);
        await db.SaveChangesAsync();

        return Results.Ok(particle);
    });
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ParticlesDB>();
    DataGenerator.Initialize(scope.ServiceProvider);
}

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

