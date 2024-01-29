using System.Text.Json.Serialization;
using Tapper;

namespace lexboxClientContracts;
[TranspilationSource]
public interface IEntry
{
    Guid Id { get; set; }
    IMultiString LexemeForm { get; set; }
    IMultiString CitationForm { get; set; }
    IMultiString LiteralMeaning { get; set; }
    IList<ISense> Senses { get; set; }
    IMultiString Note { get; set; }
}

[TranspilationSource]
public class Entry : IEntry
{
    public Guid Id { get; set; }

    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public IMultiString LexemeForm { get; set; } = new MultiString();

    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public IMultiString CitationForm { get; set; } = new MultiString();

    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public IMultiString LiteralMeaning { get; set; } = new MultiString();
    public IList<ISense> Senses { get; set; } = [];

    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public IMultiString Note { get; set; } = new MultiString();
}