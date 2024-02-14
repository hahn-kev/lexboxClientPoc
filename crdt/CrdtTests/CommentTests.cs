using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib.Changes;

namespace Tests;

public class CommentTests : DataModelTestBase
{
    [Fact]
    public async Task CanCommentOnAnEntry()
    {
        var entryId = Guid.NewGuid();
        await WriteNextChange(SimpleChange(entryId, "hello"));
        await WriteNextChange(new CommentOnEntryChange("hello world!", entryId));
        var snapshot = await DataModel.GetProjectSnapshot();
        var commentSnapshot = snapshot.Snapshots.Values.Single(s => s.IsType<Comment>());
        var comment = (Comment)await DataModel.GetBySnapshotId(commentSnapshot.Id);
        comment.CommentText.Should().Be("hello world!");
        comment.EntryId.Should().Be(entryId);
    }

    [Fact]
    public async Task DeletingAnEntryDeletesComments()
    {
        var entryId = Guid.NewGuid();
        await WriteNextChange(SimpleChange(entryId, "hello"));
        await WriteNextChange(new CommentOnEntryChange("hello world!", entryId));
        await WriteNextChange(new DeleteChange<Entry>(entryId));
        var snapshot = await DataModel.GetProjectSnapshot();
        snapshot.Snapshots.Values.Where(s => !s.EntityIsDeleted).Should().BeEmpty();
    }

    [Fact]
    public async Task CommentsOnDeletedEntriesAreDeletedAutomatically()
    {
        var entryId = Guid.NewGuid();
        await WriteNextChange(SimpleChange(entryId, "hello"));
        await WriteNextChange(new DeleteChange<Entry>(entryId));
        await WriteNextChange(new CommentOnEntryChange("hello world!", entryId));
        var snapshot = await DataModel.GetProjectSnapshot();
        snapshot.Snapshots.Values.Where(s => !s.EntityIsDeleted).Should().BeEmpty();
    }
}