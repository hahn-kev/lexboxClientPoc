using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib.Db;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class ModelSnapshotTests : DataModelTestBase
{
    [Fact]
    public void CanGetEmptyModelSnapshot()
    {
        DataModel.GetSnapshot().Should().NotBeNull();
    }

    [Fact]
    public async Task CanGetModelSnapshot()
    {
        await WriteNextChange(SimpleChange(Guid.NewGuid(), "entity1"));
        await WriteNextChange(SimpleChange(Guid.NewGuid(), "entity2"));
        var snapshot = await DataModel.GetSnapshot();
        snapshot.Snapshots.Should().HaveCount(2);
    }

    [Fact]
    public async Task ModelSnapshotShowsMultipleChanges()
    {
        var entityId = Guid.NewGuid();
        await WriteNextChange(SimpleChange(entityId, "first"));
        var secondChange = await WriteNextChange(SimpleChange(entityId, "second"));
        var snapshot = await DataModel.GetSnapshot();
        var simpleSnapshot = snapshot.Snapshots.Values.First();
        var entity = await DataModel.LoadObject(simpleSnapshot.Id);
        var entry = entity.Is<Entry>();
        entry.Value.Should().Be("second");
        snapshot.LastChange.Should().Be(secondChange.DateTime);
    }

    [Theory]
    [InlineData(10)]
    // [InlineData(100)]
    //not going higher because we run into insert performance issues
    // [InlineData(1_000)]
    public async Task CanGetSnapshotFromEarlier(int changeCount)
    {
        var entityId = Guid.NewGuid();
        await WriteNextChange(new MakeCommentChange(entityId, "hello"));
        var changes = new List<Commit>(changeCount);
        var addNew = new List<Commit>(changeCount);
        for (var i = 0; i < changeCount; i++)
        {
            changes.Add(await WriteNextChange(SimpleChange(entityId, $"change {i}"), false).AsTask());
            addNew.Add(await WriteNextChange(SimpleChange(Guid.NewGuid(), $"add {i}"), false).AsTask());
        }

        //adding all in one AddRange means there's sparse snapshots
        await DataModel.AddRange(changes.Concat(addNew));
        //there will only be a snapshot for every other commit, but there's change count * 2 commits, plus a first and last change
        DbContext.Snapshots.Should().HaveCount(2 + changeCount);

        for (int i = 0; i < changeCount; i++)
        {
            var snapshots = await DataModel.GetSnapshotsAt(changes[i].DateTime);
            var entry = snapshots[entityId].Entity.Is<Entry>();
            entry.Value.Should().Be($"change {i}");
            entry.Comment.Should().Be("hello");
            snapshots.Values.Should().HaveCount(1 + i);
        }
    }


    /// <summary>
    /// test isn't super useful as a perf test as 99% of the time is just inserting data
    /// </summary>
    [Fact(Skip = "Slow test")]
    public async Task WorstCaseSnapshotReApply()
    {
        int changeCount = 10_000;
        var entityId = Guid.NewGuid();
        await WriteNextChange(new MakeCommentChange(entityId, "hello"));
        //adding all in one AddRange means there's sparse snapshots
        await DataModel.AddRange(Enumerable.Range(0, changeCount)
            .Select(i => WriteNextChange(SimpleChange(entityId, $"change {i}"), false).Result));

        var latestSnapshot = await DataModel.GetLatest(entityId);
        //delete snapshots so when we get at then we need to re-apply
        await DbContext.Snapshots.Where(s => !s.IsRoot).ExecuteDeleteAsync();

        var computedModelSnapshots = await DataModel.GetSnapshotsAt(latestSnapshot.Commit.DateTime);

        var entitySnapshot = computedModelSnapshots.Should().ContainSingle().Subject.Value;
        entitySnapshot.Should().BeEquivalentTo(latestSnapshot, options => options.Excluding(snapshot => snapshot.Id));
        var latestSnapshotEntry = latestSnapshot.Entity.Is<Entry>();
        var entitySnapshotEntry = entitySnapshot.Entity.Is<Entry>();
        entitySnapshotEntry.Comment.Should().Be(latestSnapshotEntry.Comment);
        entitySnapshotEntry.Value.Should().Be(latestSnapshotEntry.Value);
    }
}