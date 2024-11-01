using System.Text;
using System.Text.Json;

namespace WorkflowApi;

public static class Extensions
{
    /// <summary>
    /// Configures the specified options using the configuration section with the same name as the options type.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <typeparam name="TOptions">The type of options to configure.</typeparam>
    /// <returns>The web application builder.</returns>
    public static  WebApplicationBuilder Configure<TOptions>(this WebApplicationBuilder builder) where TOptions : class
    {
        var optionsName = typeof(TOptions).Name;
        builder.Services.Configure<TOptions>(builder.Configuration.GetSection(optionsName));
        return builder;
    }

    /// <summary>
    /// Adds the specified object to the configuration.
    /// </summary>
    /// <param name="manager">The configuration manager.</param>
    /// <param name="obj">The object to add to the configuration.</param>
    /// <returns>The configuration builder.</returns>
    public static IConfigurationBuilder AddObject(this ConfigurationManager manager, object obj)
    {
        var json = JsonSerializer.Serialize(obj);
        var configuration = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json))).Build();
        return manager.AddConfiguration(configuration);
    }
}