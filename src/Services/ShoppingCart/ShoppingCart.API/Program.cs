using BuildingBlocks.Messaging.MassTransit;

var builder = WebApplication.CreateBuilder(args);

//*add services to container
var assembly = typeof(Program).Assembly;
// CORS policy name constant
const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

//Add  HttpContext
builder.Services.AddHttpContextAccessor();

//Add Cors
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: MyAllowSpecificOrigins,
                  policy =>
                  {
                      policy.WithOrigins("http://localhost:5173")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                  });
});

//Application Services
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

//Data Services
builder.Services.AddDbContext<CartDBContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DB"));
});
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.Decorate<ICartRepository, CachedCartRepository>();//use scrutor decorate with distributed cache 

builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = builder.Configuration.GetConnectionString("Redis");
});

//Async Communication Services
builder.Services.AddMessageBroker(builder.Configuration);

//Cross-cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

//*config HTTP request pipeline
app.MapCarter();
app.UseExceptionHandler(opts => { });
app.UseCors(MyAllowSpecificOrigins);
app.Run();
