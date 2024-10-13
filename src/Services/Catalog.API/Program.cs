using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using Catalog.API.Models;
using Catalog.API.Repository;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using BuildingBlocks.Messaging.MassTransit;
using System.Reflection;
using MapsterMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MyDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DB")));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IUploadImageRepository, UploadImageRepository>();
builder.Services.AddScoped<IMapper, Mapper>();
builder.Services.AddMessageBroker(builder.Configuration,Assembly.GetExecutingAssembly());

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();




var app = builder.Build();
app.MapCarter();
//*config HTTP request pipeline
app.UseExceptionHandler(opts => { });




app.Run();
