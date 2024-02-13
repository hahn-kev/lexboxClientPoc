using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib;
using CrdtLib.Changes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CrdtSample;

public static class CrdtSampleKernel
{
    public static IServiceCollection AddCrdtDataSample(this IServiceCollection services, string dbPath)
    {
        services.AddCrdtData(
            builder => builder.UseSqlite($"Data Source={dbPath}"),
            config =>
            {
                config.ChangeTypeListBuilder.Add<DeleteChange<Entry>>()
                    .Add<SimpleChange>()
                    .Add<SetAgeChange>()
                    .Add<ChangeText>()
                    .Add<AddReferenceChange>()
                    .Add<CommentOnEntryChange>();
                config.ObjectTypeListBuilder.Add<Entry>().Add<Comment>();
            });
        return services;
    }
}