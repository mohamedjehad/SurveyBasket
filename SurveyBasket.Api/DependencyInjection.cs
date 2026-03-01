namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();

        services.AddScoped<IPollService, PollService>();

        AddMapsterConfig(services);

        AddFluentValidationConfig(services);
        AddDatabaseConfig(services, configuration);


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
}
