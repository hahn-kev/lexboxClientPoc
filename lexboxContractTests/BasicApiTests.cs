using FluentAssertions;
using lexboxClientContracts;

namespace lexboxContractTests;

public class BasicApiTests
{
    private readonly ILexboxApi _api;

    public BasicApiTests()
    {
        _api = new InMemoryApi();
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
        var entries = await _api.GetEntries("a", new QueryOptions("lexemeForm"));
        entries.Should().NotBeEmpty();
    }

    [Test]
    public async Task GetEntriesWithoutExemplar()
    {
        var entries = await _api.GetEntries();
        entries.Should().NotBeEmpty();
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
        var entries = await _api.GetEntries("a");
        var entry = await _api.GetEntry(entries.First().Id);
        entry.Should().NotBeNull();
    }

    [Test]
    public async Task CreateEntry()
    {
        var id = Guid.NewGuid();
        await _api.CreateEntry(new Entry
        {
            Id = id,
            LexemeForm = new MultiString
            {
                Values = new Dictionary<WritingSystemId, string>
                {
                    { "en", "test" }
                }
            }
        });
        var entry = await _api.GetEntry(id);
        entry.Should().NotBeNull();
        entry.LexemeForm.Values["en"].Should().Be("test");
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
}