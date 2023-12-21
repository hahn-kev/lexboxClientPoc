using System.Linq.Expressions;
using SystemTextJsonPatch;

namespace lexboxClientContracts;

public class InMemoryApi : ILexboxApi
{
    private readonly List<Entry> _entries =
    [
        new Entry
        {
            Id = Guid.NewGuid(),
            LexemeForm = new()
            {
                Values =
                {
                    { "en", "apple" },
                }
            },
            Senses =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Definition = new()
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
                            Sentence = new()
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
            LexemeForm = new()
            {
                Values =
                {
                    { "en", "banana" },
                }
            },
            Senses =
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Definition = new()
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
                            Sentence = new()
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

    public Task<Entry> CreateEntry(Entry entry)
    {
        _entries.Add(entry);
        return Task.FromResult(entry);
    }

    public Task<ExampleSentence> CreateExampleSentence(Guid entryId, Guid senseId, ExampleSentence exampleSentence)
    {
        var entry = _entries.Single(e => e.Id == entryId);
        var sense = entry.Senses.Single(s => s.Id == senseId);
        sense.ExampleSentences.Add(exampleSentence);
        return Task.FromResult(exampleSentence);
    }

    public Task<Sense> CreateSense(Guid entryId, Sense sense)
    {
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

    public Task<Entry[]> GetEntries(string exemplar, QueryOptions? options = null)
    {
        var entries = _entries.Where(e => e.LexemeForm.Values["en"].StartsWith(exemplar)).ToArray();
        return Task.FromResult(entries);
    }

    public Task<Entry[]> GetEntries(QueryOptions? options = null)
    {
        return Task.FromResult(_entries.ToArray());
    }

    public Task<Entry> GetEntry(Guid id)
    {
        var entry = _entries.Single(e => e.Id == id);
        return Task.FromResult(entry);
    }

    public Task<string[]> GetExemplars()
    {
        return Task.FromResult(_exemplars);
    }

    public Task<WritingSystem[]> GetWritingSystems()
    {
        return Task.FromResult(_writingSystems.ToArray());
    }

    public Task<Entry[]> SearchEntries(string query, QueryOptions? options = null)
    {
        var entries = _entries.Where(e => e.LexemeForm.Values["en"].Contains(query)).ToArray();
        return Task.FromResult(entries);
    }

    public UpdateBuilder<T> CreateUpdateBuilder<T>() where T : class
    {
        return new InMemoryUpdateBuilder<T>();
    }

    private class InMemoryUpdateBuilder<T> : UpdateBuilder<T> where T : class
    {
        private readonly JsonPatchDocument<T> _patchDocument = new();
        public UpdateObjectInput<T> Build()
        {
            return new InMemoryUpdateObjectInput<T>(_patchDocument);
        }

        public UpdateBuilder<T> Set<T_Val>(Expression<Func<T, T_Val>> field, T_Val value)
        {
            _patchDocument.Replace(field, value);
            return this;
        }
    }

    internal class InMemoryUpdateObjectInput<T>(JsonPatchDocument<T> patchDocument) : UpdateObjectInput<T>
        where T : class
    {
        public void Apply(T obj)
        {
            patchDocument.ApplyTo(obj);
        }
    }

    public Task<Entry> UpdateEntry(Guid id, UpdateObjectInput<Entry> update)
    {
        var entry = _entries.Single(e => e.Id == id);
        update.Apply(entry);
        return Task.FromResult(entry);
    }

    public Task<ExampleSentence> UpdateExampleSentence(Guid entryId,
        Guid senseId,
        Guid exampleSentenceId,
        UpdateObjectInput<ExampleSentence> exampleSentence)
    {
        var entry = _entries.Single(e => e.Id == entryId);
        var sense = entry.Senses.Single(s => s.Id == senseId);
        var es = sense.ExampleSentences.Single(es => es.Id == exampleSentenceId);
        exampleSentence.Apply(es);
        return Task.FromResult(es);
    }

    public Task<Sense> UpdateSense(Guid entryId, Guid senseId, UpdateObjectInput<Sense> sense)
    {
        var entry = _entries.Single(e => e.Id == entryId);
        var s = entry.Senses.Single(s => s.Id == senseId);
        sense.Apply(s);
        return Task.FromResult(s);
    }
}