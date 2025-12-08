var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddCors(o =>
    o.AddPolicy("AllowBlazorClient", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod()
    )
);

var app = builder.Build();

app.UseCors("AllowBlazorClient");

// Routing API
app.MapControllers();

app.Run();
