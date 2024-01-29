extern alias tapper;
using System.Text.Json;
using lexboxClientContracts;
using Microsoft.AspNetCore.SignalR;
using SystemTextJsonPatch;
using SystemTextJsonPatch.Operations;
using TypedSignalR.Client;

namespace Lexbox.ClientServer.Hubs;

[tapper::Tapper.TranspilationSource]
public record JsonOperation(string Op, string Path, object? Value = null, string? From = null);

[Hub]
public interface ILexboxApiHub
{
    Task<WritingSystems> GetWritingSystems();
    Task<string[]> GetExemplars();
    Task<IEntry[]> GetEntriesForExemplar(string exemplar, QueryOptions? options = null);
    Task<IEntry[]> GetEntries(QueryOptions? options = null);
    Task<IEntry[]> SearchEntries(string query, QueryOptions? options = null);
    Task<IEntry> GetEntry(Guid id);

    Task<IEntry> CreateEntry(Entry entry);
    Task<IEntry> UpdateEntry(Guid id, JsonOperation[] update);
    Task DeleteEntry(Guid id);

    Task<ISense> CreateSense(Guid entryId, Sense sense);
    Task<ISense> UpdateSense(Guid entryId, Guid senseId, JsonOperation[] update);
    Task DeleteSense(Guid entryId, Guid senseId);

    Task<IExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, ExampleSentence exampleSentence);

    Task<IExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        JsonOperation[] update);

    Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId);
}

[Receiver]
public interface ILexboxClient
{
    Task OnEntryUpdated(IEntry entry);
}

public class LexboxApiHub(ILexboxApi lexboxApi, JsonSerializerOptions options) : Hub<ILexboxClient>, ILexboxApiHub
{
    public async Task<WritingSystems> GetWritingSystems()
    {
        return await lexboxApi.GetWritingSystems();
    }

    public async Task<string[]> GetExemplars()
    {
        return await lexboxApi.GetExemplars();
    }

    public async Task<IEntry[]> GetEntriesForExemplar(string exemplar, QueryOptions? options = null)
    {
        return await lexboxApi.GetEntries(exemplar, options);
    }

    public async Task<IEntry[]> GetEntries(QueryOptions? options = null)
    {
        return await lexboxApi.GetEntries(options);
    }

    public async Task<IEntry[]> SearchEntries(string query, QueryOptions? options = null)
    {
        return await lexboxApi.SearchEntries(query, options);
    }

    public async Task<IEntry> GetEntry(Guid id)
    {
        return await lexboxApi.GetEntry(id);
    }

    public async Task<IEntry> CreateEntry(Entry entry)
    {
        return await lexboxApi.CreateEntry(entry);
    }

    public async Task<IEntry> UpdateEntry(Guid id, JsonOperation[] update)
    {
        return await lexboxApi.UpdateEntry(id, FromOperations<IEntry>(update));
    }

    public async Task DeleteEntry(Guid id)
    {
        await lexboxApi.DeleteEntry(id);
    }

    public async Task<ISense> CreateSense(Guid entryId, Sense sense)
    {
        return await lexboxApi.CreateSense(entryId, sense);
    }

    public async Task<ISense> UpdateSense(Guid entryId, Guid senseId, JsonOperation[] update)
    {
        return await lexboxApi.UpdateSense(entryId, senseId, FromOperations<ISense>(update));
    }

    public async Task DeleteSense(Guid entryId, Guid senseId)
    {
        await lexboxApi.DeleteSense(entryId, senseId);
    }

    public async Task<IExampleSentence> CreateExampleSentence(Guid entryId,
        Guid senseId,
        ExampleSentence exampleSentence)
    {
        return await lexboxApi.CreateExampleSentence(entryId, senseId, exampleSentence);
    }

    public async Task<IExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        JsonOperation[] update)
    {
        return await lexboxApi.UpdateExampleSentence(entryId, senseId, exampleSentenceId, FromOperations<IExampleSentence>(update));
    }

    public async Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId)
    {
        await lexboxApi.DeleteExampleSentence(entryId, senseId, exampleSentenceId);
    }

    private JsonPatchUpdateInput<T> FromOperations<T>(JsonOperation[] operations) where T : class
    {
        return new JsonPatchUpdateInput<T>(
            new JsonPatchDocument<T>(operations.Select(o => new Operation<T>(o.Op, o.Path, o.From, o.Value)).ToList(),
                options)
        );
    }
}