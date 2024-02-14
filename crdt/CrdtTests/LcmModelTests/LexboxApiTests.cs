using System.Text.Json;
using CrdtLib;
using CrdtLib.Db;
using LcmCrdtModel;
using lexboxClientContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Entry = lexboxClientContracts.Entry;
using ExampleSentence = lexboxClientContracts.ExampleSentence;
using Sense = lexboxClientContracts.Sense;

namespace Tests.LcmModelTests;

public class BasicApiTests: IAsyncLifetime
{
    private CrdtLexboxApi _api;
    private Guid _entry1Id = new Guid("a3f5aa5a-578f-4181-8f38-eaaf27f01f1c");

    protected readonly ServiceProvider _services;
    public readonly DataModel DataModel;
    private CrdtDbContext _crdtDbContext;

    public BasicApiTests()
    {
        _services = new ServiceCollection()
            .AddLcmCrdtClient(":memory:")
            .BuildServiceProvider();
        _crdtDbContext = _services.GetRequiredService<CrdtDbContext>();
        DataModel = _services.GetRequiredService<DataModel>();
        _api = new CrdtLexboxApi(DataModel, _services.GetRequiredService<JsonSerializerOptions>());
    }

    public virtual async Task InitializeAsync()
    {
        await _crdtDbContext.Database.OpenConnectionAsync();
        await _crdtDbContext.Database.EnsureCreatedAsync();
        await _api.CreateEntry(new Entry
        {
            Id = _entry1Id,
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
                new Sense
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
                    ExampleSentences =
                    [
                        new ExampleSentence
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
        await _api.CreateEntry(new()
        {
            LexemeForm =
            {
                Values = { { "en", "apple" } }
            }
        });
    }

    public async Task DisposeAsync()
    {
        await _services.DisposeAsync();
    }

    [Fact]
    public async Task GetWritingSystems()
    {
        var writingSystems = await _api.GetWritingSystems();
        writingSystems.Analysis.Should().NotBeEmpty();
    }

    [Fact(Skip = "not implemented")]
    public async Task GetExemplars()
    {
        var exemplars = await _api.GetExemplars();
        exemplars.Should().NotBeEmpty();
    }

    [Fact(Skip = "not implemented")]
    public async Task GetEntries()
    {
        var entries = await _api.GetEntries("a");
        entries.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetEntriesWithOptions()
    {
        var entries = await _api.GetEntries(new QueryOptions("lexemeForm"));
        entries.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetEntriesWithoutExemplar()
    {
        var entries = await _api.GetEntries();
        entries.Should().NotBeEmpty();
        var entry = entries.First();
        entry.LexemeForm.Values.Should().NotBeEmpty();
    }

    [Fact]
    public async Task SearchEntries()
    {
        var entries = await _api.SearchEntries("a");
        entries.Should().NotBeEmpty();
        entries.Should().NotContain(e => e.LexemeForm.Values["en"] == "Kevin");
    }

    [Fact]
    public async Task GetEntry()
    {
        var entries = await _api.GetEntries();
        var entry = await _api.GetEntry(entries.First().Id);
        entry.Should().NotBeNull();
        entry.LexemeForm.Values.Should().NotBeEmpty();
        var sense = entry.Senses.Should()
            .NotBeEmpty($"because '{entry.LexemeForm.Values.First().Value}' should have a sense").And.Subject.First();
        sense.Gloss.Values.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateEntry()
    {
        var entry = await _api.CreateEntry(new Entry
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
                new Sense
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
                    ExampleSentences =
                    [
                        new ExampleSentence
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
        entry.Should().NotBeNull();
        entry.LexemeForm.Values["en"].Should().Be("Kevin");
        entry.LiteralMeaning.Values["en"].Should().Be("Kevin");
        entry.CitationForm.Values["en"].Should().Be("Kevin");
        entry.Note.Values["en"].Should().Be("this is a test note from Kevin");
        var sense = entry.Senses.Should().ContainSingle().Subject;
        sense.Gloss.Values["en"].Should().Be("Kevin");
        sense.Definition.Values["en"].Should().Be("Kevin");
        var example = sense.ExampleSentences.Should().ContainSingle().Subject;
        example.Sentence.Values["en"].Should().Be("Kevin is a good guy");
    }

    [Fact]
    public async Task UpdateEntry()
    {
        var updatedEntry = await _api.UpdateEntry(_entry1Id,
            _api.CreateUpdateBuilder<Entry>()
                .Set(e => e.LexemeForm.Values["en"], "updated")
                .Build());
        updatedEntry.LexemeForm.Values["en"].Should().Be("updated");
    }

    [Fact]
    public async Task UpdateEntryNote()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values =
                {
                    { "en", "test" }
                }
            }
        });
        var updatedEntry = await _api.UpdateEntry(entry.Id,
            _api.CreateUpdateBuilder<Entry>()
                .Set(e => e.Note.Values["en"], "updated")
                .Build());
        updatedEntry.Note.Values["en"].Should().Be("updated");
    }

    [Fact]
    public async Task UpdateSense()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values =
                {
                    { "en", "test" }
                }
            },
            Senses = new List<Sense>
            {
                new Sense()
                {
                    Definition = new MultiString
                    {
                        Values =
                        {
                            { "en", "test" }
                        }
                    }
                }
            }
        });
        var updatedSense = await _api.UpdateSense(entry.Id,
            entry.Senses[0].Id,
            _api.CreateUpdateBuilder<Sense>()
                .Set(e => e.Definition.Values["en"], "updated")
                .Build());
        updatedSense.Definition.Values["en"].Should().Be("updated");
    }

