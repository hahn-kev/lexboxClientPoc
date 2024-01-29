using lexboxClientContracts;
using SIL.LCModel;
using SIL.LCModel.Core.KernelInterfaces;
using IMultiString = lexboxClientContracts.IMultiString;

namespace AppLayer.Api.UpdateProxy;

public class UpdateEntryProxy(ILexEntry lcmEntry, LexboxLcmApi lexboxLcmApi) : IEntry
{
    public Guid Id
    {
        get => lcmEntry.Guid;
        set => throw new NotImplementedException();
    }

    public IMultiString LexemeForm
    {
        get => new UpdateMultiStringProxy(lcmEntry.LexemeFormOA.Form, lexboxLcmApi);
        set => throw new NotImplementedException();
    }

    public IMultiString CitationForm
    {
        get => new UpdateMultiStringProxy(lcmEntry.CitationForm, lexboxLcmApi);
        set => throw new NotImplementedException();
    }

    public IMultiString LiteralMeaning
    {
        get => new UpdateMultiStringProxy(lcmEntry.LiteralMeaning, lexboxLcmApi);
        set => throw new NotImplementedException();
    }

    public IList<ISense> Senses
    {
        get =>
            new UpdateListProxy<ISense>(
                sense => lexboxLcmApi.CreateSense(lcmEntry, sense),
                sense => lexboxLcmApi.DeleteSense(Id, sense.Id),
                i => new UpdateSenseProxy(lcmEntry.SensesOS[i], lexboxLcmApi)
            );
        set => throw new NotImplementedException();
    }

    public IMultiString Note
    {
        get => new UpdateMultiStringProxy(lcmEntry.Comment, lexboxLcmApi);
        set => throw new NotImplementedException();
    }
}

public class UpdateMultiStringProxy(ITsMultiString multiString, LexboxLcmApi lexboxLcmApi) : IMultiString
{
    public IDictionary<WritingSystemId, string> Values
    {
        get => new UpdateDictionaryProxy(multiString, lexboxLcmApi);
        set => throw new NotImplementedException();
    }

    public IMultiString Copy()
    {
        return new UpdateMultiStringProxy(multiString, lexboxLcmApi);
    }
}