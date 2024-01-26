using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CrdtSample.Models;
using CrdtLib.Changes;
using CrdtLib.Entities;
using Ycs;

namespace CrdtSample.Changes;

public class ChangeText : SingleObjectChange<Entry>, ISelfNamedType<ChangeText>
{
    public string UpdateBlob { get; set; }

    [JsonConstructor]
    public ChangeText(Guid entityId, string updateBlob) : base(entityId)
    {
        UpdateBlob = updateBlob;
    }

    public ChangeText(Entry entry, Action<YText> change): base(entry.Id)
    {
        var text = entry.YText;
        var stateBefore = text.Doc.EncodeStateVectorV2();
        change(text);
        UpdateBlob = Convert.ToBase64String(text.Doc.EncodeStateAsUpdateV2(stateBefore));
    }

    public override void ApplyChange(Entry entry)
    {
        entry.YText.Doc.ApplyUpdateV2(Convert.FromBase64String(UpdateBlob));
    }
}