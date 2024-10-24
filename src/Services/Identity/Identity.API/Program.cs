using Identity.API;
using Identity.Application;
using Identity.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80); 
});
*/
if (builder.Environment.IsEnvironment("Production"))
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddApplicationServices()
                .AddInfastructureServices(builder.Configuration)
                .AddApiServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();
app.UseApiServices();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    Console.WriteLine("Develop Mode");
}

app.UseAuthorization();

app.MapControllers();

app.Run();
