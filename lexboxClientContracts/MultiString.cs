using System.Collections;
using System.Text.Json.Serialization;

namespace lexboxClientContracts;

[JsonDerivedType(typeof(MultiString), "ms")]
public interface IMultiString
{
    IDictionary<WritingSystemId, string> Values { get; }
}

/// <summary>
/// map like object with writing system as key and string as value
/// </summary>
public class MultiString : IMultiString
{
    public MultiString()
    {
        Values = new MultiStringDict();
    }

    [JsonConstructor]
    public MultiString(IDictionary<WritingSystemId, string> values)
    {
        Values = new MultiStringDict(values);
    }

    public IDictionary<WritingSystemId, string> Values { get; }

    private class MultiStringDict : Dictionary<WritingSystemId, string>, IDictionary
    {
        public MultiStringDict()
        {
        }

        public MultiStringDict(IDictionary<WritingSystemId, string> dictionary) : base(dictionary)
        {
        }

        // this method gets called by json patch when applying to an object.
        // however the default dictionary implementation fails when trying to cast so we need to override it
        void IDictionary.Add(object key, object value)
        {
            var valStr = value as string ??
                         throw new ArgumentException("unable to convert value to string", nameof(value));
            if (key is WritingSystemId keyWs)
            {
                Add(keyWs, valStr);
            }
            else if (key is string keyStr)
            {
                Add(keyStr, valStr);
            }
            else
            {
                throw new ArgumentException("unable to convert key to writing system id", nameof(key));
            }
        }
    }
}