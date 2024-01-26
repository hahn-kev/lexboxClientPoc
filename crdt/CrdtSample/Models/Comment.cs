using CrdtLib.Db;
using CrdtLib.Entities;

namespace CrdtSample.Models;

public record Comment: IObjectBase<Comment>
{
    public Guid Id { get; init; }
    public DateTimeOffset? DeletedAt { get; set; }
    static string IPolyType.TypeName => "custom_comment_discriminator";
    public required DateTimeOffset CreatedAt { get; init; }
    public required Guid EntryId { get; init; }
    public required string CommentText { get; set; }
    public Guid[] GetReferences()
    {
        if (DeletedAt is not null) return Array.Empty<Guid>();
        return [EntryId];
    }

    public void RemoveReference(Guid id, Commit commit)
    {
        if (EntryId != id) return;
        DeletedAt = commit.DateTime;
    }

    public IObjectBase Copy()
    {
        return this with { };
    }
}