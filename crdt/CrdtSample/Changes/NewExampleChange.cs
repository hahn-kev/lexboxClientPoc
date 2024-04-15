using System.Text.Json.Serialization;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using CrdtSample.Models;
using Ycs;

namespace CrdtSample.Changes;

public class NewExampleChange : Change<Example>, ISelfNamedType<NewExampleChange>
{
    public NewExampleChange FromString(Guid definitionId, string example)
    {
        return FromAction(definitionId, text => text.Insert(0, example));
    }

    public NewExampleChange FromAction(Guid definitionId, Action<YText> change)
    {
        var doc = new YDoc();
        var stateBefore = doc.EncodeStateVectorV2();
        change(doc.GetText());
        var updateBlob = Convert.ToBase64String(doc.EncodeStateAsUpdateV2(stateBefore));
        return new NewExampleChange(Guid.NewGuid())
        {
            DefinitionId = definitionId,
            UpdateBlob = updateBlob
        };
    }

    public required Guid DefinitionId { get; init; }
    public required string UpdateBlob { get; set; }

    [JsonConstructor]
    private NewExampleChange(Guid entityId) : base(entityId)
    {
    }

    public override IObjectBase NewEntity(Commit commit)
    {
        return new Example
        {
            Id = EntityId,
            DefinitionId = DefinitionId,
            YTextBlob = UpdateBlob
        };
    }

    public override async ValueTask ApplyChange(Example entity, ChangeContext context)
    {
        entity.YTextBlob = UpdateBlob;
    }
}