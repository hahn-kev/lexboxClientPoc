﻿using CrdtSample.Models;
using CrdtLib;

namespace Tests;

public class SyncTests : IAsyncLifetime
{
    private readonly DataModelTestBase _client1 = new();
    private readonly DataModelTestBase _client2 = new();

    public async Task InitializeAsync()
    {
        await _client1.InitializeAsync();
        await _client2.InitializeAsync();
    }

    public async Task DisposeAsync()
    {
        await _client1.DisposeAsync();
        await _client2.DisposeAsync();
    }

    [Fact]
    public async Task GetNewHistoryReturnsAddedChanges()
    {
        var entity1Id = Guid.NewGuid();
        var commit = await _client1.WriteNextChange(_client1.SimpleChange(entity1Id, "entity1"));
        var syncResults = await _client1.DataModel.SyncWith(_client2.DataModel);
        syncResults.MissingFromRemote.Should().ContainSingle(c => c.Id == commit.Id);
    }

    [Fact]
    public async Task CanSyncSimpleChange()
    {
        var entity1Id = Guid.NewGuid();
        var entity2Id = Guid.NewGuid();
        await _client1.WriteNextChange(_client1.SimpleChange(entity1Id, "entity1"));
        (await _client1.DataModel.GetLatest<Entry>(entity1Id)).Value.Should().Be("entity1");
        await _client2.WriteNextChange(_client2.SimpleChange(entity2Id, "entity2"));
        (await _client2.DataModel.GetLatest<Entry>(entity2Id)).Value.Should().Be("entity2");

        //act
        await _client1.DataModel.SyncWith(_client2.DataModel);

        _client2.DbContext.Snapshots.Should().NotBeEmpty();
        var client1Snapshot = await _client1.DataModel.GetSnapshot();
        var client2Snapshot = await _client2.DataModel.GetSnapshot();
        client1Snapshot.LastCommitHash.Should().Be(client2Snapshot.LastCommitHash);
        var entity = await _client2.DataModel.LoadObject(client2Snapshot.Snapshots[entity1Id].Id);
        var client2Entity1 = entity.Is<Entry>();
        client2Entity1.Value.Should().Be("entity1");
        var entity1 = await _client1.DataModel.LoadObject(client1Snapshot.Snapshots[entity2Id].Id);
        var client1Entity2 = entity1.Is<Entry>();
        client1Entity2.Value.Should().Be("entity2");
    }

    [Fact]
    public async Task CanSyncMultipleTimes()
    {
        var entity1Id = Guid.NewGuid();
        var entity2Id = Guid.NewGuid();
        await _client1.WriteNextChange(_client1.SimpleChange(entity1Id, "entity1"));
        await _client1.DataModel.SyncWith(_client2.DataModel);

        await _client2.WriteNextChange(_client2.SimpleChange(entity2Id, "entity2"));
        await _client1.WriteNextChange(_client1.SimpleChange(entity1Id, "entity1.1"));

        //act
        await _client1.DataModel.SyncWith(_client2.DataModel);

        var client2Entity = await _client2.DataModel.GetLatest<Entry>(entity1Id);
        client2Entity.Value.Should().Be("entity1.1");
        var client1Entity = await _client1.DataModel.GetLatest<Entry>(entity1Id);
        client1Entity.Value.Should().Be("entity1.1");
    }

    [Theory]
    [InlineData(10)]
    public async Task SyncMultipleClientChanges(int clientCount)
    {
        //for this test client1 will be the server
        var entity1Id = Guid.NewGuid();
        await _client1.WriteNextChange(_client1.SimpleChange(entity1Id, "entity1"));
        var clients = Enumerable.Range(0, clientCount).Select(_ => new DataModelTestBase()).ToArray();
        for (var i = 0; i < clients.Length; i++)
        {
            var client = clients[i];
            await client.InitializeAsync();
            client.SetCurrentDate(new DateTime(2001, 1, 1).AddDays(i));
            await client.WriteNextChange(client.SimpleChange(Guid.NewGuid(), "entityClient" + i));
        }

        await _client1.DataModel.SyncMany(clients.Select(c => c.DataModel).ToArray());

        var serverSnapshot = await _client1.DataModel.GetSnapshot();
        serverSnapshot.Snapshots.Should().HaveCount(clientCount + 1);
        foreach (var entitySnapshot in serverSnapshot.Snapshots.Values)
        {
            var entity1 = await _client1.DataModel.LoadObject(entitySnapshot.Id);
            var serverEntity = entity1.Is<Entry>();
            foreach (var client in clients)
            {
                var clientSnapshot = await client.DataModel.GetSnapshot();
                var simpleSnapshot = clientSnapshot.Snapshots.Should().ContainKey(entitySnapshot.EntityId).WhoseValue;
                var entity2 = await client.DataModel.LoadObject(simpleSnapshot.Id);
                var entity = entity2.Is<Entry>();
                  entity.Should().BeEquivalentTo(serverEntity);
            }
        }
    }
}