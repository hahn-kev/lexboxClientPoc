using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace CrdtLib;

public static class CrdtKernel
{
    public static IServiceCollection AddCrdtData(this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureOptions,
        ChangeTypeListBuilder changeTypes,
        ObjectTypeListBuilder objectTypes)
    {
        services.AddSingleton(new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    MakeModifier(changeTypes, objectTypes)
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

    public static Action<JsonTypeInfo> MakeModifier(
        this (ChangeTypeListBuilder changeTypes, ObjectTypeListBuilder objectTypes) types)
    {
        return MakeModifier(types.changeTypes, types.objectTypes);
    }

    public static Action<JsonTypeInfo> MakeModifier(
        ChangeTypeListBuilder changeTypes,
        ObjectTypeListBuilder objectTypes)
    {
        return typeInfo =>
        {
            if (typeInfo.Type == typeof(IChange))
            {
                changeTypes.AddTypes(typeInfo.PolymorphismOptions!);
            }

            if (typeInfo.Type == typeof(IObjectBase))
            {
                objectTypes.AddTypes(typeInfo.PolymorphismOptions!);
            }
        };
    }

    public class ChangeTypeListBuilder
    {
        private readonly List<JsonDerivedType> _types = [];

        public ChangeTypeListBuilder Add<TDerived>() where TDerived : IChange, IPolyType
        {
            _types.Add(new JsonDerivedType(typeof(TDerived), TDerived.TypeName));
            return this;
        }

        internal void AddTypes(JsonPolymorphismOptions options)
        {
            foreach (var type in _types)
            {
                options.DerivedTypes.Add(type);
            }
        }
    }

    public class ObjectTypeListBuilder
    {
        private readonly List<JsonDerivedType> _types = [];

        public ObjectTypeListBuilder Add<TDerived>() where TDerived : IObjectBase
        {
            _types.Add(new JsonDerivedType(typeof(TDerived), TDerived.TypeName));
            return this;
        }

        internal void AddTypes(JsonPolymorphismOptions options)
        {
            foreach (var type in _types)
            {
                options.DerivedTypes.Add(type);
            }
        }
    }
}