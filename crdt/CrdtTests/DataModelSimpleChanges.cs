using Argon;
using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib.Changes;
using CrdtLib.Db;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class DataModelSimpleChanges : DataModelTestBase
{
    private readonly Guid _entity1Id = Guid.NewGuid();
    private readonly Guid _entity2Id = Guid.NewGuid();

    [Fact]
    public async Task WritingAChangeMakesASnapshot()
    {
        await WriteNextChange(NewWord(_entity1Id, "test-value"));
        var snapshot = DbContext.Snapshots.Should().ContainSingle().Subject;
        snapshot.Entity.Is<Entry>().Value.Should().Be("test-value");

        await Verify(AllData());
    }
    
    [Fact]
    public async Task WritingA2ndChangeDoesNotEffectTheFirstSnapshot()
    {
        await WriteNextChange(NewWord(_entity1Id, "change1"));
        await WriteNextChange(NewWord(_entity1Id, "change2"));

        DbContext.Snapshots.Should()
            .SatisfyRespectively(
                snap1 => snap1.Entity.Is<Entry>().Value.Should().Be("change1"),
                snap2 => snap2.Entity.Is<Entry>().Value.Should().Be("change2")
            );

        await Verify(AllData());
    }

    [Fact]
    public async Task WritingACommitWithMultipleChangesWorks()
    {
        await WriteNextChange([
            NewWord(_entity1Id, "first"),
            NewWord(_entity2Id, "second")
        ]);
        await Verify(AllData());
    }

    [Fact]
    public async Task WriteMultipleCommits()
    {
        await WriteNextChange(NewWord(Guid.NewGuid(), "change 1"));
        await WriteNextChange(NewWord(Guid.NewGuid(), "change 2"));
        DbContext.Snapshots.Should().HaveCount(2);
        await Verify(DbContext.Commits);

        await WriteNextChange(NewWord(Guid.NewGuid(), "change 3"));
        DbContext.Snapshots.Should().HaveCount(3);
        DataModel.GetLatestObjects<Entry>().Should().HaveCount(3);
    }

    [Fact]
    public async Task WritingNoChangesWorks()
    {
        await WriteNextChange(NewWord(_entity1Id, "test-value"));
        await DataModel.AddRange(Array.Empty<Commit>());

        var snapshot = DbContext.Snapshots.Should().ContainSingle().Subject;
        snapshot.Entity.Is<Entry>().Value.Should().Be("test-value");
    }

    [Fact]
    public async Task Writing2ChangesSecondOverwritesFirst()
    {
        await WriteNextChange(NewWord(_entity1Id, "first"));
        await WriteNextChange(NewWord(_entity1Id, "second"));
        var snapshot = await DbContext.Snapshots.DefaultOrder().LastAsync();
        snapshot.Entity.Is<Entry>().Value.Should().Be("second");
    }

    [Fact]
    public async Task Writing2ChangesSecondOverwritesFirstWithLocalFirst()
    {
        var firstDate = DateTimeOffset.Now;
        var secondDate = DateTimeOffset.UtcNow.AddSeconds(1);
        await WriteChange(_localClientId, firstDate, NewWord(_entity1Id, "first"));
        await WriteChange(_localClientId, secondDate, NewWord(_entity1Id, "second"));
        var snapshot = await DbContext.Snapshots.DefaultOrder().LastAsync();
        snapshot.Entity.Is<Entry>().Value.Should().Be("second");
    }

    [Fact]
    public async Task Writing2ChangesSecondOverwritesFirstWithUtcFirst()
    {
        await WriteChange(_localClientId, DateTimeOffset.UtcNow, NewWord(_entity1Id, "first"));
        await WriteChange(_localClientId, DateTimeOffset.Now.AddSeconds(1), NewWord(_entity1Id, "second"));
        var snapshot = await DbContext.Snapshots.DefaultOrder().LastAsync();
        snapshot.Entity.Is<Entry>().Value.Should().Be("second");
    }

    [Fact]
    public async Task Writing2ChangesAtOnceWithMergedHistory()
    {
        await WriteNextChange(NewWord(_entity1Id, "first"));
        var second = await WriteNextChange(NewWord(_entity1Id, "second"));
        //add range has some additional logic that depends on proper commit ordering
        await DataModel.AddRange(new[]
        {
            await WriteChangeBefore(second, new SetAgeChange(_entity1Id, 4), false),
            await WriteNextChange(NewWord(_entity1Id, "third"), false)
        });
        var entity = await DataModel.GetLatest<Entry>(_entity1Id);
        entity.Value.Should().Be("third");
        entity.Age.Should().Be(4);

        await Verify(AllData());
    }

    [Fact]
    public async Task ChangeInsertedInTheMiddleOfHistoryWorks()
    {
        var first = await WriteNextChange(NewWord(_entity1Id, "first"));
        await WriteNextChange(NewWord(_entity1Id, "second"));

        await WriteChangeAfter(first, new SetAgeChange(_entity1Id, 3));
        var snapshot = await DbContext.Snapshots.DefaultOrder().LastAsync();
        var entry = snapshot.Entity.Is<Entry>();
        entry.Age.Should().Be(3);
        entry.Value.Should().Be("second");
    }


    [Fact]
    public async Task CanTrackMultipleEntries()
    {
        await WriteNextChange(NewWord(_entity1Id, "entity1"));
        await WriteNextChange(NewWord(_entity2Id, "entity2"));

        (await DataModel.GetLatest<Entry>(_entity1Id)).Value.Should().Be("entity1");
        (await DataModel.GetLatest<Entry>(_entity2Id)).Value.Should().Be("entity2");
    }

    [Fact]
    public async Task CanCreate2EntriesOutOfOrder()
    {
        var commit1 = await WriteNextChange(NewWord(_entity1Id, "entity1"));
        await WriteChangeBefore(commit1, NewWord(_entity2Id, "entity2"));
    }

    [Fact]
    public async Task CanDeleteAnEntry()
    {
        await WriteNextChange(NewWord(_entity1Id, "test-value"));
        var deleteCommit = await WriteNextChange(new DeleteChange<Entry>(_entity1Id));
        var snapshot = await DbContext.Snapshots.DefaultOrder().LastAsync();
        snapshot.Entity.DeletedAt.Should().Be(deleteCommit.DateTime);
    }

    [Fact]
    public async Task CanUseYText()
    {
        await WriteNextChange(NewWord(_entity1Id, "test-value"));
        var entry1 = await DataModel.GetLatest<Entry>(_entity1Id);
        await WriteNextChange(new ChangeText(entry1, text => text.Insert(0, "Yo Jason")));
        await WriteNextChange(new ChangeText(entry1, text => text.Insert(3, "What's up ")));

        entry1 = await DataModel.GetLatest<Entry>(_entity1Id);
        entry1.YText.ToString().Should().Be("Yo What's up Jason");
    }

    [Fact]
    public async Task CanGetEntryLinq2Db()
    {
        await WriteNextChange(NewWord(_entity1Id, "test-value"));

        var entries = await DataModel.GetLatestObjects<Entry>().ToArrayAsyncLinqToDB();
        entries.Should().ContainSingle();
    }
}