using System.Linq.Expressions;
using System.Text.Json;
using CrdtLib.Db;
using CrdtLib.Entities;
using CrdtLib.Changes;
using Microsoft.EntityFrameworkCore;

namespace CrdtLib;

public record SyncResults(List<Commit> MissingFromLocal, List<Commit> MissingFromRemote);
public record ChangesResult(List<Commit> MissingFromClient, SyncState ServerSyncState);
public interface ISyncable
{
    Task AddRangeFromSync(IEnumerable<Commit> commits);
    Task<SyncState> GetSyncState();
    Task<ChangesResult> GetChanges(SyncState otherHeads);
    Task<SyncResults> SyncWith(ISyncable remoteModel);
    Task SyncMany(ISyncable[] remotes);
}

public record SyncState(Dictionary<Guid, long> ClientHeads);

public class DataModel(CrdtRepository crdtRepository, JsonSerializerOptions serializerOptions) : ISyncable
{
    /// <summary>
    /// after adding any commit validate the commit history, not great for performance but good for testing.
    /// </summary>
    private readonly bool _autoValidate = true;

    public async Task Add(Commit commit)
    {
        if (await crdtRepository.HasCommit(commit.Id)) return;

        await using var transaction = await crdtRepository.BeginTransactionAsync();
        await crdtRepository.AddCommit(commit);
        await UpdateSnapshots(commit);
        if (_autoValidate) await ValidateCommits();
        await transaction.CommitAsync();
    }

    async Task ISyncable.AddRangeFromSync(IEnumerable<Commit> commits)
    {
        await AddRange(commits, true);
    }

    public async Task AddRange(IEnumerable<Commit> commits, bool forceValidate = false)
    {
        var (oldestChange, newCommits) = await crdtRepository.FilterExistingCommits(commits.ToArray());
        //no changes added
        if (oldestChange is null || newCommits is []) return;

        await using var transaction = await crdtRepository.BeginTransactionAsync();
        await crdtRepository.AddCommits(newCommits);
        await UpdateSnapshots(oldestChange);
        if (_autoValidate || forceValidate) await ValidateCommits();
        await transaction.CommitAsync();
    }

    private async Task UpdateSnapshots(Commit oldestAddedCommit)
    {
        await crdtRepository.DeleteStaleSnapshots(oldestAddedCommit);
        var modelSnapshot = await GetProjectSnapshot();
        var snapshotWorker = new SnapshotWorker(modelSnapshot.Snapshots, crdtRepository);
        await snapshotWorker.UpdateSnapshots();
    }

    public async Task ValidateCommits()
    {
        Commit? parentCommit = null;
        await foreach (var commit in crdtRepository.CurrentCommits().AsAsyncEnumerable())
        {
            var parentHash = parentCommit?.Hash ?? "";
            var expectedHash = commit.GenerateHash(parentHash);
            if (commit.Hash == expectedHash && commit.ParentHash == parentHash)
            {
                parentCommit = commit;
                continue;
            }

            var actualParentCommit = await crdtRepository.FindCommitByHash(commit.ParentHash);

            throw new CommitValidationException(
                $"Commit {commit} does not match expected hash, parent hash [{commit.ParentHash}] !== [{parentHash}], expected parent {parentCommit} and actual parent {actualParentCommit}");
        }
    }

    public async Task<ObjectSnapshot?> GetEntitySnapshotAtTime(DateTimeOffset time, Guid entityId)
    {
        var snapshots = await GetSnapshotsAt(time);
        return snapshots.GetValueOrDefault(entityId);
    }

    public async Task<ObjectSnapshot> GetLatestSnapshotByObjectId(Guid entityId)
    {
        return await crdtRepository.GetCurrentSnapshotByObjectId(entityId);
    }

    public async Task<T> GetLatest<T>(Guid objectId) where T : IObjectBase
    {
        return await crdtRepository.GetCurrent<T>(objectId);
    }

    public async Task<ModelSnapshot> GetProjectSnapshot()
    {
        return new ModelSnapshot(await GetEntitySnapshots());
    }

    public IQueryable<T> GetLatestObjects<T>() where T : class, IObjectBase
    {
        return crdtRepository.GetCurrentObjects<T>();
    }

    public async Task<IObjectBase> GetBySnapshotId(Guid snapshotId)
    {
        return await crdtRepository.GetObjectBySnapshotId(snapshotId);
    }

    private async Task<SimpleSnapshot[]> GetEntitySnapshots(bool includeDeleted = false)
    {
        var queryable = crdtRepository.CurrentSnapshots();
        if (!includeDeleted) queryable = queryable.Where(s => !s.EntityIsDeleted);
        var snapshots = await queryable.Select(s =>
            new SimpleSnapshot(s.Id,
                s.TypeName,
                s.EntityId,
                s.CommitId,
                s.IsRoot,
                s.Commit.DateTime,
                s.Commit.Hash,
                s.EntityIsDeleted)).ToArrayAsync();
        return snapshots;
    }

    public async Task<Dictionary<Guid, ObjectSnapshot>> GetSnapshotsAt(DateTimeOffset dateTime)
    {
        var repository = crdtRepository.GetScopedRepository(dateTime);
        var (snapshots, pendingCommits) = await repository.GetCurrentSnapshotsAndPendingCommits();
        
        if (pendingCommits.Length != 0)
        {
            snapshots = await SnapshotWorker.ApplyCommitsToSnapshots(snapshots, repository, pendingCommits);
        }

        return snapshots;
    }

    public async Task PrintSnapshots()
    {
        await foreach (var snapshot in crdtRepository.CurrentSnapshots().AsAsyncEnumerable())
        {
            PrintSnapshot(snapshot);
        }
    }

    public static void PrintSnapshot(ObjectSnapshot objectSnapshot)
    {
        Console.WriteLine($"Last change {objectSnapshot.Id},      {objectSnapshot.Entity}");
    }

    public async Task<SyncState> GetSyncState()
    {
        return await crdtRepository.GetCurrentSyncState();
    }

    public async Task<ChangesResult> GetChanges(SyncState remoteState)
    {
        return await crdtRepository.GetChanges(remoteState);
    }

    public async Task<SyncResults> SyncWith(ISyncable remoteModel)
    {
        return await SyncHelper.SyncWith(this, remoteModel, serializerOptions);
    }

    public async Task SyncMany(ISyncable[] remotes)
    {
        await SyncHelper.SyncMany(this, remotes, serializerOptions);
    }
}