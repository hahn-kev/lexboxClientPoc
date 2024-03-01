using System.Runtime.Serialization;
using System.Text.Json;
using CrdtLib.Changes;
using CrdtLib.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CrdtLib.Db;

public class CrdtDbContext(
    DbContextOptions<CrdtDbContext> options,
    IOptions<CrdtConfig> crdtConfig,
    JsonSerializerOptions jsonSerializerOptions)
    : DbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        foreach (var modelConvention in crdtConfig.Value.ObjectTypeListBuilder.ModelConventions)
        {
            modelConvention.Invoke(configurationBuilder);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var commitEntity = builder.Entity<Commit>();
        commitEntity.HasKey(c => c.Id);
        commitEntity.Property(c => (DateTimeOffset?)c.DateTime).HasConversion(
            d => d.Value.ToUniversalTime().DateTime,
            d => new DateTimeOffset(d, TimeSpan.Zero)
        );
        commitEntity.HasMany(c => c.ChangeEntities)
            .WithOne()
            .HasForeignKey(c => c.CommitId);
        var snapshotObject = builder.Entity<ObjectSnapshot>();
        snapshotObject.HasKey(s => s.Id);
        snapshotObject
            .HasOne(s => s.Commit)
            .WithMany(c => c.Snapshots)
            .HasForeignKey(s => s.CommitId);
        snapshotObject.Property(s => s.Entity)
            .HasColumnType("jsonb")
            .HasConversion(
                entry =>  JsonSerializer.Serialize(entry, jsonSerializerOptions),
                json => DeserializeObject(json)
            );
        var changeEntity = builder.Entity<ChangeEntity>();
        changeEntity.HasKey(c => c.Id);
        changeEntity.Property(c => c.Change)
            .HasColumnType("jsonb")
            .HasConversion(
                change => JsonSerializer.Serialize(change, jsonSerializerOptions),
                json => DeserializeChange(json)
            );

        foreach (var modelConfiguration in crdtConfig.Value.ObjectTypeListBuilder.ModelConfigurations)
        {
            modelConfiguration(builder, crdtConfig.Value);
        }
    }

    private IChange DeserializeChange(string json)
    {
        return JsonSerializer.Deserialize<IChange>(json, jsonSerializerOptions) ??
               throw new SerializationException("Could not deserialize Change: " + json);
    }

    private IObjectBase DeserializeObject(string json)
    {
        return JsonSerializer.Deserialize<IObjectBase>(json, jsonSerializerOptions) ??
               throw new SerializationException("Could not deserialize Entry: " + json);
    }

    public DbSet<Commit> Commits { get; set; } = null!;
    public DbSet<ChangeEntity> ChangeEntities { get; set; } = null!;
    public DbSet<ObjectSnapshot> Snapshots { get; set; } = null!;
}

public static class DbSetExtensions
{
    public static IQueryable<Commit> DefaultOrder(this IQueryable<Commit> queryable)
    {
        return queryable.OrderBy(c => c.DateTime).ThenBy(c => c.Id);
    }

    public static IQueryable<ObjectSnapshot> DefaultOrder(this IQueryable<ObjectSnapshot> queryable)
    {
        return queryable.OrderBy(c => c.Commit.DateTime).ThenBy(c => c.Commit.Id);
    }

    public static IQueryable<ObjectSnapshot> LatestSnapshots(this IQueryable<ObjectSnapshot> queryable)
    {
        return queryable.Where(snapshot =>
            queryable.GroupBy(s => s.EntityId,
                    (entityId, snapshots) => snapshots.OrderByDescending(s => s.Commit.DateTime)
                        .ThenBy(s => s.CommitId)
                        .First().Id)
                .Contains(snapshot.Id));
    }
}