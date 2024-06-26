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
        _crdtDbContext.Database.OpenConnection();
        _crdtDbContext.Database.EnsureCreated();
        DataModel = _services.GetRequiredService<DataModel>();
        crdtApi = ActivatorUtilities.CreateInstance<CrdtLexboxApi>(_services);
    }

    public virtual async Task InitializeAsync()
    {
        lcmApi = await LexboxLcmApiFactory.CreateApi("C:\\ProgramData\\SIL\\FieldWorks\\Projects\\sena-3\\sena-3.fwdata", false);
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

        var lcmEntries = (await lcmApi.GetEntries())[..10];
        lcmEntries.Should().HaveCount(10);

        foreach (var entry in lcmEntries)
        {
            await crdtApi.CreateEntry(entry);
        }
        crdtEntries = await crdtApi.GetEntries();
        crdtEntries.Should().HaveSameCount(lcmEntries);
        //does a deep equality check
        crdtEntries.Should().BeEquivalentTo(lcmEntries);
    }
}