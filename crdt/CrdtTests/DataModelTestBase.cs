using CrdtSample;
using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib;
using CrdtLib.Changes;
using CrdtLib.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests;

public class DataModelTestBase : IAsyncLifetime
{
    protected readonly ServiceProvider _services;
    protected readonly Guid _localClientId = Guid.NewGuid();
    public readonly DataModel DataModel;
    public readonly CrdtDbContext DbContext;

    public DataModelTestBase()
    {
        _services = new ServiceCollection()
            .AddCrdtDataSample(":memory:")
            .BuildServiceProvider();
        DbContext = _services.GetRequiredService<CrdtDbContext>();
        DataModel = _services.GetRequiredService<DataModel>();
    }

    public void SetCurrentDate(DateTime dateTime)
    {
        currentDate = dateTime;
    }

    private DateTimeOffset currentDate = new(new DateTime(2000, 1, 1));
    private DateTimeOffset NextDate() => currentDate = currentDate.AddDays(1);

    public async ValueTask<Commit> WriteNextChange(IChange change, bool add = true)
    {
        return await WriteChange(_localClientId, NextDate(), change, add);
    }

    public async ValueTask<Commit> WriteNextChange(IEnumerable<IChange> changes, bool add = true)
    {
        return await WriteChange(_localClientId, NextDate(), changes, add);
    }

    public async ValueTask<Commit> WriteChangeAfter(Commit after, IChange change)
    {
        return await WriteChange(_localClientId, after.DateTime.AddHours(1), change);
    }

    public async ValueTask<Commit> WriteChangeBefore(Commit after, IChange change, bool add = true)
    {
        return await WriteChange(_localClientId, after.DateTime.AddHours(-1), change, add);
    }

    protected async ValueTask<Commit> WriteChange(Guid clientId,
        DateTimeOffset dateTime,
        IChange change,
        bool add = true)
    {
        return await WriteChange(clientId, dateTime, [change], add);
    }
    
    protected async ValueTask<Commit> WriteChange(Guid clientId, DateTimeOffset dateTime, IEnumerable<IChange> change, bool add = true)
    {
        var commit = new Commit
        {
            ClientId = clientId,
            DateTime = dateTime,
            ChangeEntities = change.Select(c => new ChangeEntity(c)).ToList()
        };
        if (add) await DataModel.Add(commit);
        return commit;
    }

    public SimpleChange SimpleChange(Guid entityId, string value)
    {
        return new SimpleChange(entityId)
        {
            Values =
            {
                { nameof(Entry.Value), value }
            }
        };
    }

    public virtual async Task InitializeAsync()
    {
        await DbContext.Database.OpenConnectionAsync();
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _services.DisposeAsync();
    }
}