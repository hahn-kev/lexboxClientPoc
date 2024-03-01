using System.Diagnostics;
using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib;
using CrdtLib.Changes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CrdtSample;

public static class CrdtSampleKernel
{
    public static IServiceCollection AddCrdtDataSample(this IServiceCollection services,
        string dbPath,
        bool enableProjectedTables = true)
    {
        services.AddCrdtData(
            builder =>
            {
                builder.UseSqlite($"Data Source={dbPath}");
                builder.EnableDetailedErrors();
                builder.EnableSensitiveDataLogging();
                #if DEBUG
                builder.LogTo(s => Debug.WriteLine(s));
                #endif
            },
            config =>
            {
                config.EnableProjectedTables = enableProjectedTables;
                config.ChangeTypeListBuilder.Add<DeleteChange<Entry>>()
                    .Add<SimpleChange>()
                    .Add<SetAgeChange>()
                    .Add<UpVoteCommentChange>()
                    .Add<ChangeText>()
                    .Add<AddReferenceChange>()
                    .Add<CommentOnEntryChange>();
                config.ObjectTypeListBuilder.Add<Entry>().Add<Comment>().Add<UpVote>();
            });
        return services;
    }
}