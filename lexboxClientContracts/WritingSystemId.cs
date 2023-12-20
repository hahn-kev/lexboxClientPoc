namespace lexboxClientContracts;

public record WritingSystem
{
    public WritingSystemId Id { get; set; }
    public string Name { get; set; }
    public string Abbreviation { get; set; }
    public string Font { get; set; }
    //todo probably need more stuff here, see wesay for ideas
}

public readonly record struct WritingSystemId(string Code): ISpanFormattable, ISpanParsable<WritingSystemId>
{
    public static implicit operator string(WritingSystemId ws) => ws.Code;
    public static implicit operator WritingSystemId(string code) => new(code);
    public static implicit operator WritingSystemId(ReadOnlySpan<char> code) => new(new string(code));
    public override string ToString()
    {
        return Code;
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        return Code;
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        Code.AsSpan().CopyTo(destination);
        charsWritten = Code.Length;
        return true;
    }

    public static WritingSystemId Parse(string s, IFormatProvider? provider)
    {
        return s;
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out WritingSystemId result)
    {
        result = default;
        if (s is null) return false;
        result = s;
        return true;
    }

    public static WritingSystemId Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        return s;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out WritingSystemId result)
    {
        result = s;
        return true;
    }
}