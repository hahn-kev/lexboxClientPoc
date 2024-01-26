using System.Collections.ObjectModel;
using System.Linq.Expressions;
using CrdtLib.Db;
using CrdtLib.Entities;
using Microsoft.EntityFrameworkCore;

namespace CrdtLib.Changes;

public class ChangeContext(Commit commit, SnapshotWorker worker, CrdtRepository crdtRepository)
{
    public Commit Commit { get; } = commit;
    public async ValueTask<ObjectSnapshot?> GetSnapshot(Guid entityId) => await worker.GetSnapshot(entityId);

    public async ValueTask<bool> IsObjectDeleted(Guid entityId) => (await GetSnapshot(entityId))?.EntityIsDeleted ?? true;

    public async ValueTask MutateOthers(Expression<Func<ObjectSnapshot, bool>> predicateExpression, Action<IObjectBase> mutate)
    {
        var predicate = predicateExpression.Compile();
        var entityIds = new HashSet<Guid>(await crdtRepository.CurrentSnapshots().Where(predicateExpression).Select(s => s.EntityId)
            .ToArrayAsync());
        //snapshots from the db might be out of date, we want to use the most up to date data in the worker as well
        entityIds.UnionWith(worker.PendingSnapshots.Values.Where(predicate).Select(s => s.EntityId));
        foreach (var entityId in entityIds)
        {
            var snapshot = await GetSnapshot(entityId);
            if (snapshot is null) throw new NullReferenceException("unable to find snapshot for entity " + entityId);
            //could be different from what's in the db if a previous change has already updated it
            if (!predicate(snapshot)) continue;
            var updatedEntry = snapshot.Entity.Copy();
            mutate(updatedEntry);
            worker.AddSnapshot(new ObjectSnapshot(updatedEntry, Commit, false));
        }
    }
}