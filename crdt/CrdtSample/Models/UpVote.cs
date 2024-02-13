using CrdtLib.Db;
using CrdtLib.Entities;

namespace CrdtSample.Models;

public record UpVote: IObjectBase<UpVote>
{
    public Guid Id { get; init; }
    public DateTimeOffset? DeletedAt { get; set; }
    public required Guid CommentId { get; set; }
    public Guid[] GetReferences()
    {
        return [CommentId];
    }

    public void RemoveReference(Guid id, Commit commit)
    {
        if (CommentId == id)
        {
            DeletedAt = commit.DateTime;
        }
    }

    public IObjectBase Copy()
    {
        return this with { };
    }
}