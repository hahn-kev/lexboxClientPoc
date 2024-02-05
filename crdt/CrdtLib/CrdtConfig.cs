﻿using System.Text.Json.Serialization.Metadata;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrdtLib;

public class CrdtConfig
{
    public bool EnableProjectedTables { get; set; } = false;
    public ChangeTypeListBuilder ChangeTypeListBuilder { get; } = new();
    public ObjectTypeListBuilder ObjectTypeListBuilder { get; } = new();

    public Action<JsonTypeInfo> MakeJsonTypeModifier()
    {
        return typeInfo =>
        {
            if (typeInfo.Type == typeof(IChange))
            {
                foreach (var type in ChangeTypeListBuilder.Types)
                {
                    typeInfo.PolymorphismOptions!.DerivedTypes.Add(type);
                }
            }

            if (typeInfo.Type == typeof(IObjectBase))
            {
                foreach (var type in ObjectTypeListBuilder.Types)
                {
                    typeInfo.PolymorphismOptions!.DerivedTypes.Add(type);
                }
            }
        };
    }
}

public class ChangeTypeListBuilder
{
    internal List<JsonDerivedType> Types { get; } = [];

    public ChangeTypeListBuilder Add<TDerived>() where TDerived : IChange, IPolyType
    {
        Types.Add(new JsonDerivedType(typeof(TDerived), TDerived.TypeName));
        return this;
    }
}

public class ObjectTypeListBuilder
{
    internal List<JsonDerivedType> Types { get; } = [];

    internal List<Action<ModelBuilder, CrdtConfig>> ModelConfigurations { get; } = [];

    public ObjectTypeListBuilder AddDbModelConfig(Action<ModelBuilder> modelConfiguration)
    {
        ModelConfigurations.Add((builder, _) => modelConfiguration(builder));
        return this;
    }

    public ObjectTypeListBuilder Add<TDerived>(Action<EntityTypeBuilder<TDerived>>? configureDb = null)
        where TDerived : class, IObjectBase
    {
        Types.Add(new JsonDerivedType(typeof(TDerived), TDerived.TypeName));
        ModelConfigurations.Add((builder, config) =>
        {
            if (!config.EnableProjectedTables) return;
            var baseType = typeof(TDerived).BaseType;
            if (baseType is not null)
                builder.Ignore(baseType);
            var entity = builder.Entity<TDerived>();
            entity.HasBaseType((Type)null!);
            entity.HasKey(e => e.Id);
            entity.HasOne<ObjectSnapshot>()
                .WithOne()
                .HasForeignKey<TDerived>("SnapshotId")
                .IsRequired();

            entity.Property(e => e.DeletedAt);
            entity.Ignore(e => e.TypeName);
            configureDb?.Invoke(entity);
        });
        return this;
    }
}