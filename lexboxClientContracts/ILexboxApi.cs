using System.Text.Json;
using SystemTextJsonPatch;

namespace lexboxClientContracts;

public interface ILexboxApi
{
    Task<string[]> GetExemplars();
    Task<Entry[]> GetEntries(string exemplar, QueryOptions? options = null);
    Task<Entry[]> GetEntries(QueryOptions? options = null);
    Task<Entry[]> SearchEntries(string query, QueryOptions? options = null);
    Task<Entry> GetEntry(Guid id);

    Task<Entry> CreateEntry(Entry entry);
    Task<Entry> UpdateEntry(Guid id, UpdateObjectInput<Entry> entry);
    Task DeleteEntry(Guid id);

    Task<Sense> CreateSense(Guid entryId, Sense sense);
    Task<Sense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<Sense> sense);
    Task DeleteSense(Guid entryId, Guid senseId);

    Task<ExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, ExampleSentence exampleSentence);
    Task<ExampleSentence> UpdateExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId, UpdateObjectInput<ExampleSentence> exampleSentence);
    Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId);
}

public record QueryOptions(string Order, int Count = 1000, int Offset = 0);

//todo what should this be?
public class UpdateObjectInput<T> where T : class
{
    //could be a list of kvps of field changes, what does this mean for a multi string?
    public Dictionary<string, object> Updates { get; set; }
    
    //could be json patch
    public JsonPatchDocument<T> Patch { get; set; }
    
    //could be a command object
    public enum Commands
    {
        updateLexemeForm,
        updateCitationForm,
        //... etc
    }
    
    public Commands Command { get; set; }
    public JsonElement CommandData { get; set; }
}