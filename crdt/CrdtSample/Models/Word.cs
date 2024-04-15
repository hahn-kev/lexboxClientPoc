using CrdtLib.Db;
using CrdtLib.Entities;

namespace CrdtSample.Models;

public class Word : IObjectBase<Word>
{
    public required string Text { get; set; }

    public Guid Id { get; init; }
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
        return new Word
        {
            Text = Text,
            Id = Id,
            DeletedAt = DeletedAt
        };
    }
}