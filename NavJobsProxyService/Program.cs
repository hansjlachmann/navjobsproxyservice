using NavJobsProxyService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Windows Service support
builder.Host.UseWindowsService();

// Add services
builder.Services.AddSingleton<INavService, NavService>();
builder.Services.AddHostedService<NavJobsWorker>();

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure HTTP request pipeline
app.MapControllers();

await app.RunAsync();

