using Serilog;
using YellowBlossom.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.MainService(builder.Configuration);
// Log Configuration
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration)
//    .WriteTo.Console()
//    .WriteTo.File("Logs/YellowBlossom.txt")
//    .CreateLogger();
Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("ServiceName", "ProductService")
    .WriteTo.File("logs/ProductService-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
Log.Logger = new LoggerConfiguration()
    .Enrich.WithProperty("ServiceName", "TaskService")
    .WriteTo.File("logs/TaskService-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
Log.ForContext("ServiceName", "ProductService");


builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.ReadFrom.Services(services);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("client_cors");
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllers();
app.Run();
