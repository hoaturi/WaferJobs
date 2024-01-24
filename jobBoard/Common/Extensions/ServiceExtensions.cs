using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JobBoard;

public static class ServiceExtensions
{
    // Registers DbContexts
    public static IServiceCollection AddDbContexts(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var dbOptions = configuration
            .GetSection(DbConnectionOptions.Key)
            .Get<DbConnectionOptions>()!;

        services.AddDbContext<AppDbContext>(option => option.UseNpgsql(dbOptions.JobBoardApiDb));

        return services;
    }

    public static IServiceCollection AddRedisCache(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var redisOptions = configuration.GetSection(RedisOptions.Key).Get<RedisOptions>()!;

        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = redisOptions.ConnectionString;
        });

        return services;
    }

    // Registers Authentications and Authorization
    public static IServiceCollection AddAuthentications(
        this IServiceCollection services,
        IConfiguration configuration
    )
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
                    ),
                };
            });

        services.AddAuthorization(options =>
        {
            RolePolicy.AddRolePolicy(options, RoleTypes.Admin.ToString());
            RolePolicy.AddRolePolicy(options, RoleTypes.JobSeeker.ToString());
            RolePolicy.AddRolePolicy(options, RoleTypes.Business.ToString());
        });

        services
            .AddIdentityCore<ApplicationUser>(option =>
            {
                option.Password.RequireDigit = true;
                option.Password.RequiredLength = 6;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireLowercase = false;

                option.User.RequireUniqueEmail = true;

                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    // Registers Infrastructure Services
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddTransient<IJwtService, JwtService>();
        services.AddTransient<ICurrentUserService, CurrentUserService>();

        return services;
    }

    //Registers Mediatr and pipeline behaviors
    public static IServiceCollection AddMediatrAndBehaviors(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        return services;
    }

    // Registers Middlewares
    public static IServiceCollection AddMiddleWares(this IServiceCollection services)
    {
        services.AddTransient<ExceptionHandlingMiddleware>();

        return services;
    }

    public static IServiceCollection AddConfigOptions(
        this IServiceCollection services,
        IConfiguration configuration
    )
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
            .Bind(configuration.GetSection(EmailOptions.key))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<RedisOptions>()
            .Bind(configuration.GetSection(RedisOptions.Key))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
