using CrdtLib;
using CrdtLib.Changes;
using CrdtLib.Db;
using LcmCrdtModel.Changes;
using lexboxClientContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LcmCrdtModel;

public static class LcmCrdtKernel
{
    public static IServiceCollection AddLcmCrdtClient(this IServiceCollection services, string dbPath)
    {
        services.AddCrdtData(
            builder => builder.UseSqlite($"Data Source={dbPath}"),
            config =>
            {
                config.ObjectTypeListBuilder.AddDbModelConfig(builder =>
                    {
                        builder.Owned<MultiString>();
                    })
                    .Add<Entry>(builder =>
                    {
                        builder.OwnsOne(e => e.Note, n => n.ToJson());
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