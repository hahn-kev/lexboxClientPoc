using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using SystemTextJsonPatch;

namespace lexboxClientContracts;

public interface ILexboxApi
{
    Task<WritingSystems> GetWritingSystems();
    Task<string[]> GetExemplars();
    Task<IEntry[]> GetEntries(string exemplar, QueryOptions? options = null);
    Task<IEntry[]> GetEntries(QueryOptions? options = null);
    Task<IEntry[]> SearchEntries(string query, QueryOptions? options = null);
    Task<IEntry> GetEntry(Guid id);

    Task<IEntry> CreateEntry(IEntry entry);
    Task<IEntry> UpdateEntry(Guid id, UpdateObjectInput<IEntry> update);
    Task DeleteEntry(Guid id);

    Task<ISense> CreateSense(Guid entryId, ISense sense);
    Task<ISense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<ISense> update);
    Task DeleteSense(Guid entryId, Guid senseId);

    Task<IExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, IExampleSentence exampleSentence);

    Task<IExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        UpdateObjectInput<IExampleSentence> update);

    Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId);

    UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class;
}

public record QueryOptions(string Order, int Count = 1000, int Offset = 0);

public interface UpdateObjectInput<T> where T : class
{
    void Apply(T obj);
    JsonPatchDocument<T> Patch { get; }
}

public class UpdateBuilder<T> where T : class
{
    private readonly JsonPatchDocument<T> _patchDocument = new();

    public UpdateObjectInput<T> Build()
    {
        return new JsonPatchUpdateInput<T>(_patchDocument);
    }

    public UpdateBuilder<T> Set<T_Val>(Expression<Func<T, T_Val>> field, T_Val value)
    {
        _patchDocument.Replace(field, value);
        return this;
    }
}