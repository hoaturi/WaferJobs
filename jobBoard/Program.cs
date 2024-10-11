using FluentValidation;
using JobBoard.Infrastructure.Extensions;
using JobBoard.Infrastructure.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add core services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Add validation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.ConfigureApiBehaviorOptions();

// Add database and caching
builder.Services.AddDbContexts(configuration);
builder.Services.AddRedisCache(configuration);

// Add authentication and authorization
builder.Services.AddAuthentications(configuration);

// Add infrastructure services
builder.Services.AddCorsPolicy(configuration);
builder.Services.AddOutputCaches();
builder.Services.AddHangfire(configuration);
builder.Services.AddSendGrid(configuration);
builder.Services.AddTypedHttpClient(configuration);

// Add application services
builder.Services.AddInfrastructureServices();
builder.Services.AddMediatrAndBehaviors();
builder.Services.AddMiddleWares();

// Add configuration options
builder.Services.AddConfigOptions(configuration);

// Configure logging
builder.Host.UseLogging();

var app = builder.Build();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(CorsOptions.PolicyName);
app.UseMiddleWares();
app.UseHangfireJobs();

app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();

app.MapControllers();

app.Run();