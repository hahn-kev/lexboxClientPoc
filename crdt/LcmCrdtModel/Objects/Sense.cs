using CrdtLib.Db;
using CrdtLib.Entities;

namespace LcmCrdtModel.Objects;

public class Sense : lexboxClientContracts.Sense, IObjectBase<Sense>
{
    Guid IObjectBase.Id
    {
        get => Id;
        init => Id = value;
    }

    public DateTimeOffset? DeletedAt { get; set; }
    public required Guid EntryId { get; set; }

    public Guid[] GetReferences()
    {
        return [EntryId];
    }

    public void RemoveReference(Guid id, Commit commit)
    {
        if (id == EntryId)
            DeletedAt = commit.DateTime;
    }

    public IObjectBase Copy()
    {
        return new Sense
        {
            Id = Id,
            EntryId = EntryId,
            DeletedAt = DeletedAt,
            Definition = Definition.Copy(),
            Gloss = Gloss.Copy(),
            PartOfSpeech = PartOfSpeech,
            SemanticDomain = [..SemanticDomain]
        };
    }
}