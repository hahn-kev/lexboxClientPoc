using System.Linq.Expressions;
using System.Text.Json;
using SystemTextJsonPatch;

namespace lexboxClientContracts;

public interface ILexboxApi
{
    Task<WritingSystem[]> GetWritingSystems();
    Task<string[]> GetExemplars();
    Task<Entry[]> GetEntries(string exemplar, QueryOptions? options = null);
    Task<Entry[]> GetEntries(QueryOptions? options = null);
    Task<Entry[]> SearchEntries(string query, QueryOptions? options = null);
    Task<Entry> GetEntry(Guid id);

    Task<Entry> CreateEntry(Entry entry);
    Task<Entry> UpdateEntry(Guid id, UpdateObjectInput<Entry> entry);
    Task DeleteEntry(Guid id);

    Task<Sense> CreateSense(Guid entryId, Sense sense);
    Task<Sense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<Sense> sense);
    Task DeleteSense(Guid entryId, Guid senseId);

    Task<ExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, ExampleSentence exampleSentence);
    Task<ExampleSentence> UpdateExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId, UpdateObjectInput<ExampleSentence> exampleSentence);
    Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId);
    
    UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class;
}

public record QueryOptions(string Order, int Count = 1000, int Offset = 0);

public interface UpdateObjectInput<T> where T : class
{
    void Apply(T obj);
}

public interface UpdateBuilder<T> where T : class
{
    UpdateBuilder<T> Set<T_Val>(Expression<Func<T, T_Val>> field, T_Val value);
    UpdateObjectInput<T> Build();
}