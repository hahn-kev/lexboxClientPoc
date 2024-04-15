using CrdtSample.Models;

namespace Tests;

public class DataModelIntegrityTests : DataModelTestBase
{
    [Fact]
    public async Task CanAddTheSameCommitMultipleTimes()
    {
        var entity1Id = Guid.NewGuid();
        var first = await WriteNextChange(NewWord(entity1Id, "entity1"));
        await WriteNextChange(NewWord(entity1Id, "entity1.1"));
        await DataModel.Add(first);
        await DataModel.Add(first);

        var entry = await DataModel.GetLatest<Entry>(entity1Id);
        entry.Value.Should().Be("entity1.1");
    }

    [Fact]
    public async Task CanAddTheSameCommitMultipleTimesAtOnce()
    {
        var entity1Id = Guid.NewGuid();
        var first = await WriteNextChange(NewWord(entity1Id, "entity1"));
        await WriteNextChange(NewWord(entity1Id, "entity1.1"));
        await DataModel.AddRange(Enumerable.Repeat(first, 5));

        var entry = await DataModel.GetLatest<Entry>(entity1Id);
        entry.Value.Should().Be("entity1.1");
    }
}