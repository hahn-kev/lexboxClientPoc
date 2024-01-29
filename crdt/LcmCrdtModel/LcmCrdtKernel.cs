using CrdtLib;
using CrdtLib.Changes;
using LcmCrdtModel.Changes;
using lexboxClientContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LcmCrdtModel;

public static class LcmCrdtKernel
{
    public static (CrdtKernel.ChangeTypeListBuilder changeTypes, CrdtKernel.ObjectTypeListBuilder objectTypes, CrdtKernel.ComplexTypeListBuilder complexTypes)
        PolyTypeListBuilder()
    {
        var objectTypes = new CrdtKernel.ObjectTypeListBuilder()
            .Add<Entry>()
            .Add<Sense>()
            .Add<ExampleSentence>();
        var changeTypes = new CrdtKernel.ChangeTypeListBuilder()
            .Add<JsonPatchChange<Entry>>()
            .Add<JsonPatchChange<Sense>>()
            .Add<JsonPatchChange<ExampleSentence>>()
            .Add<DeleteChange<Entry>>()
            .Add<DeleteChange<Sense>>()
            .Add<DeleteChange<ExampleSentence>>()
            .Add<CreateEntryChange>()
            .Add<CreateSenseChange>()
            .Add<CreateExampleSentenceChange>();
        var complexTypes = new CrdtKernel.ComplexTypeListBuilder()
            .Add<IMultiString, MultiString>();
        return (changeTypes, objectTypes, complexTypes);
    }

    public static IServiceCollection AddLcmCrdtClient(this IServiceCollection services, string dbPath)
    {
        var (changeTypes, objectTypes, complexTypes) = PolyTypeListBuilder();
        services.AddCrdtData(
            builder => builder.UseSqlite($"Data Source={dbPath}"),
            changeTypes,
            objectTypes,
            complexTypes
        );
        services.AddSingleton<ILexboxApi, CrdtLexboxApi>();
        return services;
    }
}