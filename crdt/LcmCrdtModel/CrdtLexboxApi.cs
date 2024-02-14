using System.Linq.Expressions;
using System.Text.Json;
using CrdtLib;
using CrdtLib.Changes;
using CrdtLib.Db;
using LcmCrdtModel.Changes;
using lexboxClientContracts;
using LinqToDB;
using LinqToDB.EntityFrameworkCore;

namespace LcmCrdtModel;

public class CrdtLexboxApi(DataModel dataModel, JsonSerializerOptions jsonOptions) : ILexboxApi
{
    //todo persist somewhere
    Guid ClientId = Guid.NewGuid();

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
        var queryable = dataModel.GetLatestObjects<Entry>().ToLinqToDB();
        if (predicate is not null) queryable = queryable.Where(predicate);
        var entries = await queryable
            .OfType<lexboxClientContracts.Entry>()
            .ToArrayAsync();
        var allSenses = await dataModel.GetLatestObjects<Sense>()
            .Where(s => entries.Select(e => e.Id).Contains(s.EntryId))
            .GroupBy(s => s.EntryId)
            .ToDictionaryAsync(g => g.Key);
        var allSenseIds = allSenses.Values.SelectMany(s => s, (_, sense) => sense.Id);
        var allExampleSentences = await dataModel
            .GetLatestObjects<ExampleSentence>()
            .Where(e => allSenseIds.Contains(e.SenseId))
            .GroupBy(s => s.SenseId)
            .ToDictionaryAsync(g => g.Key);
        foreach (var entry in entries)
        {
            entry.Senses = allSenses.TryGetValue(entry.Id, out var senses) ? senses.ToArray() : [];
            foreach (var sense in entry.Senses)
            {
                sense.ExampleSentences = allExampleSentences.TryGetValue(sense.Id, out var sentences) ? sentences.ToArray() : [];
            }
        }

        return entries;
    }

    public async Task<lexboxClientContracts.Entry> GetEntry(Guid id)
    {
        var entry = await dataModel.GetLatest<Entry>(id);
        var senses = await dataModel.GetLatestObjects<Sense>()
            .Where(s => s.EntryId == id)
            .ToArrayAsync();
        var exampleSentences = await dataModel
            .GetLatestObjects<ExampleSentence>()
            .Where(e => senses.Select(s => s.Id).Contains(e.SenseId))
            .GroupBy(s => s.SenseId)
            .ToDictionaryAsync(g => g.Key);
        entry.Senses = senses;
        foreach (var sense in senses)
        {
            sense.ExampleSentences = exampleSentences.TryGetValue(sense.Id, out var sentences) ? sentences.ToArray() : [];
        }

        return entry;
    }

    public async Task<lexboxClientContracts.Entry> CreateEntry(lexboxClientContracts.Entry entry)
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

    public async Task<lexboxClientContracts.Entry> UpdateEntry(Guid id,
        UpdateObjectInput<lexboxClientContracts.Entry> update)
    {
        var patchChange = new JsonPatchChange<Entry>(id, update.Patch, jsonOptions);
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

    public async Task<lexboxClientContracts.Sense> CreateSense(Guid entryId, lexboxClientContracts.Sense sense)
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

    public async Task<lexboxClientContracts.Sense> UpdateSense(Guid entryId,
        Guid senseId,
        UpdateObjectInput<lexboxClientContracts.Sense> update)
    {
        var patchChange = new JsonPatchChange<Sense>(senseId, update.Patch, jsonOptions);
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

    public async Task<lexboxClientContracts.ExampleSentence> CreateExampleSentence(Guid entryId,
        Guid senseId,
        lexboxClientContracts.ExampleSentence exampleSentence)
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

    public async Task<lexboxClientContracts.ExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        UpdateObjectInput<lexboxClientContracts.ExampleSentence> update)
    {
        var jsonPatch = update.Patch;
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
        return new UpdateBuilder<T>();
    }
}