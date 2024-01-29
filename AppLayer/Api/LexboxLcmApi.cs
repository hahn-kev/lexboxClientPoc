using AppLayer.Api.UpdateProxy;
using lexboxClientContracts;
using SIL.LCModel;
using SIL.LCModel.Core.KernelInterfaces;
using SIL.LCModel.Core.Text;
using SIL.LCModel.DomainServices;
using SIL.LCModel.Infrastructure;
using IMultiString = lexboxClientContracts.IMultiString;

namespace AppLayer.Api;

public class LexboxLcmApiFactory
{
    public static async Task<ILexboxApi> CreateApi(string projectFilePath, bool onCloseSave)
    {
        using var projectFile = File.OpenRead(projectFilePath);
        var cache = await ProjectLoader.LoadCache(projectFile, projectFilePath);
        return new LexboxLcmApi(cache, onCloseSave);
    }
}

public class LexboxLcmApi(LcmCache cache, bool onCloseSave) : ILexboxApi, IDisposable
{
    private readonly IRepository<ILexEntry> _entriesRepository =
        cache.ServiceLocator.GetInstance<IRepository<ILexEntry>>();

    private readonly IRepository<ILexSense> _senseRepository =
        cache.ServiceLocator.GetInstance<IRepository<ILexSense>>();

    private readonly IRepository<ILexExampleSentence> _exampleSentenceRepository =
        cache.ServiceLocator.GetInstance<IRepository<ILexExampleSentence>>();

    private readonly ILexEntryFactory _lexEntryFactory = cache.ServiceLocator.GetInstance<ILexEntryFactory>();
    private readonly ILexSenseFactory _lexSenseFactory = cache.ServiceLocator.GetInstance<ILexSenseFactory>();

    private readonly ILexExampleSentenceFactory _lexExampleSentenceFactory =
        cache.ServiceLocator.GetInstance<ILexExampleSentenceFactory>();

    private readonly IMoMorphTypeRepository _morphTypeRepository =
        cache.ServiceLocator.GetInstance<IMoMorphTypeRepository>();

    public void Dispose()
    {
        if (onCloseSave)
            cache.ActionHandlerAccessor.Commit();
        cache.Dispose();
    }

    internal WritingSystemId GetWritingSystemId(int ws)
    {
        return cache.ServiceLocator.WritingSystemManager.Get(ws).Id;
    }

    internal int GetWritingSystemHandle(WritingSystemId ws)
    {
        return cache.ServiceLocator.WritingSystemManager.Get(ws.Code).Handle;
    }

    public Task<WritingSystem[]> GetWritingSystems()
    {
        return Task.FromResult(cache.ServiceLocator.WritingSystems.AllWritingSystems.Select(ws => new WritingSystem
        {
            Id = ws.Id,
            Name = ws.LanguageTag,
            Abbreviation = ws.Abbreviation,
            Font = ws.DefaultFontName
        }).ToArray());
    }

    public Task<string[]> GetExemplars()
    {
        throw new NotImplementedException();
    }

    public Task<IEntry[]> GetEntries(string exemplar, QueryOptions? options = null)
    {
        throw new NotImplementedException();
    }

    private IEntry FromLexEntry(ILexEntry entry)
    {
        return new lexboxClientContracts.Entry
        {
            Id = entry.Guid,
            Note = FromLcmMultiString(entry.Comment),
            LexemeForm = FromLcmMultiString(entry.LexemeFormOA.Form),
            CitationForm = FromLcmMultiString(entry.CitationForm),
            LiteralMeaning = FromLcmMultiString(entry.LiteralMeaning),
            Senses = entry.AllSenses.Select(FromLexSense).ToList()
        };
    }

    private ISense FromLexSense(ILexSense sense)
    {
        return new Sense
        {
            Id = sense.Guid,
            Gloss = FromLcmMultiString(sense.Gloss),
            Definition = FromLcmMultiString(sense.Definition),
            PartOfSpeech = sense.SenseTypeRA?.Name.BestAnalysisVernacularAlternative.Text ?? string.Empty,
            SemanticDomain = sense.SemanticDomainsRC.Select(s => s.OcmCodes).ToList(),
            ExampleSentences = sense.ExamplesOS.Select(FromLexExampleSentence).ToList()
        };
    }

    private IExampleSentence FromLexExampleSentence(ILexExampleSentence sentence)
    {
        return new ExampleSentence
        {
            Id = sentence.Guid,
            Sentence = FromLcmMultiString(sentence.Example),
            Reference = sentence.Reference.Text
        };
    }

