using Carter;
using Coupon.Grpc;
using Email.API.Models;
using Email.API.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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


builder.Services.AddGrpcClient<CouponProtoService.CouponProtoServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:3010");
});

builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});



builder.Services.AddTransient<ISenderEmail, SenderEmail>();
builder.Services.AddTransient<IEmailRepository, EmailRepository>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.MapCarter();

app.Run();