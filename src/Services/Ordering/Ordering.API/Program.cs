using BuildingBlocks.Exceptions.Handler;
using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


//add services to container

//Add  HttpContext
builder.Services.AddHttpContextAccessor();

//-------------------------------
//Application - EF core
//Infrastructure - Mediatr
//Web API - Carter, Heathcheck

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices();
//-------------------------------

//Add cross-cutting concern
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
var app = builder.Build();


//config HTTP request pipeline
app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
app.UseExceptionHandler(opts => { });
app.Run();
