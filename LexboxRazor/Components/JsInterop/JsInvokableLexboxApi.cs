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
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task<ISense> CreateSense(Guid entryId, ISense sense)
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task DeleteEntry(Guid id)
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId)
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task DeleteSense(Guid entryId, Guid senseId)
    {
        throw new NotImplementedException();
    }

    [JSInvokable("GetEntriesForExemplar")]
    public Task<IEntry[]> GetEntries(string exemplar, QueryOptions? options = null)
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task<IEntry[]> GetEntries(QueryOptions? options = null)
    {
        return implementation.GetEntries(options);
    }

    [JSInvokable]
    public Task<IEntry> GetEntry(Guid id)
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task<string[]> GetExemplars()
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task<WritingSystem[]> GetWritingSystems()
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task<IEntry[]> SearchEntries(string query, QueryOptions? options = null)
    {
        throw new NotImplementedException();
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

    [JSInvokable]
    public Task<IExampleSentence> UpdateExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId, UpdateObjectInput<IExampleSentence> update)
    {
        throw new NotImplementedException();
    }

    [JSInvokable]
    public Task<ISense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<ISense> update)
    {
        throw new NotImplementedException();
    }
}
