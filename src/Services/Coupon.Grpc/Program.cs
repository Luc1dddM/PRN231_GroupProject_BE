using Carter;
using Coupon.Grpc;
using Coupon.Grpc.Models;
using Coupon.Grpc.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
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

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGrpcService<CouponService>();
// Configure the HTTP request pipeline.
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapGet("/api/coupons/{couponCode}", async (string couponCode, CouponProtoService.CouponProtoServiceClient client) =>
{
    var request = new GetCouponRequest { CouponCode = couponCode };
    var response = await client.GetCouponAsync(request);
    return Results.Ok(response);
})
                .WithTags("Coupons")
                .WithName("GetCoupon");

app.MapGet("/api/coupons", async (CouponProtoService.CouponProtoServiceClient client) =>
{
    var response = await client.GetCouponsAsync(new Empty());
    return Results.Ok(response.Coupons);
})
    .WithTags("Coupons")
    .WithName("GetCoupons");

app.MapPost("/api/coupons", async (CreateCouponRequest request, CouponProtoService.CouponProtoServiceClient client) =>
{
    var response = await client.CreateCouponAsync(request);
    return Results.Ok(response);
})
    .WithTags("Coupons")
    .WithName("CreateCoupon");

app.MapPut("/api/coupons/{id}", async (int id, EditCouponRequest request, CouponProtoService.CouponProtoServiceClient client) =>
{
    request.Id = id;
    var response = await client.EditCouponAsync(request);
    return Results.Ok(response);
})
    .WithTags("Coupons")
    .WithName("EditCoupon");


app.Run();
