using System.Linq.Expressions;
using SystemTextJsonPatch;

namespace lexboxClientContracts;

public class JsonPatchUpdateBuilder<T> : UpdateBuilder<T> where T : class
{
    private readonly JsonPatchDocument<T> _patchDocument = new();

    public UpdateObjectInput<T> Build()
    {
        return new JsonPatchUpdateObjectInput<T>(_patchDocument);
    }

    public UpdateBuilder<T> Set<T_Val>(Expression<Func<T, T_Val>> field, T_Val value)
    {
        _patchDocument.Replace(field, value);
        return this;
    }
}

public class JsonPatchUpdateObjectInput<T>(JsonPatchDocument<T> patchDocument) : UpdateObjectInput<T>
    where T : class
{
    public void Apply(T obj)
    {
        patchDocument.ApplyTo(obj);
    }
}