using Serilog;
using Serilog.Context;
using YellowBlossom.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.MainService(builder.Configuration);

// Log Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/YellowBlossom.txt")
    .CreateLogger();
using (LogContext.PushProperty("ServiceName", "ProductService"))
{
    Log.Information("This message will be logged with ProductService context");
}

using (LogContext.PushProperty("ServiceName", "TaskService"))
{
    Log.Information("This message will be logged with TaskService context");
}

using (LogContext.PushProperty("ServiceName", "TeamService"))
{
    Log.Information("This message will be logged with TeamService context");
}

using (LogContext.PushProperty("ServiceName", "Taskervice"))
{
    Log.Information("This message will be logged with TaskService context");
}

using (LogContext.PushProperty("ServiceName", "ProjectService"))
{
    Log.Information("This message will be logged with ProjectService context");
}

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
