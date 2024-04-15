using System.Linq.Expressions;
using System.Text.Json;
using CrdtLib;
using CrdtLib.Changes;
using CrdtLib.Db;
using CrdtLib.Helpers;
using LcmCrdtModel.Changes;
using lexboxClientContracts;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;

namespace LcmCrdtModel;

public class CrdtLexboxApi(DataModel dataModel, JsonSerializerOptions jsonOptions, IHybridDateTimeProvider timeProvider) : ILexboxApi
{
    //todo persist somewhere
    Guid ClientId = Guid.NewGuid();

    private IQueryable<Entry> Entries => dataModel.GetLatestObjects<Entry>().ToLinqToDB();
    private IQueryable<Sense> Senses => dataModel.GetLatestObjects<Sense>().ToLinqToDB();
    private IQueryable<ExampleSentence> ExampleSentences => dataModel.GetLatestObjects<ExampleSentence>().ToLinqToDB();

    public Task<WritingSystems> GetWritingSystems()
    {
        return Task.FromResult(new WritingSystems()
        {
            Analysis =
            [
                new WritingSystem { Id = "en", Name = "English", Abbreviation = "en", Font = "Arial" },
            ],
            Vernacular =
            [
                new WritingSystem { Id = "en", Name = "English", Abbreviation = "en", Font = "Arial" },
            ]
        });
    }

    public Task<string[]> GetExemplars()
    {
        throw new NotImplementedException();
    }

    public Task<lexboxClientContracts.Entry[]> GetEntries(string exemplar, QueryOptions? options = null)
    {
        throw new NotImplementedException();
    }

    public async Task<lexboxClientContracts.Entry[]> GetEntries(QueryOptions? options = null)
    {
        return await GetEntries(predicate: null, options);
    }

    public async Task<lexboxClientContracts.Entry[]> SearchEntries(string query, QueryOptions? options = null)
    {
        return await GetEntries(e => e.LexemeForm.SearchValue(query), options);
    }

    private async Task<lexboxClientContracts.Entry[]> GetEntries(
        Expression<Func<Entry, bool>>? predicate = null,
        QueryOptions? options = null)
    {
        var queryable = Entries;
        if (predicate is not null) queryable = queryable.Where(predicate);
        var entries = await queryable.ToArrayAsyncLinqToDB();
        var allSenses = (await Senses
            .Where(s => entries.Select(e => e.Id).Contains(s.EntryId))
            .ToArrayAsync())
            .ToLookup(s => s.EntryId)
            .ToDictionary(g => g.Key, g => g.ToArray());
        var allSenseIds = allSenses.Values.SelectMany(s => s, (_, sense) => sense.Id);
        var allExampleSentences = (await ExampleSentences
            .Where(e => allSenseIds.Contains(e.SenseId))
            .ToArrayAsync())
            .ToLookup(s => s.SenseId)
            .ToDictionary(g => g.Key, g => g.ToArray());
        foreach (var entry in entries)
        {
            entry.Senses = allSenses.TryGetValue(entry.Id, out var senses) ? senses.ToArray() : [];
            foreach (var sense in entry.Senses)
            {
                sense.ExampleSentences = allExampleSentences.TryGetValue(sense.Id, out var sentences)
                    ? sentences.ToArray()
                    : [];
            }
        }

        return entries;
    }

    public async Task<lexboxClientContracts.Entry?> GetEntry(Guid id)
    {
        var entry = await dataModel.GetLatest<Entry>(id);
        if (entry is null) return null;
        var senses = await Senses
                .Where(s => s.EntryId == id).ToArrayAsyncLinqToDB();
        var exampleSentences = (await ExampleSentences
                .Where(e => senses.Select(s => s.Id).Contains(e.SenseId)).ToArrayAsyncLinqToDB())
            .ToLookup(e => e.SenseId)
            .ToDictionary(g => g.Key, g => g.ToArray());
        entry.Senses = senses;
        foreach (var sense in senses)
        {
            sense.ExampleSentences = exampleSentences.TryGetValue(sense.Id, out var sentences) ? sentences.ToArray() : [];
        }

        return entry;
    }

    public async Task<lexboxClientContracts.Entry> CreateEntry(lexboxClientContracts.Entry entry)
    {
        await dataModel.AddChanges(ClientId,
        [
            new CreateEntryChange(entry),
            ..entry.Senses.Select(s => new CreateSenseChange(s, entry.Id)),
            ..entry.Senses.SelectMany(s => s.ExampleSentences,
                (sense, sentence) => new CreateExampleSentenceChange(sentence, sense.Id))
        ]);
        return await GetEntry(entry.Id);
    }

    public async Task<lexboxClientContracts.Entry> UpdateEntry(Guid id,
        UpdateObjectInput<lexboxClientContracts.Entry> update)
    {
        var patchChange = new JsonPatchChange<Entry>(id, update.Patch, jsonOptions);
        await dataModel.AddChange(ClientId, patchChange);
        return await GetEntry(id);
    }

    public async Task DeleteEntry(Guid id)
    {
        await dataModel.AddChange(ClientId, new DeleteChange<Entry>(id));
    }

    public async Task<lexboxClientContracts.Sense> CreateSense(Guid entryId, lexboxClientContracts.Sense sense)
    {
        await dataModel.AddChanges(ClientId,
        [
            new CreateSenseChange(sense, entryId),
            ..sense.ExampleSentences.Select(sentence =>
                new CreateExampleSentenceChange(sentence, sense.Id))
        ]);
        return await dataModel.GetLatest<Sense>(sense.Id);
    }

    public async Task<lexboxClientContracts.Sense> UpdateSense(Guid entryId,
        Guid senseId,
        UpdateObjectInput<lexboxClientContracts.Sense> update)
    {
        var patchChange = new JsonPatchChange<Sense>(senseId, update.Patch, jsonOptions);
        await dataModel.AddChange(ClientId, patchChange);
        return await dataModel.GetLatest<Sense>(senseId);
    }

    public async Task DeleteSense(Guid entryId, Guid senseId)
    {
        await dataModel.AddChange(ClientId, new DeleteChange<Sense>(senseId));
    }

    public async Task<lexboxClientContracts.ExampleSentence> CreateExampleSentence(Guid entryId,
        Guid senseId,
        lexboxClientContracts.ExampleSentence exampleSentence)
    {
        await dataModel.AddChange(ClientId, new CreateExampleSentenceChange(exampleSentence, senseId));
        return await dataModel.GetLatest<ExampleSentence>(exampleSentence.Id);
    }

    public async Task<lexboxClientContracts.ExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        UpdateObjectInput<lexboxClientContracts.ExampleSentence> update)
    {
        var jsonPatch = update.Patch;
        var patchChange = new JsonPatchChange<ExampleSentence>(exampleSentenceId, jsonPatch, jsonOptions);
        await dataModel.AddChange(ClientId, patchChange);
        return await dataModel.GetLatest<ExampleSentence>(exampleSentenceId);
    }

    public Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId)
    {
        return dataModel.AddChange(ClientId, new DeleteChange<ExampleSentence>(exampleSentenceId));
    }

    public UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class
    {
        return new UpdateBuilder<T>();
    }
}