    private MultiString FromLcmMultiString(ITsMultiString multiString)
    {
        var result = new MultiString();
        for (int i = 0; i < multiString.StringCount; i++)
        {
            var tsString = multiString.GetStringFromIndex(i, out var ws);
            result.Values.Add(GetWritingSystemId(ws), tsString.Text);
        }

        return result;
    }

    public Task<IEntry[]> GetEntries(QueryOptions? options = null)
    {
        return Task.FromResult(_entriesRepository.AllInstances().Select(FromLexEntry).ToArray());
    }

    public async Task<IEntry[]> SearchEntries(string query, QueryOptions? options = null)
    {
        var entries = await GetEntries(options);
        return entries.Where(e => e.LexemeForm.Values.Values.Any(v => v.Contains(query))).ToArray();
    }

    public Task<IEntry> GetEntry(Guid id)
    {
        return Task.FromResult(FromLexEntry(_entriesRepository.GetObject(id)));
    }

    public async Task<IEntry> CreateEntry(IEntry entry)
    {
        if (entry.Id != default) throw new NotSupportedException("Id must be empty");
        Guid entryId = default;
        UndoableUnitOfWorkHelper.Do("Create Entry",
            "Remove entry",
            cache.ServiceLocator.ActionHandler,
            () =>
            {
                var rootMorphType = _morphTypeRepository.GetObject(MoMorphTypeTags.kguidMorphRoot);
                var firstSense = entry.Senses.FirstOrDefault();
                var lexEntry = _lexEntryFactory.Create(new LexEntryComponents
                {
                    MorphType = rootMorphType,
                    LexemeFormAlternatives = MultiStringToTsStrings(entry.LexemeForm),
                    GlossAlternatives = MultiStringToTsStrings(firstSense?.Gloss),
                    GlossFeatures = [],
                    MSA = null
                });
                UpdateLcmMultiString(lexEntry.CitationForm, entry.CitationForm);
                UpdateLcmMultiString(lexEntry.LiteralMeaning, entry.LiteralMeaning);
                UpdateLcmMultiString(lexEntry.Comment, entry.Note);
                if (firstSense is not null)
                {
                    var lexSense = lexEntry.SensesOS.First();
                    ApplySenseToLexSense(firstSense, lexSense);
                }

                //first sense is already created
                foreach (var sense in entry.Senses.Skip(1))
                {
                    CreateSense(lexEntry, sense);
                }

                entryId = lexEntry.Guid;
            });
        if (entryId == default) throw new InvalidOperationException("Entry was not created");

        return await GetEntry(entryId);
    }

    private IList<ITsString> MultiStringToTsStrings(IMultiString? multiString)
    {
        if (multiString is null) return [];
        var result = new List<ITsString>(multiString.Values.Count);
        foreach (var (ws, value) in multiString.Values)
        {
            result.Add(TsStringUtils.MakeString(value, GetWritingSystemHandle(ws)));
        }

        return result;
    }

    private void UpdateLcmMultiString(ITsMultiString multiString, IMultiString newMultiString)
    {
        foreach (var (ws, value) in newMultiString.Values)
        {
            var writingSystemHandle = GetWritingSystemHandle(ws);
            multiString.set_String(writingSystemHandle, TsStringUtils.MakeString(value, writingSystemHandle));
        }
    }

    public Task<IEntry> UpdateEntry(Guid id, UpdateObjectInput<IEntry> update)
    {
        var lexEntry = _entriesRepository.GetObject(id);
        UndoableUnitOfWorkHelper.Do("Update Entry",
            "Revert entry",
            cache.ServiceLocator.ActionHandler,
            () =>
            {
                var updateProxy = new UpdateEntryProxy(lexEntry, this);
                update.Apply(updateProxy);
            });
        return Task.FromResult(FromLexEntry(lexEntry));
    }

    public Task DeleteEntry(Guid id)
    {
        UndoableUnitOfWorkHelper.Do("Delete Entry",
            "Revert delete",
            cache.ServiceLocator.ActionHandler,
            () =>
            {
                _entriesRepository.GetObject(id).Delete();
            });
        return Task.CompletedTask;
    }

    internal void CreateSense(ILexEntry lexEntry, ISense sense)
    {
        var lexSense = _lexSenseFactory.Create(sense.Id, lexEntry);
        ApplySenseToLexSense(sense, lexSense);
    }

