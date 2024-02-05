using Microsoft.Extensions.DependencyInjection;

namespace HtmlGenerator;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddHtmlGeneratorServices(this IServiceCollection services)
    {
        services.AddTransient<IGenerator, Generator>();
        return services;
    }
}
