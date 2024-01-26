using System.Linq.Expressions;
using System.Text.Json;
using CrdtLib;
using CrdtLib.Changes;
using CrdtLib.Db;
using LcmCrdtModel.Changes;
using lexboxClientContracts;
using Microsoft.EntityFrameworkCore;
using SystemTextJsonPatch;

namespace LcmCrdtModel;

public class CrdtLexboxApi(DataModel dataModel, JsonSerializerOptions jsonOptions) : ILexboxApi
{
    //todo persist somewhere
    Guid ClientId = Guid.NewGuid();

    public Task<WritingSystem[]> GetWritingSystems()
    {
        return Task.FromResult(Array.Empty<WritingSystem>());
    }

    public Task<string[]> GetExemplars()
    {
        return null;
    }

    public Task<IEntry[]> GetEntries(string exemplar, QueryOptions? options = null)
    {
        return null;
    }

    public async Task<IEntry[]> GetEntries(QueryOptions? options = null)
    {
        return await dataModel.GetLatestObjects<Entry>().OfType<IEntry>().ToArrayAsync();
    }

    public Task<IEntry[]> SearchEntries(string query, QueryOptions? options = null)
    {
        return null;
    }

    public async Task<IEntry> GetEntry(Guid id)
    {
        var entry = await dataModel.GetLatest<Entry>(id);
        var senses = await dataModel.GetLatestObjects<Sense>(snapshot => snapshot.References.Contains(id)).ToArrayAsync();
        var exampleSentences = await dataModel.GetLatestObjects<ExampleSentence>(snapshot => snapshot.References.Intersect(senses.Select(s => s.Id)).Any())
            .ToArrayAsync();
        entry.Senses = senses;
        foreach (var sense in senses)
        {
            sense.ExampleSentences = exampleSentences.Where(s => s.SenseId == sense.Id).ToArray();
        }
        return entry;
    }

    public async Task<IEntry> CreateEntry(IEntry entry)
    {
        var changeEntity = new ChangeEntity(new CreateEntryChange(entry));
        await dataModel.Add(new Commit
        {
            ClientId = ClientId,
            ChangeEntities =
            [
                changeEntity,
                ..entry.Senses.Select(s => new ChangeEntity(new CreateSenseChange(s, entry.Id))),
                ..entry.Senses.SelectMany(s => s.ExampleSentences,
                    (sense, sentence) => new ChangeEntity(new CreateExampleSentenceChange(sentence, sense.Id)))
            ]
        });
        return await GetEntry(entry.Id);
    }

    public async Task<IEntry> UpdateEntry(Guid id, UpdateObjectInput<IEntry> update)
    {
        var jsonPatch = ((CrdtUpdateBuilder<IEntry>)update).PatchChange;
        var patchChange = new JsonPatchChange<Entry>(id, jsonPatch, jsonOptions);
        await dataModel.Add(new Commit
        {
            ClientId = ClientId,
            ChangeEntities = { new ChangeEntity(patchChange) }
        });
        return await GetEntry(id);
    }

    public async Task DeleteEntry(Guid id)
    {
        await dataModel.Add(new Commit
        {
            ClientId = ClientId,
            ChangeEntities = { new ChangeEntity(new DeleteChange<Entry>(id)) }
        });
    }

    public async Task<ISense> CreateSense(Guid entryId, ISense sense)
    {
        await dataModel.Add(new Commit
        {
            ClientId = ClientId,
            ChangeEntities =
            [
                new ChangeEntity(new CreateSenseChange(sense, entryId)),
                ..sense.ExampleSentences.Select(sentence =>
                    new ChangeEntity(new CreateExampleSentenceChange(sentence, sense.Id)))
            ]
        });
        return await dataModel.GetLatest<Sense>(sense.Id);
    }

    public async Task<ISense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<ISense> update)
    {
        var jsonPatch = ((CrdtUpdateBuilder<ISense>)update).PatchChange;
        var patchChange = new JsonPatchChange<Sense>(senseId, jsonPatch, jsonOptions);
        await dataModel.Add(new Commit(Guid.NewGuid())
        {
            ClientId = ClientId,
            ChangeEntities = { new ChangeEntity(patchChange) }
        });
        return await dataModel.GetLatest<Sense>(senseId);
    }

    public async Task DeleteSense(Guid entryId, Guid senseId)
    {
        await dataModel.Add(new Commit
        {
            ClientId = ClientId,
            ChangeEntities = { new ChangeEntity(new DeleteChange<Sense>(senseId)) }
        });
    }

    public async Task<IExampleSentence> CreateExampleSentence(Guid entryId,
        Guid senseId,
        IExampleSentence exampleSentence)
    {
        await dataModel.Add(new Commit
        {
            ClientId = ClientId,
            ChangeEntities =
            {
                new ChangeEntity(new CreateExampleSentenceChange(exampleSentence, senseId))
            }
        });
        return await dataModel.GetLatest<ExampleSentence>(exampleSentence.Id);
    }

    public async Task<IExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        UpdateObjectInput<IExampleSentence> update)
    {
        var jsonPatch = ((CrdtUpdateBuilder<IExampleSentence>)update).PatchChange;
        var patchChange = new JsonPatchChange<ExampleSentence>(exampleSentenceId, jsonPatch, jsonOptions);
        await dataModel.Add(new Commit(Guid.NewGuid())
        {
            ClientId = ClientId,
            ChangeEntities = { new ChangeEntity(patchChange) }
        });
        return await dataModel.GetLatest<ExampleSentence>(exampleSentenceId);
    }

    public Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId)
    {
        return dataModel.Add(new Commit
        {
            ClientId = ClientId,
            ChangeEntities = { new ChangeEntity(new DeleteChange<ExampleSentence>(exampleSentenceId)) }
        });
    }

    public UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class
    {
        return new CrdtUpdateBuilder<T>();
    }

    private class CrdtUpdateBuilder<T> : UpdateBuilder<T>, UpdateObjectInput<T> where T : class
    {
        public JsonPatchDocument<T> PatchChange { get; } = new();

        public UpdateBuilder<T> Set<T_Val>(Expression<Func<T, T_Val>> field, T_Val value)
        {
            PatchChange.Replace(field, value);
            return this;
        }

        public UpdateObjectInput<T> Build()
        {
            return this;
        }

        public void Apply(T obj)
        {
            PatchChange.ApplyTo(obj);
        }
    }
}