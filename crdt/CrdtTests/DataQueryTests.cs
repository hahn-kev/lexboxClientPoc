using CrdtSample.Models;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class DataQueryTests: DataModelTestBase
{
    private readonly Guid _entity1Id = Guid.NewGuid();
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await WriteNextChange(NewWord(_entity1Id, "entity1"));
    }

    [Fact]
    public async Task CanQueryLatestData()
    {
        var entries = await DataModel.GetLatestObjects<Entry>().ToArrayAsync();
        var entry = entries.Should().ContainSingle().Subject;
        entry.Value.Should().Be("entity1");
    }
}