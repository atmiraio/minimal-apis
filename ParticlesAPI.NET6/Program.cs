var builder = WebApplication.CreateBuilder(args);

//Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Routes
app.MapGet("/minimal", () => "Get method");
app.MapPost("/minimal", () => "Post method");
app.MapPut("/minimal", () => "Put method");
app.MapDelete("/minimal", () => "Delete method");

//Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.Run();