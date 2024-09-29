var builder = WebApplication.CreateBuilder(args);

//*add services to container
var assembly = typeof(Program).Assembly;

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

//Cross-cutting Services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

//*config HTTP request pipeline
app.MapCarter();
app.UseExceptionHandler(opts => { });

app.Run();
