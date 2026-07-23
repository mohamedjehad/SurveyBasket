using Asp.Versioning;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Authentication.Filters;
using SurveyBasket.Api.Health;
using SurveyBasket.Api.Settings;
using System.Text;
using System.Threading.RateLimiting;

namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddHybridCache();

        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        options.AddDefaultPolicy(builder =>
        builder
        .WithOrigins(allowedOrigins!)
        .AllowAnyMethod()
        .AllowAnyHeader())
        );

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailSender, EmailService>();
        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IResultService, ResultService>();


        services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>(name:"database")
                .AddHangfire(options => { options.MinimumAvailableServers = 1; })
                .AddCheck<MailProviderHealthCheck>(name:"mail service");

        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;


        rateLimiterOptions.AddPolicy("ipLimit", httpContext =>
                 RateLimitPartition.GetFixedWindowLimiter(
                 partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                 factory: _ => new FixedWindowRateLimiterOptions
                 {
                     PermitLimit = 2,
                     Window = TimeSpan.FromSeconds(20)
                 }));

        rateLimiterOptions.AddPolicy("userLimit", httpContext =>
                 RateLimitPartition.GetFixedWindowLimiter(
                 partitionKey: httpContext.User.GetUserId(),
                 factory: _ => new FixedWindowRateLimiterOptions
                 {
                     PermitLimit = 2,
                     Window = TimeSpan.FromSeconds(20)
                 }));


            rateLimiterOptions.AddConcurrencyLimiter("concurrency", options =>
            {
                options.PermitLimit = 1000;
                options.PermitLimit = 100;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            //rateLimiterOptions.AddTokenBucketLimiter("token", options =>
            //{
            //    options.TokenLimit = 10;
            //    options.QueueLimit = 1;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //    options.ReplenishmentPeriod = TimeSpan.FromSeconds(120);
            //    options.TokensPerPeriod = 2;
            //    options.AutoReplenishment = true;
            //});

            //rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
            //{
            //    options.PermitLimit = 10;
            //    options.QueueLimit = 5;
            //    options.Window = TimeSpan.FromSeconds(60);
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});

            //rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
            //{
            //    options.PermitLimit = 10;
            //    options.QueueLimit = 5;
            //    options.Window = TimeSpan.FromSeconds(60);
            //    options.SegmentsPerWindow = 6;
            //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});

        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHttpContextAccessor();
        services.AddMapsterConfig();
        services.AddAuthConfig(configuration);
        services.AddBackgroundJobsConfig(configuration);
        services.AddFluentValidationConfig();
        services.AddDatabaseConfig(configuration);

        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }
    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation().
                        AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

    private static IServiceCollection AddDatabaseConfig(this IServiceCollection services,IConfiguration configuration)
    {
        var connectionString = configuration
         .GetConnectionString("DefaultConnection") ??
         throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

        services.AddDbContext<ApplicationDbContext>
                    (options => options.UseSqlServer(connectionString));
        return services;
    }
    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();

        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddTransient<IAuthorizationHandler,PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider,PermissionPolicyProvider>();

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        var jwtSettings= configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }
        ).AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                ValidAudience =jwtSettings?.Audience,
                ValidIssuer = jwtSettings?.Issuer
            };
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }
    private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
         .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
         .UseSimpleAssemblyNameTypeSerializer()
         .UseRecommendedSerializerSettings()
         .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

        services.AddHangfireServer();
        return services;
    }
}
