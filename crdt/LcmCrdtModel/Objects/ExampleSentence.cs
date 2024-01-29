using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using CrdtLib.Db;
using CrdtLib.Entities;
using lexboxClientContracts;

namespace LcmCrdtModel.Objects;

public class ExampleSentence : lexboxClientContracts.ExampleSentence, IObjectBase<ExampleSentence>
{
    Guid IObjectBase.Id
    {
        get => Id;
        init => Id = value;
    }

    public required Guid SenseId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public Guid[] GetReferences()
    {
        return [SenseId];
    }

    public void RemoveReference(Guid id, Commit commit)
    {
        if (id == SenseId)
            DeletedAt = commit.DateTime;
    }

    public IObjectBase Copy()
    {
        return JsonSerializer.Deserialize<ExampleSentence>(JsonSerializer.Serialize(this), new JsonSerializerOptions
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    static typeInfo =>
                    {
                        if (typeInfo.Type == typeof(IMultiString))
                        {
                                typeInfo.CreateObject = () => new MultiString();
                        }
                    }
                }
            }
        })!;
    }
}