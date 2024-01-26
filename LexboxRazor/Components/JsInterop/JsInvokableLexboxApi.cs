using lexboxClientContracts;
using Microsoft.JSInterop;
using SystemTextJsonPatch;

namespace LexboxRazor.Components.JsInterop;

public class JsInvokableLexboxApi(ILexboxApi implementation) : ILexboxApi
{
    [JSInvokable]
    public Task<IEntry> CreateEntry(IEntry entry)
    {
        return implementation.CreateEntry(entry);
    }

    [JSInvokable]
    public Task<IExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, IExampleSentence exampleSentence)
    {
        return implementation.CreateExampleSentence(entryId, senseId, exampleSentence);
    }

    [JSInvokable]
    public Task<ISense> CreateSense(Guid entryId, ISense sense)
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
    public Task<IEntry[]> GetEntries(string exemplar, QueryOptions? options = null)
    {
        return implementation.GetEntries(exemplar, options);
    }

    [JSInvokable]
    public Task<IEntry[]> GetEntries(QueryOptions? options = null)
    {
        return implementation.GetEntries(options);
    }

    [JSInvokable]
    public Task<IEntry> GetEntry(Guid id)
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
    public Task<IEntry[]> SearchEntries(string query, QueryOptions? options = null)
    {
        return implementation.SearchEntries(query, options);
    }

    public Task<IEntry> UpdateEntry(Guid id, UpdateObjectInput<IEntry> update)
    {
        return implementation.UpdateEntry(id, update);
    }

    [JSInvokable]
    public Task<IEntry> UpdateEntry(Guid id, JsonPatchDocument<IEntry> patchDocument)
    {
        return UpdateEntry(id, new JsonPatchUpdateObjectInput<IEntry>(patchDocument));
    }

    public Task<IExampleSentence> UpdateExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId, UpdateObjectInput<IExampleSentence> update)
    {
        return implementation.UpdateExampleSentence(entryId, senseId, exampleSentenceId, update);
    }

    [JSInvokable]
    public Task<IExampleSentence> UpdateExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId, JsonPatchDocument<IExampleSentence> patchDocument)
    {
        return UpdateExampleSentence(entryId, senseId, exampleSentenceId, new JsonPatchUpdateObjectInput<IExampleSentence>(patchDocument));
    }

    public Task<ISense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<ISense> update)
    {
        return implementation.UpdateSense(entryId, senseId, update);
    }

    [JSInvokable]
    public Task<ISense> UpdateSense(Guid entryId, Guid senseId, JsonPatchDocument<ISense> update)
    {
        return UpdateSense(entryId, senseId, new JsonPatchUpdateObjectInput<ISense>(update));
    }
}
