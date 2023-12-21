namespace lexboxClientContracts;

public class Entry
{
    public Guid Id { get; set; }
    public MultiString LexemeForm { get; set; } = new();
    public MultiString CitationForm { get; set; } = new();
    public MultiString LiteralMeaning { get; set; } = new();
    public List<Sense> Senses { get; set; } = [];
    public MultiString Note { get; set; } = new();
}

public class Sense
{
    public Guid Id { get; set; }
    public MultiString Definition { get; set; } = new();
    public MultiString Gloss { get; set; } = new();
    public string PartOfSpeech { get; set; } = string.Empty;
    public List<string> SemanticDomain { get; set; } = [];
    public List<ExampleSentence> ExampleSentences { get; set; } = [];
}

public class ExampleSentence
{
    public Guid Id { get; set; }
    public MultiString Sentence { get; set; } = new();
    public MultiString Translation { get; set; } = new();
    public string Reference { get; set; } = string.Empty;
}

/// <summary>
/// map like object with writing system as key and string as value
/// </summary>
public class MultiString
{
    public Dictionary<WritingSystemId, string> Values { get; set; } = new();
}