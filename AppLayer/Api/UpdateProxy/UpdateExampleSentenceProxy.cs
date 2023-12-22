using lexboxClientContracts;
using SIL.LCModel;
using SIL.LCModel.Core.Text;
using IMultiString = lexboxClientContracts.IMultiString;

namespace AppLayer.Api.UpdateProxy;

public class UpdateExampleSentenceProxy(ILexExampleSentence sentence, LexboxLcmApi lexboxLcmApi): IExampleSentence
{
    public Guid Id
    {
        get => sentence.Guid;
        set => throw new NotImplementedException();
    }

    public IMultiString Sentence
    {
        get => new UpdateMultiStringProxy(sentence.Example, lexboxLcmApi);
        set => throw new NotImplementedException();
    }

    public IMultiString Translation
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public string Reference
    {
        get => throw new NotImplementedException();
        set => sentence.Reference = TsStringUtils.MakeString(value, sentence.Reference.get_WritingSystem(0));
    }
}