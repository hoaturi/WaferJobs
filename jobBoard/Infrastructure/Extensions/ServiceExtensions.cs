using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using JobBoard.Common.Behaviors;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;
using JobBoard.Common.Middlewares;
using JobBoard.Common.Security;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.BackgroundJobs.CurrencyRateRefreshJob;
using JobBoard.Infrastructure.Options;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Persistence.Utils;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.DomainValidationService;
using JobBoard.Infrastructure.Services.EmailService;
using JobBoard.Infrastructure.Services.FileUploadService;
using JobBoard.Infrastructure.Services.JobMetricService;
using JobBoard.Infrastructure.Services.JwtService;
using JobBoard.Infrastructure.Services.PaymentService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;

namespace JobBoard.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(CorsOptions.Key).Get<CorsOptions>()!;

        services.AddCors(options =>
        {
            options.AddPolicy(CorsOptions.PolicyName, builder =>
            {
                builder.WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errorMessages = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => new { Field = x.Key, Message = e.ErrorMessage }))
                    .ToList();

                throw new CustomValidationException(
                    errorMessages.Select(x => new ValidationError(x.Field, x.Message)).ToList()
                );
            };
        });
    }

    public static void AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = configuration.GetSection(DbOptions.Key).Get<DbOptions>()!;
        services.AddDbContext<AppDbContext>(option => option.UseNpgsql(dbOptions.WaferJobsDb));
    }

    public static void AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOptions = configuration.GetSection(RedisOptions.Key).Get<RedisOptions>()!;

        var connectionString = $"{redisOptions.Host}:{redisOptions.Port},password={redisOptions.Password}";
        IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);

        services.AddSingleton(connectionMultiplexer);

        services.AddStackExchangeRedisCache(option =>
        {
            option.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer);
        });
    }

    public static void AddAuthentications(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.Key).Get<JwtOptions>()!;

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(option =>
            {
                option.MapInboundClaims = false;
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.AccessKey))
                };
            });

        services.AddAuthorization(options =>
        {
            RolePolicy.AddRolePolicy(options, nameof(UserRoles.JobSeeker));
            RolePolicy.AddRolePolicy(options, nameof(UserRoles.Business));
            RolePolicy.AddRolePolicy(options, nameof(UserRoles.Admin));
        });

        services.AddIdentityCore<ApplicationUserEntity>(ConfigureIdentityOptions)
            .AddRoles<ApplicationRoleEntity>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddOutputCaches(this IServiceCollection services)
    {
        services.AddOutputCache(options =>
        {
            foreach (OutputCacheKeys cacheKey in Enum.GetValues(typeof(OutputCacheKeys)))
            {
                var policyName = cacheKey.ToString();
                var minutes = (int)cacheKey;

                options.AddPolicy(policyName, builder => builder.Expire(TimeSpan.FromMinutes(minutes)));
            }
        });
    }

    private static void ConfigureIdentityOptions(IdentityOptions options)
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    }

    public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = configuration.GetSection(DbOptions.Key).Get<DbOptions>()!;

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(dbOptions.WaferJobsDb)));

        services.AddHangfireServer();
    }

    public static void AddSendGrid(this IServiceCollection services, IConfiguration configuration)
    {
        var sendGridOptions = configuration.GetSection(SendGridOptions.Key).Get<SendGridOptions>()!;
        services.AddSendGrid(options => options.ApiKey = sendGridOptions.ApiKey);
    }

    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IPaymentService, PaymentService>();
        services.AddSingleton<IFileUploadService, FileUploadService>();
        services.AddSingleton<IDomainValidationService, DomainValidationService>();

        services.AddScoped<IJobMetricService, JobMetricService>();

        services.AddScoped<IEntityConstraintChecker, EntityConstraintChecker>();
    }

    public static void AddTypedHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        var currencyOptions = configuration.GetSection(CurrencyOptions.Key).Get<CurrencyOptions>()!;

        services.AddHttpClient<CurrencyRateRefreshJob>()
            .ConfigureHttpClient(client => { client.BaseAddress = new Uri(currencyOptions.BaseUrl); });
    }

    public static void AddMediatrAndBehaviors(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
        });
    }

    public static void AddMiddleWares(this IServiceCollection services)
    {
        services.AddSingleton<ExceptionHandlingMiddleware>();
    }

    public static void UseLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, loggerConfig) =>
        {
            loggerConfig.ReadFrom.Configuration(context.Configuration);
        });
    }

    public static void AddConfigOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<JwtOptions>().Bind(configuration.GetSection(JwtOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<DbOptions>()
            .Bind(configuration.GetSection(DbOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<CloudFlareOptions>()
            .Bind(configuration.GetSection(CloudFlareOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<EmailOptions>().Bind(configuration.GetSection(EmailOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<RedisOptions>().Bind(configuration.GetSection(RedisOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<StripeOptions>().Bind(configuration.GetSection(StripeOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<CorsOptions>().Bind(configuration.GetSection(CorsOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<SendGridOptions>().Bind(configuration.GetSection(SendGridOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<CurrencyOptions>().Bind(configuration.GetSection(CurrencyOptions.Key))
            .ValidateDataAnnotations();
    }
}