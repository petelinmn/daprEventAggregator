using System.Threading.Tasks;
using DaprEventAggregatorPlugin;
using Newtonsoft.Json;

namespace StereotypeRecognizer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Startup class.
/// </summary>
public class Startup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">Configuration.</param>
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    private IConfiguration Configuration { get; }

    /// <summary>
    /// Configures Services.
    /// </summary>
    /// <param name="services">Service Collection.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddDapr();
        //services.AddActors(options =>
        //{
        //    options.Actors.RegisterActor<EvantAggregatorConfigurationActor>();
        //});
    }

    /// <summary>
    /// Configures Application Builder and WebHost environment.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <param name="env">Webhost environment.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseCloudEvents();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapEASubscribeHandler(GetPubsubConfiguration).Wait();
            endpoints.MapControllers();
        });
    }

    private static async Task<Dictionary<string, List<string>>> GetPubsubConfiguration()
    {
        var configuration = await ConfigurationReader.Read();

        var result = new Dictionary<string, List<string>>();
        foreach (var pubsub in configuration.PubSubs)
        {
            result.Add(pubsub.Name,
                pubsub.Stereotypes
                    .SelectMany(e => e.Events)
                    .Distinct()
                    .ToList());
        }

        return result;
    }
}