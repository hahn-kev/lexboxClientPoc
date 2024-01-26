using System.Text.Json;
using lexboxClientContracts;
using Xunit.Abstractions;
using Entry = LcmCrdtModel.Objects.Entry;

namespace Tests.LcmModelTests;

public class SerializationTests(ITestOutputHelper output)
{
    [Fact]
    public void CanSerializeEntry()
    {
        var entry = new Entry()
        {
            Id = Guid.NewGuid(),
            LexemeForm = { Values = { { "en", "test" } } },
            CitationForm = { Values = { { "en", "test" } } },
            Senses =
            {
                new Sense
                {
                    Id = Guid.NewGuid(),
                    Gloss = { Values = { { "en", "test" } } }
                }
            }
        };
        var act = () => JsonSerializer.Serialize(entry);
        var json = act.Should().NotThrow().Subject;
        output.WriteLine(json);
    }

    [Fact]
    public void CanDeserializeEntry()
    {
        var entry = new Entry()
        {
            Id = Guid.NewGuid(),
            LexemeForm = { Values = { { "en", "test" } } },
            CitationForm = { Values = { { "en", "test" } } },
            Senses =
            {
                new Sense
                {
                    Id = Guid.NewGuid(),
                    Gloss = { Values = { { "en", "test" } } },
                    ExampleSentences =
                    {
                        new ExampleSentence()
                        {
                            Id = Guid.NewGuid(),
                            Sentence = { Values = { { "en", "this is only a test" } } }
                        }
                    }
                }
            }
        };
        var json = JsonSerializer.Serialize(entry);
        var act = () => JsonSerializer.Deserialize<Entry>(json);
        var fromJson = act.Should().NotThrow().Subject;
        fromJson.Should().BeEquivalentTo(entry);
    }

    [Fact]
    public void CanDeserializeMultiString()
    {
        //lang=json
        var json = """{"$type": "ms", "Values": {"en": "test"}}""";
        var expectedMs = new MultiString()
        {
            Values = { { "en", "test" } }
        };
        var actualMs = JsonSerializer.Deserialize<IMultiString>(json);
        actualMs.Should().NotBeNull();
        actualMs!.Values.Should().ContainKey("en");
        actualMs.Should().BeEquivalentTo(expectedMs);
    }

    [Fact]
    public void EqualityTest()
    {
        var entryId = Guid.NewGuid();
        var senseId = Guid.NewGuid();
        var entry = new Entry()
        {
            Id = entryId,
            LexemeForm = { Values = { { "en", "test" } } },
            CitationForm = { Values = { { "en", "test" } } },
            Senses =
            {
                new Sense
                {
                    Id = senseId,
                    Gloss = { Values = { { "en", "test" } } }
                }
            }
        };
        var entryCopy = new Entry()
        {
            Id = entryId,
            LexemeForm = { Values = { { "en", "test" } } },
            CitationForm = { Values = { { "en", "test" } } },
            Senses =
            {
                new Sense
                {
                    Id = senseId,
                    Gloss = { Values = { { "en", "test" } } }
                }
            }
        };
        entry.Should().BeEquivalentTo(entryCopy);
    }
}