    private void ApplySenseToLexSense(ISense sense, ILexSense lexSense)
    {
        UpdateLcmMultiString(lexSense.Gloss, sense.Gloss);
        UpdateLcmMultiString(lexSense.Definition, sense.Definition);
        foreach (var exampleSentence in sense.ExampleSentences)
        {
            CreateExampleSentence(lexSense, exampleSentence);
        }
    }

    public Task<ISense> CreateSense(Guid entryId, ISense sense)
    {
        if (sense.Id != default) sense.Id = Guid.NewGuid();
        if (!_entriesRepository.TryGetObject(entryId, out var lexEntry))
            throw new InvalidOperationException("Entry not found");
        UndoableUnitOfWorkHelper.Do("Create Sense",
            "Remove sense",
            cache.ServiceLocator.ActionHandler,
            () => CreateSense(lexEntry, sense));
        return Task.FromResult(FromLexSense(_senseRepository.GetObject(sense.Id)));
    }

    public Task<ISense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<ISense> update)
    {
        var lexSense = _senseRepository.GetObject(senseId);
        if (lexSense.Owner.Guid != entryId) throw new InvalidOperationException("Sense does not belong to entry");
        UndoableUnitOfWorkHelper.Do("Update Sense",
            "Revert sense",
            cache.ServiceLocator.ActionHandler,
            () =>
            {
                var updateProxy = new UpdateSenseProxy(lexSense, this);
                update.Apply(updateProxy);
            });
        return Task.FromResult(FromLexSense(lexSense));
    }

    public Task DeleteSense(Guid entryId, Guid senseId)
    {
        var lexSense = _senseRepository.GetObject(senseId);
        if (lexSense.Owner.Guid != entryId) throw new InvalidOperationException("Sense does not belong to entry");
        UndoableUnitOfWorkHelper.Do("Delete Sense",
            "Revert delete",
            cache.ServiceLocator.ActionHandler,
            () => lexSense.Delete());
        return Task.CompletedTask;
    }

    internal void CreateExampleSentence(ILexSense lexSense, IExampleSentence exampleSentence)
    {
        var lexExampleSentence = _lexExampleSentenceFactory.Create(exampleSentence.Id, lexSense);
        UpdateLcmMultiString(lexExampleSentence.Example, exampleSentence.Sentence);
        lexExampleSentence.Reference = TsStringUtils.MakeString(exampleSentence.Reference,
            lexExampleSentence.Reference.get_WritingSystem(0));
    }

    public Task<IExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, IExampleSentence exampleSentence)
    {
        if (exampleSentence.Id != default) exampleSentence.Id = Guid.NewGuid();
        if (!_senseRepository.TryGetObject(senseId, out var lexSense))
            throw new InvalidOperationException("Sense not found");
        UndoableUnitOfWorkHelper.Do("Create Example Sentence",
            "Remove example sentence",
            cache.ServiceLocator.ActionHandler,
            () => CreateExampleSentence(lexSense, exampleSentence));
        return Task.FromResult(FromLexExampleSentence(_exampleSentenceRepository.GetObject(exampleSentence.Id)));
    }

    public Task<IExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        UpdateObjectInput<IExampleSentence> update)
    {
        var lexExampleSentence = _exampleSentenceRepository.GetObject(exampleSentenceId);
        if (lexExampleSentence.Owner.Guid != senseId)
            throw new InvalidOperationException("Example sentence does not belong to sense");
        if (lexExampleSentence.Owner.Owner.Guid != entryId)
            throw new InvalidOperationException("Example sentence does not belong to entry");
        UndoableUnitOfWorkHelper.Do("Update Example Sentence",
            "Revert example sentence",
            cache.ServiceLocator.ActionHandler,
            () =>
            {
                var updateProxy = new UpdateExampleSentenceProxy(lexExampleSentence, this);
                update.Apply(updateProxy);
            });
        return Task.FromResult(FromLexExampleSentence(lexExampleSentence));
    }

    public Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId)
    {
        var lexExampleSentence = _exampleSentenceRepository.GetObject(exampleSentenceId);
        if (lexExampleSentence.Owner.Guid != senseId)
            throw new InvalidOperationException("Example sentence does not belong to sense");
        if (lexExampleSentence.Owner.Owner.Guid != entryId)
            throw new InvalidOperationException("Example sentence does not belong to entry");
        UndoableUnitOfWorkHelper.Do("Delete Example Sentence",
            "Revert delete",
            cache.ServiceLocator.ActionHandler,
            () => lexExampleSentence.Delete());
        return Task.CompletedTask;
    }

    public UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class
    {
        return new UpdateBuilder<T>();
    }
}