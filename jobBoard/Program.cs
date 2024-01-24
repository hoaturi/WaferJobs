using FluentValidation;
using JobBoard;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureServices();
builder.Services.AddConfigOptions(configuration);
builder.Services.AddDbContexts(configuration);
builder.Services.AddRedisCache(configuration);
builder.Services.AddAuthentications(configuration);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddMediatrAndBehaviors();
builder.Services.AddMiddleWares();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapGet("/", () => "Hello World!").RequireAuthorization(RolePolicy.JobSeeker);

app.MapControllers();

app.Run();
