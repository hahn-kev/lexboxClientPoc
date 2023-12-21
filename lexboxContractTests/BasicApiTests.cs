using AppLayer.Api;
using FluentAssertions;
using lexboxClientContracts;

namespace lexboxContractTests;

public class BasicApiTests
{
    // set this to false to run against Lcm
    private const bool inMemory = true;
    private static ILexboxApi _api = null!;

    public BasicApiTests()
    {
        if (inMemory)
            _api = new InMemoryApi();
    }

    [OneTimeSetUp]
    public static async Task Setup()
    {
        if (!inMemory)
            _api = await LexboxLcmApiFactory.CreateApi(@"C:\ProgramData\SIL\FieldWorks\Projects\sena-3\sena-3.fwdata",
                false);
    }

    [OneTimeTearDown]
    public static async Task TearDown()
    {
        (_api as IDisposable)?.Dispose();
        await ((_api as IAsyncDisposable)?.DisposeAsync() ?? ValueTask.CompletedTask);
    }

    [Test]
    public async Task GetWritingSystems()
    {
        var writingSystems = await _api.GetWritingSystems();
        writingSystems.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetExemplars()
    {
        var exemplars = await _api.GetExemplars();
        exemplars.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetEntries()
    {
        var entries = await _api.GetEntries("a");
        entries.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetEntriesWithOptions()
    {
        var entries = await _api.GetEntries(new QueryOptions("lexemeForm"));
        entries.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetEntriesWithoutExemplar()
    {
        var entries = await _api.GetEntries();
        entries.Should().NotBeEmpty();
        var entry = entries.First();
        entry.LexemeForm.Values.Should().NotBeEmpty();
    }

    [Test]
    public async Task SearchEntries()
    {
        var entries = await _api.SearchEntries("a");
        entries.Should().NotBeEmpty();
    }

    [Test]
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

    [Test]
    public async Task CreateEntry()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values = new Dictionary<WritingSystemId, string>
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
        entry.Should().NotBeNull();
        entry.LexemeForm.Values["en"].Should().Be("Kevin");
        entry.Note.Values["en"].Should().Be("this is a test note from Kevin");
        var sense = entry.Senses.Should().ContainSingle().Subject;
        sense.Gloss.Values["en"].Should().Be("Kevin");
        var example = sense.ExampleSentences.Should().ContainSingle().Subject;
        example.Sentence.Values["en"].Should().Be("Kevin is a good guy");
    }

    [Test]
    public async Task UpdateEntry()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values = new Dictionary<WritingSystemId, string>
                {
                    { "en", "test" }
                }
            }
        });
        var updatedEntry = await _api.UpdateEntry(entry.Id,
            _api.CreateUpdateBuilder<Entry>()
                .Set(e => e.LexemeForm.Values["en"], "updated")
                .Build());
        updatedEntry.LexemeForm.Values["en"].Should().Be("updated");
    }

    [Test]
    public async Task UpdateSense()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values = new Dictionary<WritingSystemId, string>
                {
                    { "en", "test" }
                }
            },
            Senses = new List<Sense>
            {
                new()
                {
                    Definition = new MultiString
                    {
                        Values = new Dictionary<WritingSystemId, string>
                        {
                            { "en", "test" }
                        }
                    }
                }
            }
        });
        var updatedEntry = await _api.UpdateEntry(entry.Id,
            _api.CreateUpdateBuilder<Entry>()
                .Set(e => e.Senses[0].Definition.Values["en"], "updated")
                .Build());
        updatedEntry.Senses[0].Definition.Values["en"].Should().Be("updated");
    }

    [Test]
    public async Task UpdateExampleSentence()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values = new Dictionary<WritingSystemId, string>
                {
                    { "en", "test" }
                }
            },
            Senses = new List<Sense>
            {
                new()
                {
                    Definition = new MultiString
                    {
                        Values = new Dictionary<WritingSystemId, string>
                        {
                            { "en", "test" }
                        }
                    },
                    ExampleSentences = new List<ExampleSentence>
                    {
                        new()
                        {
                            Sentence = new MultiString
                            {
                                Values = new Dictionary<WritingSystemId, string>
                                {
                                    { "en", "test" }
                                }
                            }
                        }
                    }
                }
            }
        });
        var updatedEntry = await _api.UpdateEntry(entry.Id,
            _api.CreateUpdateBuilder<Entry>()
                .Set(e => e.Senses[0].ExampleSentences[0].Sentence.Values["en"], "updated")
                .Build());
        updatedEntry.Senses[0].ExampleSentences[0].Sentence.Values["en"].Should().Be("updated");
    }

    [Test]
    public async Task DeleteEntry()
    {
        var entry = await _api.CreateEntry(new Entry
        {
            LexemeForm = new MultiString
            {
                Values = new Dictionary<WritingSystemId, string>
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