using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CrdtLib;

public static class CrdtKernel
{
    public static IServiceCollection AddCrdtData(this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureOptions,
        Action<CrdtConfig> configureCrdt)
    {
        services.AddOptions<CrdtConfig>().Configure(configureCrdt);
        services.AddSingleton(sp => new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    sp.GetRequiredService<IOptions<CrdtConfig>>().Value.MakeJsonTypeModifier()
                }
            }
        });
        services.AddDbContext<CrdtDbContext>((provider, builder) =>
            {
                configureOptions(builder);
                builder
                    .AddInterceptors(provider.GetServices<IInterceptor>().ToArray())
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging();
            },
            ServiceLifetime.Singleton);
        services.AddSingleton<CrdtRepository>();
        services.AddSingleton<DataModel>();
        return services;
    }
}