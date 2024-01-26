using System.Text.Json.Serialization;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using lexboxClientContracts;

namespace LcmCrdtModel.Changes;


public class CreateEntryChange : Change<Entry>, ISelfNamedType<CreateEntryChange>
{
    public CreateEntryChange(IEntry entry) : base(entry.Id == Guid.Empty ? Guid.NewGuid() : entry.Id)
    {
        entry.Id = EntityId;
        LexemeForm = entry.LexemeForm;
        CitationForm = entry.CitationForm;
        LiteralMeaning = entry.LiteralMeaning;
        Note = entry.Note;
    }
    
    [JsonConstructor]
    private CreateEntryChange(Guid entityId) : base(entityId)
    {
    }

    public IMultiString? LexemeForm { get; set; }

    public IMultiString? CitationForm { get; set; }

    public IMultiString? LiteralMeaning { get; set; }

    public IMultiString? Note { get; set; }

    public override IObjectBase NewEntity(Commit commit)
    {
        return new Entry
        {
            Id = EntityId,
            LexemeForm = LexemeForm ?? new MultiString(),
            CitationForm = CitationForm ?? new MultiString(),
            LiteralMeaning = LiteralMeaning ?? new MultiString(),
            Note = Note ?? new MultiString()
        };
    }

    public override ValueTask ApplyChange(Entry entity, ChangeContext context)
    {
        if (LexemeForm is not null) entity.LexemeForm = LexemeForm;
        if (CitationForm is not null) entity.CitationForm = CitationForm;
        if (LiteralMeaning is not null) entity.LiteralMeaning = LiteralMeaning;
        if (Note is not null) entity.Note = Note;
        return ValueTask.CompletedTask;
    }
}