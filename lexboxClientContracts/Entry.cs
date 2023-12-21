namespace lexboxClientContracts;

public class Entry
{
    public Guid Id { get; set; }
    public MultiString LexemeForm { get; set; }
    public MultiString CitationForm { get; set; }
    public MultiString LiteralMeaning { get; set; }
    public List<Sense> Senses { get; set; }
    public MultiString Note { get; set; }
}

public class Sense
{
    public Guid Id { get; set; }
    public MultiString Definition { get; set; }
    public MultiString Gloss { get; set; }
    public string PartOfSpeech { get; set; }
    public List<string> SemanticDomain { get; set; }
    public List<ExampleSentence> ExampleSentences { get; set; }
}

public class ExampleSentence
{
    public Guid Id { get; set; }
    public MultiString Sentence { get; set; }
    public MultiString Translation { get; set; }
    public MultiString Reference { get; set; }
}

/// <summary>
/// map like object with writing system as key and string as value
/// </summary>
public class MultiString
{
    public Dictionary<WritingSystemId, string> Values { get; set; } = new();
}