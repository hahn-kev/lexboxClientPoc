using System.Text.Json;
using CrdtSample;
using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Helpers;
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
        HybridDateTime = new HybridDateTime(new DateTime(2000, 1, 1), 0),
        ChangeEntities =
        {
            new ChangeEntity(new NewWordChange(entry1Id, "hello", "a note")),
            new ChangeEntity(new SetWordTextChange(entry2Id, "word2"))
        }
    },
    new Commit
    {
        ClientId = client1Id,
        HybridDateTime = new HybridDateTime(new DateTime(2000, 1, 15), 0),
        ChangeEntities =
        {
            new ChangeEntity(new SetWordNoteChange(entry1Id, "updated note"))
        }
    },
    new Commit
    {
        ClientId = client1Id,
        HybridDateTime = new HybridDateTime(new DateTime(2000, 1, 30), 0),
        ChangeEntities =
        {
            new ChangeEntity(new DeleteChange<Word>(entry1Id))
        }
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
        HybridDateTime = new HybridDateTime(new DateTime(2000, 1, 5), 0),
        ChangeEntities =
        {
            new ChangeEntity(new SetWordTextChange(entry1Id, "offline change 1"))
        }
    },
    new Commit
    {
        ClientId = client2Id,
        HybridDateTime = new HybridDateTime(new DateTime(2000, 1, 7), 0),
        ChangeEntities =
        {
            new ChangeEntity(new SetWordTextChange(entry1Id, "offline change two"))
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
