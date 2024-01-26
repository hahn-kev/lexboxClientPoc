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
            new CrdtKernel.ChangeTypeListBuilder()
                .Add<DeleteChange<Entry>>()
                .Add<SimpleChange>()
                .Add<MakeCommentChange>()
                .Add<ChangeText>()
                .Add<AddReferenceChange>()
                .Add<CommentOnEntryChange>(),
            new CrdtKernel.ObjectTypeListBuilder().Add<Entry>().Add<Comment>());
        return services;
    }
}