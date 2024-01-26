using System.Text.Json.Serialization;
using CrdtLib.Db;
using CrdtLib.Entities;
using Ycs;

namespace CrdtSample.Models;

public record Entry : IObjectBase<Entry>, INewableObject<Entry>
{
    public static Entry New(Guid id, Commit commit)
    {
        return new Entry
        {
            Id = id
        };
    }

    public Guid Id { get; init; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? EntryReference { get; set; }

    public Guid[] GetReferences()
    {
        if (EntryReference is null) return Array.Empty<Guid>();
        return new[] { EntryReference.Value };
    }

    public void RemoveReference(Guid id, Commit commit)
    {
        if (EntryReference == id) EntryReference = null;
    }

    public IObjectBase Copy()
    {
        return this with { };
    }

    public string? Comment { get; set; }
    public string? Value { get; set; }
    public DateTime? FirstUsed { get; set; }

    private YDoc _yDoc = new YDoc();

    [JsonIgnore]
    public YText YText => _yDoc.GetText();

    public string YTextBlob
    {
        get => Convert.ToBase64String(_yDoc.EncodeStateAsUpdateV2());
        set
        {
            _yDoc = new YDoc();
            _yDoc.ApplyUpdateV2(Convert.FromBase64String(value));
        }
    }
}