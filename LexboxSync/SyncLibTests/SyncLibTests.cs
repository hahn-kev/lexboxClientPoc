using System;
using CrdtLib;
using CrdtLib.Db;
using AppLayer.Api;
using LcmCrdtModel;
using lexboxClientContracts;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using SIL.WritingSystems;

namespace SyncLibTests;

public class SyncLibTests : IAsyncLifetime
{
    private ILexboxApi lcmApi;
    private ILexboxApi crdtApi;

    protected readonly ServiceProvider _services;
    public readonly DataModel DataModel;
    private CrdtDbContext _crdtDbContext;

    public SyncLibTests()
    {
        _services = new ServiceCollection()
            .AddLcmCrdtClient(":memory:")
            .BuildServiceProvider();
        _crdtDbContext = _services.GetRequiredService<CrdtDbContext>();
        DataModel = _services.GetRequiredService<DataModel>();
        crdtApi = new CrdtLexboxApi(DataModel, _services.GetRequiredService<JsonSerializerOptions>());
    }

    public virtual async Task InitializeAsync()
    {
        lcmApi = await LexboxLcmApiFactory.CreateApi("C:/ProgramData/SIL/FieldWorks/Projects/test-chris-02/test-chris-02.fwdata", false);
        await _crdtDbContext.Database.OpenConnectionAsync();
        await _crdtDbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _services.DisposeAsync();
    }

    [Fact]
    public async Task MigrateAllEntriesToNewCrdt()
    {
        var crdtEntries = await crdtApi.GetEntries();
        crdtEntries.Should().BeEmpty();

        var lcmEntries = (await lcmApi.GetEntries()).Take(10);
        lcmEntries.Count().Should().Be(10);

        foreach (var entry in lcmEntries)
        {
            await crdtApi.CreateEntry(entry);
        }

        crdtEntries.Length.Should().Be(lcmEntries.Count());
        JsonSerializer.Serialize(lcmEntries).Should().Be(JsonSerializer.Serialize(crdtEntries));
    }
}