using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using CrdtSample.Models;
using CrdtLib.Changes;
using CrdtLib.Entities;

namespace CrdtSample.Changes;

public class SimpleChange : SingleObjectChange<Entry>, ISelfNamedType<SimpleChange>
{
    public SimpleChange(Guid entityId) : base(entityId)
    {
    }

    public Dictionary<string, object> Values { get; set; } = new();

    public override void ApplyChange(Entry entry)
    {
        var type = entry.GetType();
        foreach (var (key, value) in Values)
        {
            var propertyInfo = type.GetProperty(key);
            if (propertyInfo is null) continue;
            if (value is JsonElement element)
            {
                propertyInfo.SetValue(entry, element.Deserialize(propertyInfo.PropertyType));
            }
            else
            {
                propertyInfo.SetValue(entry, value);
            }
        }
    }
}