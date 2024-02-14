using CrdtLib.Changes;
using CrdtLib.Entities;
using CrdtSample.Models;

namespace CrdtSample.Changes;

public class SetAgeChange: SingleObjectChange<Entry>, ISelfNamedType<SetAgeChange>
{
    public int Age { get; }

    public SetAgeChange(Guid entityId, int age) : base(entityId)
    {
        Age = age;
    }

    public override void ApplyChange(Entry value)
    {
        value.Age = Age;
    }
}