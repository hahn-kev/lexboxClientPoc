using SystemTextJsonPatch;

namespace lexboxClientContracts;

public class InMemoryApi : ILexboxApi
{
    private readonly List<IEntry> _entries =
    [
        new Entry
        {
            Id = Guid.NewGuid(),
            LexemeForm = new MultiString
            {
                Values =
                {
                    { "en", "apple" },
                }
            },
            Senses =
            [
                new Sense
                {
                    Id = Guid.NewGuid(),
                    Gloss = new MultiString
                    {
                        Values =
                        {
                            {"en", "fruit"}
                        }
                    },
                    Definition = new MultiString
                    {
                        Values =
                        {
                            { "en", "A red or green fruit that grows on a tree" },
                        }
                    },
                    ExampleSentences =
                    [
                        new ExampleSentence
                        {
                            Id = Guid.NewGuid(),
                            Sentence = new MultiString
                            {
                                Values =
                                {
                                    { "en", "The apple fell from the tree." },
                                }
                            }
                        },
                    ],
                },
            ],
        },
        new Entry
        {
            Id = Guid.NewGuid(),
            LexemeForm = new MultiString
            {
                Values =
                {
                    { "en", "banana" },
                }
            },
            Senses =
            [
                new Sense
                {
                    Id = Guid.NewGuid(),
                    Gloss = new MultiString
                    {
                        Values =
                        {
                            { "en", "fruit" }
                        }
                    },
                    Definition = new MultiString
                    {
                        Values =
                        {
                            { "en", "A yellow fruit that grows on a tree" },
                        }
                    },
                    ExampleSentences =
                    [
                        new ExampleSentence
                        {
                            Id = Guid.NewGuid(),
                            Sentence = new MultiString
                            {
                                Values =
                                {
                                    { "en", "The banana fell from the tree." },
                                }
                            }
                        },
                    ],
                },
            ],
        },
    ];

    private readonly List<WritingSystem> _writingSystems =
    [
        new WritingSystem { Id = "en", Name = "English", Abbreviation = "en", Font = "Arial" },
    ];

    private readonly string[] _exemplars = Enumerable.Range('a', 'z').Select(c => ((char)c).ToString()).ToArray();

    public Task<IEntry> CreateEntry(IEntry entry)
    {
        if (entry.Id == default) entry.Id = Guid.NewGuid();
        _entries.Add(entry);
        return Task.FromResult(entry);
    }

    public Task<IExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, IExampleSentence exampleSentence)
    {
        if (exampleSentence.Id == default) exampleSentence.Id = Guid.NewGuid();
        var entry = _entries.Single(e => e.Id == entryId);
        var sense = entry.Senses.Single(s => s.Id == senseId);
        sense.ExampleSentences.Add(exampleSentence);
        return Task.FromResult(exampleSentence);
    }

    public Task<ISense> CreateSense(Guid entryId, ISense sense)
    {
        if (sense.Id == default) sense.Id = Guid.NewGuid();
        var entry = _entries.Single(e => e.Id == entryId);
        entry.Senses.Add(sense);
        return Task.FromResult(sense);
    }


    public Task DeleteEntry(Guid id)
    {
        _entries.RemoveAll(e => e.Id == id);
        return Task.CompletedTask;
    }

    public Task DeleteExampleSentence(Guid entryId, Guid senseId, Guid exampleSentenceId)
    {
        var entry = _entries.Single(e => e.Id == entryId);
        var sense = entry.Senses.Single(s => s.Id == senseId);
        sense.ExampleSentences.RemoveAll(es => es.Id == exampleSentenceId);
        return Task.CompletedTask;
    }

    public Task DeleteSense(Guid entryId, Guid senseId)
    {
        var entry = _entries.Single(e => e.Id == entryId);
        entry.Senses.RemoveAll(s => s.Id == senseId);
        return Task.CompletedTask;
    }

    public Task<IEntry[]> GetEntries(string exemplar, QueryOptions? options = null)
    {
        var entries = _entries.Where(e => e.LexemeForm.Values["en"].StartsWith(exemplar)).OfType<IEntry>().ToArray();
        return Task.FromResult(entries);
    }

    public Task<IEntry[]> GetEntries(QueryOptions? options = null)
    {
        return Task.FromResult(_entries.OfType<IEntry>().ToArray());
    }

    public Task<IEntry> GetEntry(Guid id)
    {
        var entry = _entries.Single(e => e.Id == id);
        return Task.FromResult(entry as IEntry);
    }

    public Task<string[]> GetExemplars()
    {
        return Task.FromResult(_exemplars);
    }

    public Task<WritingSystem[]> GetWritingSystems()
    {
        return Task.FromResult(_writingSystems.ToArray());
    }

    public Task<IEntry[]> SearchEntries(string query, QueryOptions? options = null)
    {
        var entries = _entries.Where(e => e.LexemeForm.Values["en"].Contains(query)).OfType<IEntry>().ToArray();
        return Task.FromResult(entries);
    }

    public UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class
    {
        return new JsonPatchUpdateBuilder<T>();
    }

    public Task<IEntry> UpdateEntry(Guid id, UpdateObjectInput<IEntry> update)
    {
        var entry = _entries.Single(e => e.Id == id);
        update.Apply(entry);
        return Task.FromResult(entry as IEntry);
    }

    public Task<IExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        UpdateObjectInput<IExampleSentence> update)
    {
        var entry = _entries.Single(e => e.Id == entryId);
        var sense = entry.Senses.Single(s => s.Id == senseId);
        var es = sense.ExampleSentences.Single(es => es.Id == exampleSentenceId);
        update.Apply(es);
        return Task.FromResult(es);
    }

    public Task<ISense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<ISense> update)
    {
        var entry = _entries.Single(e => e.Id == entryId);
        var s = entry.Senses.Single(s => s.Id == senseId);
        update.Apply(s);
        return Task.FromResult(s);
    }
}

internal static class Helpers
{
    public static void RemoveAll<T>(this IList<T> list, Func<T, bool> predicate)
    {
        for (var i = list.Count - 1; i >= 0; i--)
        {
            if (predicate(list[i]))
            {
                list.RemoveAt(i);
            }
        }
    }
}