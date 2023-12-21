namespace lexboxClientContracts;

public interface IEntry
{
    Guid Id { get; set; }
    IMultiString LexemeForm { get; set; }
    IMultiString CitationForm { get; set; }
    IMultiString LiteralMeaning { get; set; }
    IList<ISense> Senses { get; set; }
    IMultiString Note { get; set; }
}

public class Entry : IEntry
{
    public Guid Id { get; set; }
    public IMultiString LexemeForm { get; set; } = new MultiString();
    public IMultiString CitationForm { get; set; } = new MultiString();
    public IMultiString LiteralMeaning { get; set; } = new MultiString();
    public IList<ISense> Senses { get; set; } = [];
    public IMultiString Note { get; set; } = new MultiString();
}