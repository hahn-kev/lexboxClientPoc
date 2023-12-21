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

    private readonly ILexEntryFactory _lexEntryFactory = cache.ServiceLocator.GetInstance<ILexEntryFactory>();

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
        return null;
    }

    public Task<IEntry[]> GetEntries(string exemplar, QueryOptions? options = null)
    {
        return null;
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
                var lexEntry = _lexEntryFactory.Create(new LexEntryComponents
                {
                    MorphType = rootMorphType,
                    LexemeFormAlternatives = MultiStringToTsStrings(entry.LexemeForm),
                    GlossAlternatives = MultiStringToTsStrings(entry.Senses.FirstOrDefault()?.Gloss),
                    GlossFeatures = [],
                    MSA = null
                });
                UpdateLcmMultiString(lexEntry.CitationForm, entry.CitationForm);
                UpdateLcmMultiString(lexEntry.LiteralMeaning, entry.LiteralMeaning);
                UpdateLcmMultiString(lexEntry.Comment, entry.Note);
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

    public Task<IEntry> UpdateEntry(Guid id, UpdateObjectInput<IEntry> entry)
    {
        
        var lcmEntry = _entriesRepository.GetObject(id);
        UndoableUnitOfWorkHelper.Do("Update Entry",
            "Revert entry",
            cache.ServiceLocator.ActionHandler,
            () =>
            {
                var updateProxy = new UpdateEntryProxy(lcmEntry, this);
                entry.Apply(updateProxy);
            });
        return Task.FromResult(FromLexEntry(lcmEntry));
    }

    public Task DeleteEntry(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<ISense> CreateSense(Guid entryId, ISense sense)
    {
        throw new NotImplementedException();
    }

    public Task<ISense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<ISense> sense)
    {
        throw new NotImplementedException();
    }

    public Task DeleteSense(Guid entryId, Guid senseId)
    {
        throw new NotImplementedException();
    }

    public Task<IExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, IExampleSentence exampleSentence)
    {
        throw new NotImplementedException();
    }

    public Task<IExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        UpdateObjectInput<IExampleSentence> exampleSentence)
    {
        throw new NotImplementedException();
    }

    public Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId)
    {
        throw new NotImplementedException();
    }

    public UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class
    {
        return new JsonPatchUpdateBuilder<T>();
    }
}