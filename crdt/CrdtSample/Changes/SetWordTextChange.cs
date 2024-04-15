using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Entities;
using CrdtSample.Models;

namespace CrdtSample.Changes;

public class SetWordTextChange(Guid entityId, string text) : Change<Word>(entityId), ISelfNamedType<SetWordTextChange>
{
    public string Text { get; } = text;

    public override IObjectBase NewEntity(Commit commit)
    {
        throw new System.NotImplementedException();
    }

    public override async ValueTask ApplyChange(Word entity, ChangeContext context)
    {
        entity.Text = Text;
    }
}