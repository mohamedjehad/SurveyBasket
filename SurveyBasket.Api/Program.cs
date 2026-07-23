using Hangfire;
using Hangfire.Dashboard;
using HangfireBasicAuthenticationFilter;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Serilog;
using SurveyBasket.Api;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context,configuration)=>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    Authorization =
    [
        new HangfireCustomBasicAuthenticationFilter
        {
        User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
        Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
        }

    ],
    //IsReadOnlyFunc = (DashboardContext context) => true
});


RecurringJob.AddOrUpdate<INotificationService>
    ("SendNewPollsNotification", x => x.SendNewPollsNotification(null), Cron.Daily());

app.MapControllers();

app.UseExceptionHandler();
app.MapHealthChecks("health",
    new HealthCheckOptions
    {
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }
);

app.Run();
