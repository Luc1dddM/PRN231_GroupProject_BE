using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;
using System.Text;
using System.Text.Json;
using ToolZone.Gateway.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOcelot();

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
                     .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);


var jwtSection = builder.Configuration.GetSection("JWT");
builder.Services.Configure<Jwt>(jwtSection);

var appSettings = jwtSection.Get<Jwt>();
var secret = Encoding.ASCII.GetBytes(appSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = true;
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidIssuer = appSettings.ValidIssuer,
        ValidAudience = appSettings.ValidAudience,
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(secret)
    };

    o.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.StatusCode = 410;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    message = "Token has expired."
                });

                return context.Response.WriteAsync(result);
            }

            return Task.CompletedTask;
        }
    };

});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseOcelot().Wait();

app.Run();
