using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using CrdtSample.Models;
using CrdtLib.Changes;
using CrdtLib.Entities;

namespace CrdtSample.Changes;

public class MakeCommentChange : SingleObjectChange<Entry>, ISelfNamedType<MakeCommentChange>
{
    [JsonConstructor]
    public MakeCommentChange(Guid entityId, string comment) : base(entityId)
    {
        Comment = comment;
    }

    public MakeCommentChange(Guid entityId) : base(entityId)
    {
    }

    public string Comment { get; set; } = "";

    public override void ApplyChange(Entry entry)
    {
        entry.Comment = Comment;
    }
}