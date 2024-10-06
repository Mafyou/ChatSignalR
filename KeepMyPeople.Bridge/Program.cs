using KeepInTouch.API.Providers;
using KeepMyPeople.Bridge.AuthToken;
using KeepMyPeople.Bridge.Chat;
using KeepMyPepole.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddTransient<JwtProvider>();

builder.Services.AddSignalR(o =>
{
    // Null to unlimited
    o.MaximumReceiveMessageSize = null;
    o.EnableDetailedErrors = true;
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();


app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = string.Empty;
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Name of Your API v1");
});
var hubName = $"{Constants.Hub_Name}";
var ar = app.MapHub<KITHub>(hubName); //, o => o.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets);
// var ar = app.MapHub<KITHub>(ep);

app.MapControllers();

app.MapPost("/Token", ([FromBody]Guid id, [FromServices] JwtProvider jwtProvider) =>
{
    var token = jwtProvider.Generate(id);
    return token;
});

app.Run();