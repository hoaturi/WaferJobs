using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using JobBoard.Common.Behaviors;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;
using JobBoard.Common.Middlewares;
using JobBoard.Common.Options;
using JobBoard.Common.Security;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Persistence.Utils;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using JobBoard.Infrastructure.Services.FileUploadService;
using JobBoard.Infrastructure.Services.JwtService;
using JobBoard.Infrastructure.Services.LookupServices.JobPostCountService;
using JobBoard.Infrastructure.Services.LookupServices.LocationService;
using JobBoard.Infrastructure.Services.LookupServices.PopularKeywordsService;
using JobBoard.Infrastructure.Services.PaymentService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Extensions.DependencyInjection;
using Serilog;

namespace JobBoard.Infrastructure.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddCorsPolicy(configuration)
            .ConfigureApiBehaviorOptions()
            .AddDbContexts(configuration)
            .AddRedisCache(configuration)
            .AddAuthentications(configuration)
            .AddHangfire(configuration)
            .AddSendGrid(configuration)
            .AddInfrastructureServices()
            .AddMediatrAndBehaviors()
            .AddMiddleWares()
            .AddConfigOptions(configuration);

        return services;
    }

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(CorsOptions.Key).Get<CorsOptions>()!;

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    private static IServiceCollection ConfigureApiBehaviorOptions(this IServiceCollection services)
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

        return services;
    }

    private static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = configuration.GetSection(DbConnectionOptions.Key).Get<DbConnectionOptions>()!;
        services.AddDbContext<AppDbContext>(option => option.UseNpgsql(dbOptions.JobBoardApiDb));
        return services;
    }

    private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOptions = configuration.GetSection(RedisOptions.Key).Get<RedisOptions>()!;
        services.AddStackExchangeRedisCache(option => option.Configuration = redisOptions.ConnectionString);
        return services;
    }

    private static IServiceCollection AddAuthentications(this IServiceCollection services, IConfiguration configuration)
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
        });

        services.AddIdentityCore<ApplicationUserEntity>(ConfigureIdentityOptions)
            .AddRoles<ApplicationRoleEntity>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
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

    private static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = configuration.GetSection(DbConnectionOptions.Key).Get<DbConnectionOptions>()!;

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(dbOptions.JobBoardApiDb)));

        services.AddHangfireServer();
        return services;
    }

    private static IServiceCollection AddSendGrid(this IServiceCollection services, IConfiguration configuration)
    {
        var sendGridOptions = configuration.GetSection(SendGridOptions.Key).Get<SendGridOptions>()!;
        services.AddSendGrid(options => options.ApiKey = sendGridOptions.ApiKey);
        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IPaymentService, PaymentService>();
        services.AddSingleton<IFileUploadService, FileUploadService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IPopularKeywordsService, PopularKeywordsService>();
        services.AddScoped<IJobPostCountService, JobPostCountService>();

        services.AddScoped<EntityConstraintChecker>();
        return services;
    }

    private static IServiceCollection AddMediatrAndBehaviors(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
        });
        return services;
    }

    private static IServiceCollection AddMiddleWares(this IServiceCollection services)
    {
        services.AddSingleton<ExceptionHandlingMiddleware>();
        return services;
    }

    public static IHostBuilder UseSerilogWithSeq(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(context.Configuration));
    }

    private static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<JwtOptions>().Bind(configuration.GetSection(JwtOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<DbConnectionOptions>()
            .Bind(configuration.GetSection(DbConnectionOptions.Key))
            .ValidateDataAnnotations();

        services.AddOptionsWithValidateOnStart<AzureOptions>().Bind(configuration.GetSection(AzureOptions.Key))
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

        return services;
    }
}