using BuildingBlocks.Exceptions.Handler;
using Coupon.API.Models;
using Coupon.API.Repository;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using BuildingBlocks.Messaging.MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalPolicy",
        builder => builder.WithOrigins("http://localhost:5173") 
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddDbContext<Prn231GroupProjectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddTransient<ICouponRepository, CouponRepository>();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());

var app = builder.Build();

app.UseCors("LocalPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler(opts => { });
app.UseAuthorization();

app.MapControllers();

app.Run();
