using System.Text.Json.Serialization;

namespace lexboxClientContracts;

[JsonDerivedType(typeof(Sense), "sense")]
public interface ISense
{
    Guid Id { get; set; }
    IMultiString Definition { get; set; }
    IMultiString Gloss { get; set; }
    string PartOfSpeech { get; set; }
    IList<string> SemanticDomain { get; set; }
    IList<IExampleSentence> ExampleSentences { get; set; }
}

public class Sense : ISense
{
    public Guid Id { get; set; }
    public IMultiString Definition { get; set; } = new MultiString();
    public IMultiString Gloss { get; set; } = new MultiString();
    public string PartOfSpeech { get; set; } = string.Empty;
    public IList<string> SemanticDomain { get; set; } = [];
    public IList<IExampleSentence> ExampleSentences { get; set; } = [];
}