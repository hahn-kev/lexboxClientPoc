using System.Linq.Expressions;
using CrdtLib.Db;
using Microsoft.EntityFrameworkCore;

namespace CrdtLib.Changes;

public class ChangeContext(Commit commit, SnapshotWorker worker, CrdtRepository crdtRepository)
{
    public Commit Commit { get; } = commit;
    public async ValueTask<ObjectSnapshot?> GetSnapshot(Guid entityId) => await worker.GetSnapshot(entityId);

    public async ValueTask<bool> IsObjectDeleted(Guid entityId) => (await GetSnapshot(entityId))?.EntityIsDeleted ?? true;

    public async ValueTask MarkDeleted(Guid deletedEntityId)
    {
        Expression<Func<ObjectSnapshot, bool>> predicateExpression = snapshot => snapshot.References.Contains(deletedEntityId);
        var predicate = predicateExpression.Compile();

        var toRemoveRefFromIds = new HashSet<Guid>(await crdtRepository.CurrentSnapshots()
            .Where(predicateExpression)
            .Select(s => s.EntityId)
            .ToArrayAsync());
        //snapshots from the db might be out of date, we want to use the most up to date data in the worker as well
        toRemoveRefFromIds.UnionWith(worker.PendingSnapshots.Values.Where(predicate).Select(s => s.EntityId));
        foreach (var entityId in toRemoveRefFromIds)
        {
            var snapshot = await GetSnapshot(entityId);
            if (snapshot is null) throw new NullReferenceException("unable to find snapshot for entity " + entityId);
            //could be different from what's in the db if a previous change has already updated it
            if (!predicate(snapshot)) continue;
            var updatedEntry = snapshot.Entity.Copy();
            var wasDeleted = updatedEntry.DeletedAt.HasValue;

            updatedEntry.RemoveReference(deletedEntityId, Commit);
            var deletedByRemoveRef = !wasDeleted && updatedEntry.DeletedAt.HasValue;

            worker.AddSnapshot(new ObjectSnapshot(updatedEntry, Commit, false));

            //we need to do this after we add the snapshot above otherwise we might get stuck in a loop of deletions
            if (deletedByRemoveRef)
            {
                await MarkDeleted(updatedEntry.Id);
            }
        }
    }
}