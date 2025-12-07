using BlazorGame.GameService.Persistence;
using BlazorGame.GameService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseInMemoryDatabase("GameDb")
);
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped(typeof(Repository<>));
builder.Services.AddScoped<GameplayService>();
builder.Services.AddSingleton<SalleService>();
builder.Services.AddScoped<DonjonService>();
builder.Services.AddScoped<ScoresService>();
builder.Services.AddScoped<PartieService>();

builder.Services.AddCors(o =>
    o.AddPolicy("AllowBlazorClient", p =>
        p.WithOrigins("https://localhost:5003", "http://localhost:5003")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BlazorGame.GameService API",
        Version = "v1"
    });
});

builder.Services.AddAuthentication.AddJwtBearer(options =>
{
    options.Authority = "http://localhost:8180/realms/gamequest";
    options.Audience = "gamequest-backend";
    options.RequireHttpsMetadata = false;
});

builder.Services.AddAuthorizationBuilder();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    db.Database.EnsureCreated();

    var guestId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    if (!db.Joueurs.Any(j => j.Id == guestId))
    {
        db.Joueurs.Add(new SharedModels.Domain.Users.Joueur
        {
            Id = guestId,
            Pseudo = "Guest",
            KeycloakUserName = "guest",
            Actif = true
        });
        db.Joueurs.Add(new SharedModels.Domain.Users.Joueur
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Pseudo = "Admin",
            KeycloakUserName = "admin",
            Actif = true,
            Admin = true
        });
        db.SaveChanges();
    }
    // creation d'un admin par défaut
    if (!db.Joueurs.Any(j => j.Pseudo == "admin1234" && j.Admin))
    {
        db.Joueurs.Add(new SharedModels.Domain.Users.Joueur
        {
            Id = Guid.NewGuid(),
            Pseudo = "admin1234",
            KeycloakUserName = "admin1234",
            Actif = true,
            Admin = true
        });
        db.SaveChanges();
    }
}
app.UseStaticFiles();
app.UseCors("AllowBlazorClient");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BlazorGame.GameService API v1");
    });
}

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
