using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using CrdtSample.Models;

namespace CrdtSample.Changes;

public class NewWordChange(Guid entityId, string text) : Change<Word>(entityId), ISelfNamedType<NewWordChange>
{
    public string Text { get; } = text;

    public override IObjectBase NewEntity(Commit commit)
    {
        return new Word
        {
            Text = Text,
            Id = EntityId
        };
    }

    public override ValueTask ApplyChange(Word entity, ChangeContext context)
    {
        entity.Text = Text;
        return ValueTask.CompletedTask;
    }
}