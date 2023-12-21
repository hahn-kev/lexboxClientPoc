namespace lexboxClientContracts;

public interface IMultiString
{
    IDictionary<WritingSystemId, string> Values { get; set; }
}

/// <summary>
/// map like object with writing system as key and string as value
/// </summary>
public class MultiString : IMultiString
{
    public IDictionary<WritingSystemId, string> Values { get; set; } = new Dictionary<WritingSystemId, string>();
}