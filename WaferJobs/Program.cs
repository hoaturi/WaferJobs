using FluentValidation;
using WaferJobs.Infrastructure.Extensions;
using WaferJobs.Infrastructure.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.ConfigureApiBehaviorOptions();

builder.Services.AddDbContexts(configuration);
builder.Services.AddRedisCache(configuration);

builder.Services.AddAuthentications(configuration);

builder.Services.AddCorsPolicy(configuration);
builder.Services.AddOutputCaches();
builder.Services.AddHangfire(configuration);
builder.Services.AddSendGrid(configuration);
builder.Services.AddTypedHttpClient(configuration);

builder.Services.AddInfrastructureServices();
builder.Services.AddMediatrAndBehaviors();
builder.Services.AddMiddleWares();

builder.Services.AddConfigOptions(configuration);

builder.Host.UseLogging();

builder.Services.AddApplicationInsights();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(CorsOptions.PolicyName);
app.UseMiddleWares();
app.UseHangfireJobs();

app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();

app.MapControllers();

app.Run();