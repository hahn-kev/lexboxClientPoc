using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrdtLib;

/// <summary>
/// helper service to update snapshots and apply commits to them, has mutable state, don't reuse
/// </summary>
public class SnapshotWorker
{
    private readonly IReadOnlyDictionary<Guid, SimpleSnapshot>? _snapshots;
    private readonly CrdtRepository _crdtRepository;
    private readonly SimpleSnapshot? _oldestSnapshot;
    public Dictionary<Guid, ObjectSnapshot> PendingSnapshots { get; } = new();

    public SnapshotWorker(Dictionary<Guid, ObjectSnapshot> snapshots, CrdtRepository crdtRepository)
    {
        PendingSnapshots = snapshots;
        var oldestSnapshot = snapshots.Values.MinBy(s => s.Commit.CompareKey);
        _oldestSnapshot = oldestSnapshot is null ? null : new SimpleSnapshot(oldestSnapshot);
        _crdtRepository = crdtRepository;
    }

    public SnapshotWorker(IReadOnlyDictionary<Guid, SimpleSnapshot> snapshots, CrdtRepository crdtRepository)
    {
        _snapshots = snapshots;
        _crdtRepository = crdtRepository;
        _oldestSnapshot = snapshots.Values.MinBy(s => (s.DateTime, s.CommitId));
    }

    public async Task UpdateSnapshots()
    {
        //need to use oldestCommitAppliedToAllSnapshots because some snapshots might not have changes that are newer
        //but before the oldestAddedCommit
        var commits = await _crdtRepository.CurrentCommits().Where(c =>
                _oldestSnapshot == null
                || (c.DateTime == _oldestSnapshot.DateTime &&
                    c.Id > _oldestSnapshot.CommitId)
                || c.DateTime > _oldestSnapshot.DateTime)
            .Include(c => c.ChangeEntities).ToArrayAsync();
        await ApplyCommitChanges(commits, true);

        await _crdtRepository.AddSnapshots(PendingSnapshots.Values);
    }

    public async ValueTask ApplyCommitChanges(ICollection<Commit> commits, bool updateCommitHash)
    {
        var commitIndex = 0;
        var previousCommitHash = _oldestSnapshot?.CommitHash;
        foreach (var commit in commits)
        {
            if (updateCommitHash && previousCommitHash is not null)
            {
                //we're rewriting history, so we need to update the previous commit hash
                commit.SetParentHash(previousCommitHash);
            }

            previousCommitHash = commit.Hash;
            commitIndex++;
            foreach (var commitChange in commit.ChangeEntities)
            {
                IObjectBase entity;
                var snapshot = await GetSnapshot(commitChange.EntityId);
                var changeContext = new ChangeContext(commit, this, _crdtRepository);
                if (snapshot is not null)
                {
                    entity = snapshot.Entity.Copy();
                }
                else
                {
                    entity = commitChange.Change.NewEntity(commit);
                }

                await commitChange.Change.ApplyChange(entity, changeContext);
                //to get the state in a point in time we would have to find a snapshot before that time, then apply any commits that came after that snapshot but still before the point in time.
                //we would probably want the most recent snapshot to always follow current, so we might need to track the number of changes a given snapshot represents so we can 
                //decide when to create a new snapshot instead of replacing one inline. This would be done by using the current snapshots parent, instead of the snapshot itself.
                // s0 -> s1 -> sCurrent
                // if always taking snapshots would become
                // s0 -> s1 -> sCurrent -> sNew
                //but but to not snapshot every change we could do this instead
                // s0 -> s1 -> sNew

                //for now just skip every other change
                if (snapshot is not null && (snapshot.IsRoot || commitIndex % 2 == 0))
                {
                    _crdtRepository.AddIfNew(snapshot);
                }

                var newSnapshot = new ObjectSnapshot(entity, commit, snapshot is null);
                AddSnapshot(newSnapshot);
            }
        }
    }

    public async ValueTask<ObjectSnapshot?> GetSnapshot(Guid entityId)
    {
        if (PendingSnapshots.TryGetValue(entityId, out var snapshot))
        {
            return snapshot;
        }

        if (_snapshots?.TryGetValue(entityId, out var simpleSnapshot) == true)
        {
            return await _crdtRepository.FindSnapshot(simpleSnapshot.Id);
        }

        return null;
    }
    
    public void AddSnapshot(ObjectSnapshot snapshot)
    {
        PendingSnapshots[snapshot.Entity.Id] = snapshot;
    }
}