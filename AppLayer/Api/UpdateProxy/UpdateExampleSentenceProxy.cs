﻿using lexboxClientContracts;
using SIL.LCModel;
using SIL.LCModel.Core.Text;

namespace AppLayer.Api.UpdateProxy;

public class UpdateExampleSentenceProxy(ILexExampleSentence sentence, LexboxLcmApi lexboxLcmApi): ExampleSentence
{
    public override Guid Id
    {
        get => sentence.Guid;
        set => throw new NotImplementedException();
    }

    public override MultiString Sentence
    {
        get => new UpdateMultiStringProxy(sentence.Example, lexboxLcmApi);
        set => throw new NotImplementedException();
    }

    public override MultiString Translation
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public override string Reference
    {
        get => throw new NotImplementedException();
        set => sentence.Reference = TsStringUtils.MakeString(value, sentence.Reference.get_WritingSystem(0));
    }
}