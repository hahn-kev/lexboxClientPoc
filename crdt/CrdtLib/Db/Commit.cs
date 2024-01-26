using System.Diagnostics.CodeAnalysis;
using System.IO.Hashing;
using System.Text.Json.Serialization;
using CrdtLib.Changes;

namespace CrdtLib.Db;

public class Commit
{
    [JsonConstructor]
    public Commit(Guid id, string hash, string parentHash)
    {
        Id = id;
        Hash = hash;
        ParentHash = parentHash;
    }
    public Commit(Guid id)
    {
        Id = id;
        Hash = GenerateHash("");
        ParentHash = "";
    }

    public Commit() : this(Guid.NewGuid())
    {
    }

    public (DateTimeOffset, Guid) CompareKey => (DateTime, Id);
    public Guid Id { get; }
    public DateTimeOffset DateTime { get; init; } = DateTimeOffset.UtcNow;
    public string Hash { get; private set; }
    public string ParentHash { get; private set; }

    public void SetParentHash(string parentHash)
    {
        Hash = GenerateHash(parentHash);
        ParentHash = parentHash;
    }

    public string GenerateHash(string parentHash)
    {
        var idBytes = Id.ToByteArray();
        var parentHashBytes = Convert.FromHexString(parentHash);
        Span<byte> hashBytes = stackalloc byte[idBytes.Length + parentHashBytes.Length];
        idBytes.AsSpan().CopyTo(hashBytes);
        parentHashBytes.AsSpan().CopyTo(hashBytes[idBytes.Length..]);
        return Convert.ToHexString(XxHash64.Hash(hashBytes));
    }

    public required Guid ClientId { get; init; }

    public void AddChange(IChange change)
    {
        ChangeEntities.Add(new ChangeEntity(change));
    }

    public List<ChangeEntity> ChangeEntities { get; init; } = new();

    public override string ToString()
    {
        return $"{Id} [{DateTime}]";
    }
}

public class ChangeEntity
{
    [SetsRequiredMembers]
    public ChangeEntity(IChange change)
    {
        Id = change.Id;
        Change = change;
        CommitId = change.CommitId;
        EntityId = change.EntityId;
    }

    [JsonConstructor]
    private ChangeEntity()
    {
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CommitId { get; set; }
    public Guid EntityId { get; set; }
    private IChange _change;

    public required IChange Change
    {
        get => _change;
        [MemberNotNull(nameof(_change))]
        set
        {
            _change = value;
            _change.Id = Id;
            _change.CommitId = CommitId;
            if (_change.EntityId != Guid.Empty) EntityId = _change.EntityId;
            _change.EntityId = EntityId;
        }
    }
}