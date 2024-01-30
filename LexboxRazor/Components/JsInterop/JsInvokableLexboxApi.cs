using lexboxClientContracts;
using Microsoft.JSInterop;
using SystemTextJsonPatch;

namespace LexboxRazor.Components.JsInterop;

public class JsInvokableLexboxApi(ILexboxApi implementation) : ILexboxApi
{
    [JSInvokable]
    public Task<Entry> CreateEntry(Entry entry)
    {
        return implementation.CreateEntry(entry);
    }

    [JSInvokable]
    public Task<ExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, ExampleSentence exampleSentence)
    {
        return implementation.CreateExampleSentence(entryId, senseId, exampleSentence);
    }

    [JSInvokable]
    public Task<Sense> CreateSense(Guid entryId, Sense sense)
    {
        return implementation.CreateSense(entryId, sense);
    }

    public UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task DeleteEntry(Guid id)
    {
        return implementation.DeleteEntry(id);
    }

    [JSInvokable]
    public Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId)
    {
        return implementation.DeleteExampleSentence(entryId, senseId, exampleSentenceId);
    }

    [JSInvokable]
    public Task DeleteSense(Guid entryId, Guid senseId)
    {
        return implementation.DeleteSense(entryId, senseId);
    }

    [JSInvokable("GetEntriesForExemplar")]
    public Task<Entry[]> GetEntries(string exemplar, QueryOptions? options = null)
    {
        return implementation.GetEntries(exemplar, options);
    }

    [JSInvokable]
    public Task<Entry[]> GetEntries(QueryOptions? options = null)
    {
        return implementation.GetEntries(options);
    }

    [JSInvokable]
    public Task<Entry> GetEntry(Guid id)
    {
        return implementation.GetEntry(id);
    }

    [JSInvokable]
    public Task<string[]> GetExemplars()
    {
        return implementation.GetExemplars();
    }

    [JSInvokable]
    public Task<WritingSystems> GetWritingSystems()
    {
        return implementation.GetWritingSystems();
    }

    [JSInvokable]
    public Task<Entry[]> SearchEntries(string query, QueryOptions? options = null)
    {
        return implementation.SearchEntries(query, options);
    }

    public Task<Entry> UpdateEntry(Guid id, UpdateObjectInput<Entry> update)
    {
        return implementation.UpdateEntry(id, update);
    }

    [JSInvokable]
    public Task<Entry> UpdateEntry(Guid id, JsonPatchDocument<Entry> patchDocument)
    {
        return UpdateEntry(id, new JsonPatchUpdateInput<Entry>(patchDocument));
    }

    public Task<ExampleSentence> UpdateExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId, UpdateObjectInput<ExampleSentence> update)
    {
        return implementation.UpdateExampleSentence(entryId, senseId, exampleSentenceId, update);
    }

    [JSInvokable]
    public Task<ExampleSentence> UpdateExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId, JsonPatchDocument<ExampleSentence> patchDocument)
    {
        return UpdateExampleSentence(entryId, senseId, exampleSentenceId, new JsonPatchUpdateInput<ExampleSentence>(patchDocument));
    }

    public Task<Sense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<Sense> update)
    {
        return implementation.UpdateSense(entryId, senseId, update);
    }

    [JSInvokable]
    public Task<Sense> UpdateSense(Guid entryId, Guid senseId, JsonPatchDocument<Sense> update)
    {
        return UpdateSense(entryId, senseId, new JsonPatchUpdateInput<Sense>(update));
    }
}
