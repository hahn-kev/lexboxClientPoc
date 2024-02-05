using System.Diagnostics.CodeAnalysis;
using CrdtSample.Models;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;

namespace CrdtSample.Changes;

public class CommentOnEntryChange : Change<Comment>, ISelfNamedType<CommentOnEntryChange>
{
    public string Comment { get; }
    public Guid EntryId { get; }

    public CommentOnEntryChange(string comment, Guid entryId): base(Guid.NewGuid())
    {
        Comment = comment;
        EntryId = entryId;
    }

    public override IObjectBase NewEntity(Commit commit)
    {
        return new Comment
        {
            Id = EntityId,
            CreatedAt = commit.DateTime,
            EntryId = EntryId,
            CommentText = Comment
        };
    }

    public override async ValueTask ApplyChange(Comment entity, ChangeContext context)
    {
        entity.CommentText = Comment;
        if (await context.IsObjectDeleted(EntryId))
        {
            entity.DeletedAt = context.Commit.DateTime;
            await context.MarkDeleted(EntityId);
        }
    }
}