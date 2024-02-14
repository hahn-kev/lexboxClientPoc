using System.Text.Json;
using CrdtSample;
using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib;
using CrdtLib.Changes;
using CrdtLib.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var client1Id = Guid.NewGuid();
var client2Id = Guid.NewGuid();
var entry1Id = Guid.NewGuid();
var entry2Id = Guid.NewGuid();
await using var services = new ServiceCollection()
    .AddCrdtDataSample("db.sqlite")
    .BuildServiceProvider();
await using var context = services.GetRequiredService<CrdtDbContext>();

var localModel = services.GetRequiredService<DataModel>();
await localModel.AddRange(new[]
{
    new Commit
    {
        ClientId = client1Id,
        DateTime = new DateTime(2000, 1, 1),
        ChangeEntities =
        {
            new ChangeEntity(new SimpleChange(entry1Id)
                {
                    Values =
                    {
                        { nameof(Entry.Value), "Hello" },
                        { nameof(Entry.FirstUsed), new DateTime(2021, 1, 1) }
                    }
                }
            ),
            new ChangeEntity(
                new SetAgeChange(entry2Id, 4)
            )
        }
    },
    new Commit
    {
        ClientId = client1Id,
        DateTime = new DateTime(2000, 1, 15),
        ChangeEntities =
        {
            new ChangeEntity(
                new SimpleChange(entry1Id)
                {
                    Values =
                    {
                        { nameof(Entry.Age), 20 }
                    }
                })
        }
    },
    new Commit
    {
        ClientId = client1Id,
        DateTime = new DateTime(2000, 1, 30),
        ChangeEntities =
        {
            new ChangeEntity(new DeleteChange<Entry>(entry1Id))
        }
    }
});
var entry1 = await localModel.GetLatest<Entry>(entry1Id);
await localModel.Add(new Commit()
{
    ClientId = client1Id,
    DateTime = new DateTime(2000, 1, 16),
    ChangeEntities =
    {
        new ChangeEntity(
            new ChangeText(entry1, text => text.Insert(0, "Yo Jason"))
        )
    }
});
await localModel.PrintSnapshots();

Console.WriteLine("Insert change in history");
await using var remoteServices = new ServiceCollection()
    .AddCrdtDataSample(":memory:")
    .BuildServiceProvider();
var remoteContext = remoteServices.GetRequiredService<CrdtDbContext>();
await remoteContext.Database.OpenConnectionAsync();
await remoteContext.Database.EnsureCreatedAsync();
var remoteModel = remoteServices.GetRequiredService<DataModel>();
await remoteModel.AddRange(new[]
{
    new Commit
    {
        ClientId = client2Id,
        DateTime = new DateTime(2000, 1, 5),
        ChangeEntities =
        {
            new ChangeEntity(
            new SimpleChange(entry1Id)
            {
                Values =
                {
                    { nameof(Entry.Value), "offline change 1" },
                }
            })
        }
    },
    new Commit
    {
        ClientId = client2Id,
        DateTime = new DateTime(2000, 1, 7),
        ChangeEntities =
        {
            new ChangeEntity(
            new SimpleChange(entry1Id)
            {
                Values =
                {
                    { nameof(Entry.Value), "offline change two" },
                }
            })
        }
    }
});
await remoteModel.PrintSnapshots();


//perform a sync between local and remote
await localModel.SyncWith(remoteModel);

//sync finished
var localModelJson = JsonSerializer.Serialize(localModel);
var remoteModelJson = JsonSerializer.Serialize(remoteModel);
Console.WriteLine($"Compare local and remote, models are the same: {localModelJson == remoteModelJson}");

var dateTime = new DateTime(2000, 1, 8);
Console.WriteLine($"Snapshot at {dateTime}");
var snapshotAtTime = await localModel.GetEntitySnapshotAtTime(dateTime, entry1Id);
if (snapshotAtTime is not null) DataModel.PrintSnapshot(snapshotAtTime);

Console.WriteLine("Insert text");
await localModel.Add(new Commit
{
    ClientId = client1Id,
    DateTime = new DateTime(2000, 1, 17),
    ChangeEntities =
    {
        new ChangeEntity(
            new ChangeText(await localModel.GetLatest<Entry>(entry1Id), text => text.Insert(3, "What's up "))
        )
    }
});
await localModel.PrintSnapshots();