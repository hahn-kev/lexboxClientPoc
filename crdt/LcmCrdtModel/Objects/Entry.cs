using CrdtLib.Db;
using CrdtLib.Entities;

namespace LcmCrdtModel.Objects;

public class Entry : lexboxClientContracts.Entry, IObjectBase<Entry>, INewableObject<Entry>
{
    public static Entry New(Guid id, Commit commit)
    {
        return new()
        {
            Id = id
        };
    }

    Guid IObjectBase.Id
    {
        get => Id;
        init => Id = value;
    }

    public DateTimeOffset? DeletedAt { get; set; }

    public Guid[] GetReferences()
    {
        return [];
    }

    public void RemoveReference(Guid id, Commit commit)
    {
    }

    public IObjectBase Copy()
    {
        return new Entry
        {
            Id = Id,
            DeletedAt = DeletedAt,
            LexemeForm = LexemeForm.Copy(),
            CitationForm = CitationForm.Copy(),
            LiteralMeaning = LiteralMeaning.Copy(),
            Note = Note.Copy()
        };
    }
}