using lexboxClientContracts;
using SIL.LCModel;
using IMultiString = lexboxClientContracts.IMultiString;

namespace AppLayer.Api.UpdateProxy;

public class UpdateSenseProxy(ILexSense sense, LexboxLcmApi lexboxLcmApi) : ISense
{
    public Guid Id
    {
        get => sense.Guid;
        set => throw new NotImplementedException();
    }

    public IMultiString Definition
    {
        get => new UpdateMultiStringProxy(sense.Definition, lexboxLcmApi);
        set => throw new NotImplementedException();
    }

    public IMultiString Gloss
    {
        get => new UpdateMultiStringProxy(sense.Gloss, lexboxLcmApi);
        set => throw new NotImplementedException();
    }

    public string PartOfSpeech
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IList<string> SemanticDomain
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public IList<IExampleSentence> ExampleSentences
    {
        get =>
            new UpdateListProxy<IExampleSentence>(
                sentence => lexboxLcmApi.CreateExampleSentence(sense, sentence),
                sentence => lexboxLcmApi.DeleteExampleSentence(sense.Owner.Guid, Id, sentence.Id),
                i => new UpdateExampleSentenceProxy(sense.ExamplesOS[i], lexboxLcmApi)
            );
        set => throw new NotImplementedException();
    }
}