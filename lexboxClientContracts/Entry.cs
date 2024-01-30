using System.Text.Json.Serialization;
using Tapper;

namespace lexboxClientContracts;

[TranspilationSource]
public class Entry
{
    public virtual Guid Id { get; set; }

    public virtual MultiString LexemeForm { get; set; } = new();

    public virtual MultiString CitationForm { get; set; } = new();

    public virtual MultiString LiteralMeaning { get; set; } = new();
    public virtual IList<Sense> Senses { get; set; } = [];

    public virtual MultiString Note { get; set; } = new();
}