    [Fact]
    public async Task UpdateSensePartOfSpeech()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values =
                {
                    { "en", "test" }
                }
            },
            Senses = new List<Sense>
            {
                new Sense()
                {
                    PartOfSpeech = "test",
                    Definition = new MultiString
                    {
                        Values =
                        {
                            { "en", "test" }
                        }
                    }
                }
            }
        });
        var updatedSense = await _api.UpdateSense(entry.Id,
            entry.Senses[0].Id,
            _api.CreateUpdateBuilder<Sense>()
                .Set(e => e.PartOfSpeech, "updated")
                .Build());
        updatedSense.PartOfSpeech.Should().Be("updated");
    }

    [Fact]
    public async Task UpdateSenseSemanticDomain()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values =
                {
                    { "en", "test" }
                }
            },
            Senses = new List<Sense>
            {
                new Sense()
                {
                    SemanticDomain =
                    [
                        "test"
                    ],
                    Definition = new MultiString
                    {
                        Values =
                        {
                            { "en", "test" }
                        }
                    }
                }
            }
        });
        var updatedSense = await _api.UpdateSense(entry.Id,
            entry.Senses[0].Id,
            _api.CreateUpdateBuilder<Sense>()
                .Set(e => e.SemanticDomain[0], "updated")
                .Build());
        updatedSense.SemanticDomain.Should().Contain("updated");
    }

    [Fact]
    public async Task UpdateExampleSentence()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values =
                {
                    { "en", "test" }
                }
            },
            Senses = new List<Sense>
            {
                new Sense()
                {
                    Definition = new MultiString
                    {
                        Values =
                        {
                            { "en", "test" }
                        }
                    },
                    ExampleSentences = new List<ExampleSentence>
                    {
                        new ExampleSentence()
                        {
                            Sentence = new MultiString
                            {
                                Values =
                                {
                                    { "en", "test" }
                                }
                            }
                        }
                    }
                }
            }
        });
        entry.Senses.Should().ContainSingle().Which.ExampleSentences.Should().ContainSingle();
        var updatedExample = await _api.UpdateExampleSentence(entry.Id,
            entry.Senses[0].Id,
            entry.Senses[0].ExampleSentences[0].Id,
            _api.CreateUpdateBuilder<ExampleSentence>()
                .Set(e => e.Sentence.Values["en"], "updated")
                .Build());
        updatedExample.Sentence.Values["en"].Should().Be("updated");
    }

    [Fact]
    public async Task UpdateExampleSentenceTranslation()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values =
                {
                    { "en", "test" }
                }
            },
            Senses = new List<Sense>
            {
                new Sense()
                {
                    Definition = new MultiString
                    {
                        Values =
                        {
                            { "en", "test" }
                        }
                    },
                    ExampleSentences = new List<ExampleSentence>
                    {
                        new ExampleSentence()
                        {
                            Sentence = new MultiString
                            {
                                Values =
                                {
                                    { "en", "test" }
                                }
                            },
                            Translation =
                            {
                                Values =
                                {
                                    { "en", "test" }
                                }
                            }
                        }
                    }
                }
            }
        });
        entry.Senses.Should().ContainSingle().Which.ExampleSentences.Should().ContainSingle();
        var updatedExample = await _api.UpdateExampleSentence(entry.Id,
            entry.Senses[0].Id,
            entry.Senses[0].ExampleSentences[0].Id,
            _api.CreateUpdateBuilder<ExampleSentence>()
                .Set(e => e.Translation.Values["en"], "updated")
                .Build());
        updatedExample.Translation.Values["en"].Should().Be("updated");
    }

    [Fact]
    public async Task DeleteEntry()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values =
                {
                    { "en", "test" }
                }
            }
        });
        await _api.DeleteEntry(entry.Id);

        var entries = await _api.GetEntries();
        entries.Should().NotContain(e => e.Id == entry.Id);
    }
}