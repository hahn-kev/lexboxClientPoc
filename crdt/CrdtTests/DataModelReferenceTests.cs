using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib.Changes;

namespace Tests;

public class DataModelReferenceTests : DataModelTestBase
{
    private readonly Guid _entity1Id = Guid.NewGuid();
    private readonly Guid _entity2Id = Guid.NewGuid();

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await WriteNextChange(SimpleChange(_entity1Id, "entity1"));
        await WriteNextChange(SimpleChange(_entity2Id, "entity2"));
    }

    [Fact]
    public async Task CanAddReferenceBetweenEntries()
    {
        await WriteNextChange(new AddReferenceChange(_entity1Id, _entity2Id));
        var entry = await DataModel.GetLatest<Entry>(_entity1Id);
        entry.EntryReference.Should().Be(_entity2Id);
    }

    [Fact]
    public async Task CanNotAddRefToDeletedEntry()
    {
        await WriteNextChange(new DeleteChange<Entry>(_entity2Id));
        await WriteNextChange(new AddReferenceChange(_entity1Id, _entity2Id));
        var entry = await DataModel.GetLatest<Entry>(_entity1Id);
        entry.EntryReference.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAfterTheFactRewritesReferences()
    {
        var addRef = await WriteNextChange(new AddReferenceChange(_entity1Id, _entity2Id));
        var entryWithRef = await DataModel.GetLatest<Entry>(_entity1Id);
        entryWithRef.EntryReference.Should().Be(_entity2Id);

        await WriteChangeBefore(addRef, new DeleteChange<Entry>(_entity2Id));
        var entryWithoutRef = await DataModel.GetLatest<Entry>(_entity1Id);
        entryWithoutRef.EntryReference.Should().BeNull();
    }

    [Fact]
    public async Task DeleteRemovesAllReferences()
    {
        await WriteNextChange(new AddReferenceChange(_entity1Id, _entity2Id));
        var entryWithRef = await DataModel.GetLatest<Entry>(_entity1Id);
        entryWithRef.EntryReference.Should().Be(_entity2Id);

        await WriteNextChange(new DeleteChange<Entry>(_entity2Id));
        var entryWithoutRef = await DataModel.GetLatest<Entry>(_entity1Id);
        entryWithoutRef.EntryReference.Should().BeNull();
    }

    [Fact]
    public async Task SnapshotsDontGetMutatedByADelete()
    {
        var refAdd = await WriteNextChange(new AddReferenceChange(_entity1Id, _entity2Id));
        await WriteNextChange(new DeleteChange<Entry>(_entity2Id));
        var entitySnapshot1 = await DataModel.GetEntitySnapshotAtTime(refAdd.DateTime, _entity1Id);
        entitySnapshot1.Should().NotBeNull();
        entitySnapshot1!.Entity.Is<Entry>().EntryReference.Should().Be(_entity2Id);
    }

    [Fact]
    public async Task DeleteRetroactivelyRemovesRefs()
    {
        var entityId3 = Guid.NewGuid();
        await WriteNextChange(SimpleChange(entityId3, "entity3"));
        await WriteNextChange(new AddReferenceChange(_entity1Id, _entity2Id));
        var delete = await WriteNextChange(new DeleteChange<Entry>(_entity2Id));

        //a ref was synced in the past, it happened before the delete, the reference should be retroactively removed
        await WriteChangeBefore(delete, new AddReferenceChange(entityId3, _entity2Id));
        var entry = await DataModel.GetLatest<Entry>(entityId3);
        entry.EntryReference.Should().BeNull();
    }

    [Fact]
    public async Task CommentChainGetsDeleted()
    {
        var entry1 = Guid.NewGuid();
        await WriteNextChange(SimpleChange(entry1, "entry1"));
        var comment1 = new CommentOnEntryChange("hello 1", entry1);
        var comment1Id = comment1.EntityId;
        await WriteNextChange(comment1);
        var upvote = new UpVoteCommentChange(comment1Id);
        var upvoteId = upvote.EntityId;
        await WriteNextChange(upvote);

        await WriteNextChange(new DeleteChange<Entry>(entry1));

        var snapshot = await DataModel.GetProjectSnapshot();
        snapshot.Snapshots.Should().NotContainKeys(
            entry1,
            comment1Id,
            upvoteId
        );
    }
}