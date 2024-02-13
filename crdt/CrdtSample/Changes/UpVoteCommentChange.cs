using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using CrdtSample.Models;

namespace CrdtSample.Changes;

public class UpVoteCommentChange(Guid commentId): Change<UpVote>(Guid.NewGuid()), ISelfNamedType<UpVoteCommentChange>
{
    public Guid CommentId { get; } = commentId;

    public override IObjectBase NewEntity(Commit commit)
    {
        return new UpVote
        {
            Id = EntityId,
            CommentId = CommentId
        };
    }

    public override async ValueTask ApplyChange(UpVote entity, ChangeContext context)
    {
        if (await context.IsObjectDeleted(CommentId))
        {
            entity.DeletedAt = context.Commit.DateTime;
            await context.MarkDeleted(EntityId);
        }
    }
}