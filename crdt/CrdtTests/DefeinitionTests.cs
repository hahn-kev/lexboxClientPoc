using CrdtSample.Changes;
using CrdtSample.Models;
using CrdtLib.Changes;

namespace Tests;

public class DefinitionTests : DataModelTestBase
{
    public IChange NewDefinition(Guid wordId, string text, string partOfSpeech)
    {
        return new NewDefinitionChange(Guid.NewGuid())
        {
            WordId = wordId,
            Text = text,
            PartOfSpeech = partOfSpeech
        };
    }
    
    [Fact]
    public async Task CanAddADefinitionToAWord()
    {
        var wordId = Guid.NewGuid();
        await WriteNextChange(NewWord(wordId, "hello"));
        await WriteNextChange(NewDefinition(wordId, "a greeting", "verb"));
        var snapshot = await DataModel.GetProjectSnapshot();
        var definitionSnapshot = snapshot.Snapshots.Values.Single(s => s.IsType<Definition>());
        var definition = (Definition)await DataModel.GetBySnapshotId(definitionSnapshot.Id);
        definition.Text.Should().Be("a greeting");
        definition.WordId.Should().Be(wordId);
    }

    [Fact]
    public async Task DeletingAWordDeletesTheDefinition()
    {
        var wordId = Guid.NewGuid();
        await WriteNextChange(NewWord(wordId, "hello"));
        await WriteNextChange(NewDefinition(wordId, "a greeting", "verb"));
        await WriteNextChange(new DeleteChange<Word>(wordId));
        var snapshot = await DataModel.GetProjectSnapshot();
        snapshot.Snapshots.Values.Where(s => !s.EntityIsDeleted).Should().BeEmpty();
    }

    [Fact]
    public async Task AddingADefinitionToADeletedWordDeletesIt()
    {
        var wordId = Guid.NewGuid();
        await WriteNextChange(NewWord(wordId, "hello"));
        await WriteNextChange(new DeleteChange<Word>(wordId));
        await WriteNextChange(NewDefinition(wordId, "a greeting", "verb"));
        var snapshot = await DataModel.GetProjectSnapshot();
        snapshot.Snapshots.Values.Where(s => !s.EntityIsDeleted).Should().BeEmpty();
    }
}