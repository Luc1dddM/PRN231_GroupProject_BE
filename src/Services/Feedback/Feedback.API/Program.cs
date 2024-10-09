using Feedback.API.Models;
using Feedback.API.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Config DB
builder.Services.AddDbContext<Prn231GroupProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddTransient<IFeedbackRepository, FeedbackRepository>();

builder.Services.AddControllers();

builder.Services.AddHttpClient(); 

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("AllowAll");

app.UseRouting();  

app.UseHttpsRedirection();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});

app.Run();
