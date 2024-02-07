using System.Text.Json;
using CrdtLib;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using LcmCrdtModel.Changes;
using lexboxClientContracts;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using LinqToDB.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LcmCrdtModel;

public static class LcmCrdtKernel
{
    public static IServiceCollection AddLcmCrdtClient(this IServiceCollection services, string dbPath)
    {
        LinqToDBForEFTools.Initialize();
        
        //attemping to allow us to not have to manually use Json.Value in our code
        // LinqToDB.Linq.Expressions.MapMember((MultiString multiString, string ws) => multiString.Values[ws], (multiString, ws) => Json.Value(multiString, ms => ms.Values[ws]));
        // LinqToDB.Linq.Expressions.MapMember((MultiString multiString, string ws) => multiString.Values[(WritingSystemId)ws], (multiString, ws) => Json.Value(multiString, ms => ms.Values[(WritingSystemId)ws]));
        LinqToDB.Linq.Expressions.MapMember((MultiString multiString, WritingSystemId ws) => multiString.Values[ws], (multiString, ws) => Json.Value(multiString, ms => ms.Values[ws]));
        services.AddCrdtData(
            builder => builder.UseSqlite($"Data Source={dbPath}").UseLinqToDB(optionsBuilder =>
            {
                var mappingSchema = new MappingSchema();
                mappingSchema.SetConvertExpression((WritingSystemId id) => new DataParameter
                {
                    Value = id.Code,
                    DataType = DataType.Text
                });
                optionsBuilder.AddMappingSchema(mappingSchema);
            }),
            config =>
            {
                config.EnableProjectedTables = true;
                config.ObjectTypeListBuilder.AddDbModelConfig(builder =>
                    {
                        // builder.Owned<MultiString>();
                    })
                    .AddDbModelConvention(builder =>
                    {
                        builder.Properties<MultiString>()
                            .HaveColumnType("jsonb")
                            .HaveConversion<MultiStringDbConverter>();
                    })
                    .Add<Entry>(builder =>
                    {
                        // builder.OwnsOne(e => e.Note, n => n.ToJson());
                        // builder.OwnsOne(e => e.LexemeForm, n => n.ToJson());
                        // builder.OwnsOne(e => e.CitationForm, n => n.ToJson());
                        // builder.OwnsOne(e => e.LiteralMeaning, n => n.ToJson());
                    })
                    .Add<Sense>(builder =>
                    {
                        builder.HasOne<Entry>()
                            .WithMany()
                            .HasForeignKey(sense => sense.EntryId);
                    })
                    .Add<ExampleSentence>(builder =>
                    {
                        builder.HasOne<Sense>()
                            .WithMany()
                            .HasForeignKey(e => e.SenseId);
                    });

                config.ChangeTypeListBuilder.Add<JsonPatchChange<Entry>>()
                    .Add<JsonPatchChange<Sense>>()
                    .Add<JsonPatchChange<ExampleSentence>>()
                    .Add<DeleteChange<Entry>>()
                    .Add<DeleteChange<Sense>>()
                    .Add<DeleteChange<ExampleSentence>>()
                    .Add<CreateEntryChange>()
                    .Add<CreateSenseChange>()
                    .Add<CreateExampleSentenceChange>();
            }
        );
        services.AddSingleton<ILexboxApi, CrdtLexboxApi>();
        services.AddSingleton<IHostedService, StartupService>();
        return services;
    }

    private class MultiStringDbConverter() : ValueConverter<MultiString, string>(
        mul => JsonSerializer.Serialize(mul, (JsonSerializerOptions?)null),
        json => JsonSerializer.Deserialize<MultiString>(json, (JsonSerializerOptions?)null) ?? new());

    private class StartupService(CrdtDbContext dbContext, ILexboxApi lexboxApi) : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //todo use migrations before releasing
            // await dbContext.Database.MigrateAsync(cancellationToken);
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            await lexboxApi.CreateEntry(new()
            {
                LexemeForm =
                {
                    Values =
                    {
                        { "en", "Kevin" }
                    }
                },
                Note =
                {
                    Values =
                    {
                        { "en", "this is a test note from Kevin" }
                    }
                },
                CitationForm =
                {
                    Values =
                    {
                        { "en", "Kevin" }
                    }
                },
                LiteralMeaning =
                {
                    Values =
                    {
                        { "en", "Kevin" }
                    }
                },
                Senses =
                [
                    new()
                    {
                        Gloss =
                        {
                            Values =
                            {
                                { "en", "Kevin" }
                            }
                        },
                        Definition =
                        {
                            Values =
                            {
                                { "en", "Kevin" }
                            }
                        },
                        SemanticDomain = ["Person"],
                        ExampleSentences =
                        [
                            new()
                            {
                                Sentence =
                                {
                                    Values =
                                    {
                                        { "en", "Kevin is a good guy" }
                                    }
                                }
                            }
                        ]
                    }
                ]
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}