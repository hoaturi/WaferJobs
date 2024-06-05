using System.Text;
using JobBoard.Common.Behaviors;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;
using JobBoard.Common.Interfaces;
using JobBoard.Common.Middlewares;
using JobBoard.Common.Options;
using JobBoard.Common.Security;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using JobBoard.Infrastructure.Services.FileUploadService;
using JobBoard.Infrastructure.Services.JwtService;
using JobBoard.Infrastructure.Services.PaymentService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace JobBoard.Common.Extensions;

public static class ServiceExtensions
{
    public static void AddCorsPolicy(this IServiceCollection services,
        IConfiguration configuration)
    {
        var corsOptions = configuration.GetSection(CorsOptions.Key).Get<CorsOptions>()!;

        services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                builder =>
                {
                    builder
                        .WithOrigins(corsOptions.AllowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errorMessages = context.ModelState
                    .Where(x => x.Value is { Errors.Count: > 0 })
                    .SelectMany(x => x.Value!.Errors.Select(e => new
                    {
                        Field = x.Key,
                        Message = e.ErrorMessage
                    }))
                    .ToList();

                throw new CustomValidationException(
                    errorMessages.Select(x => new ValidationError(x.Field, x.Message)).ToList()
                );
            };
        });
    }

// Registers DbContexts
    public static void AddDbContexts(this IServiceCollection services,
        IConfiguration configuration)
    {
        var dbOptions = configuration
            .GetSection(DbConnectionOptions.Key)
            .Get<DbConnectionOptions>()!;

        services.AddDbContext<AppDbContext>(option => { option.UseNpgsql(dbOptions.JobBoardApiDb); });
    }

    public static void AddRedisCache(this IServiceCollection services,
        IConfiguration configuration)
    {
        var redisOptions = configuration.GetSection(RedisOptions.Key).Get<RedisOptions>()!;

        services.AddStackExchangeRedisCache(option => { option.Configuration = redisOptions.ConnectionString; });
    }


// Registers Authentications and Authorization
    public static void AddAuthentications(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(option =>
            {
                option.MapInboundClaims = false;
                var jwtOptions = configuration.GetSection(JwtOptions.Key).Get<JwtOptions>()!;

                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.AccessKey)
                    )
                };
            });

        services.AddAuthorization(options =>
        {
            RolePolicy.AddRolePolicy(options, nameof(UserRoles.JobSeeker));
            RolePolicy.AddRolePolicy(options, nameof(UserRoles.Business));
        });

        services
            .AddIdentityCore<ApplicationUserEntity>(option =>
            {
                option.Password.RequireDigit = true;
                option.Password.RequiredLength = 6;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;

                option.User.RequireUniqueEmail = true;

                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddRoles<ApplicationRoleEntity>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }

// Registers Infrastructure Services
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IEmailService, EmailService>();
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IPaymentService, PaymentService>();
        services.AddSingleton<IFileUploadService, FileUploadService>();
    }

//Registers Mediatr and pipeline behaviors
    public static void AddMediatrAndBehaviors(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
        });
    }

// Registers Middlewares
    public static void AddMiddleWares(this IServiceCollection services)
    {
        services.AddSingleton<ExceptionHandlingMiddleware>();
    }

    public static void UseSerilogWithSeq(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog(
            (context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration)
        );
    }

    public static void AddConfigOptions(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<DbConnectionOptions>()
            .Bind(configuration.GetSection(DbConnectionOptions.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<AzureOptions>()
            .Bind(configuration.GetSection(AzureOptions.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<EmailOptions>()
            .Bind(configuration.GetSection(EmailOptions.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<RedisOptions>()
            .Bind(configuration.GetSection(RedisOptions.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<StripeOptions>()
            .Bind(configuration.GetSection(StripeOptions.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<CorsOptions>()
            .Bind(configuration.GetSection(CorsOptions.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}