namespace lexboxClientContracts;

public interface IExampleSentence
{
    Guid Id { get; set; }
    IMultiString Sentence { get; set; }
    IMultiString Translation { get; set; }
    string Reference { get; set; }
}

public class ExampleSentence : IExampleSentence
{
    public Guid Id { get; set; }
    public IMultiString Sentence { get; set; } = new MultiString();
    public IMultiString Translation { get; set; } = new MultiString();
    public string Reference { get; set; } = string.Empty;
}