using System.Diagnostics.CodeAnalysis;
using CrdtSample.Models;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;

namespace CrdtSample.Changes;

public class AddReferenceChange : Change<Entry>, ISelfNamedType<AddReferenceChange>
{
    public Guid ReferenceId { get; }
    public AddReferenceChange(Guid entityId, Guid referenceId): base(entityId)
    {
        ReferenceId = referenceId;
    }

    public override async ValueTask ApplyChange(Entry entity, ChangeContext context)
    {
        if (!await context.IsObjectDeleted(ReferenceId))
            entity.EntryReference = ReferenceId;
    }

    public override IObjectBase NewEntity(Commit commit)
    {
        return Entry.New(EntityId, commit);
    }
}