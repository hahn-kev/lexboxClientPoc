﻿using System.Linq.Expressions;
using CrdtLib.Entities;
using CrdtLib.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrdtLib.Db;

public class CrdtRepository(CrdtDbContext _dbContext, DateTimeOffset? currentTime = null)
{
    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _dbContext.Database.BeginTransactionAsync();
    }
    
    
    public async Task<bool> HasCommit(Guid commitId)
    {
        return await _dbContext.Commits.AnyAsync(c => c.Id == commitId);
    }

    public async Task<(Commit? oldestChange, Commit[] newCommits)> FilterExistingCommits(ICollection<Commit> commits)
    {
        Commit? oldestChange = null;
        var commitIdsToExclude = await _dbContext.Commits.Where(c => commits.Select(c => c.Id).Contains(c.Id))
            .Select(c => c.Id).ToArrayAsync();
        var newCommits = commits.ExceptBy(commitIdsToExclude, c => c.Id).Select(commit =>
        {
            if (oldestChange is null || commit.CompareKey.CompareTo(oldestChange.CompareKey) < 0) oldestChange = commit;
            return commit;
        }).ToArray(); //need to use ToArray because the select has side effects that must trigger before this method returns
        return (oldestChange, newCommits);
    }

    public async Task DeleteStaleSnapshots(Commit oldestChange)
    {
        //use oldest commit added to clear any snapshots that are based on a now incomplete history
        await _dbContext.Snapshots
            .Where(s => (s.Commit.DateTime == oldestChange.DateTime && s.CommitId > oldestChange.Id) ||
                        s.Commit.DateTime > oldestChange.DateTime).ExecuteDeleteAsync();
    }

    public IQueryable<Commit> CurrentCommits()
    {
        return _dbContext.Commits.DefaultOrder().Where(c => currentTime == null || c.DateTime <= currentTime);
    }

    public IQueryable<ObjectSnapshot> CurrentSnapshots()
    {
        return _dbContext.Snapshots.Where(snapshot =>
            _dbContext.Snapshots.GroupBy(s => s.EntityId,
                    (entityId, snapshots) => snapshots
                        .OrderByDescending(s => s.Commit.DateTime)
                        .ThenBy(s => s.CommitId)
                        .First(s => currentTime == null || s.Commit.DateTime <= currentTime).Id)
                .Contains(snapshot.Id));
    }

    public async Task<(Dictionary<Guid, ObjectSnapshot> currentSnapshots, Commit[] pendingCommits)> GetCurrentSnapshotsAndPendingCommits()
    {
        var snapshots = await CurrentSnapshots().ToDictionaryAsync(s => s.EntityId);

        if (snapshots.Count == 0) return (snapshots, []);
        var lastCommit = snapshots.Values.Select(s => s.Commit).MaxBy(c => (c.DateTime, c.Id));
        ArgumentNullException.ThrowIfNull(lastCommit);
        var newCommits = await CurrentCommits()
            .Include(c => c.ChangeEntities)
            .Where(c => lastCommit.DateTime < c.DateTime)
            .ToArrayAsync();
        return (snapshots, newCommits);
    }

    public async Task<Commit?> FindCommitByHash(string hash)
    {
        return await _dbContext.Commits.SingleOrDefaultAsync(c => c.Hash == hash);
    }

    public async Task<ObjectSnapshot?> FindSnapshot(Guid id)
    {
        return await _dbContext.Snapshots.FindAsync(id);
    }

    public async Task<ObjectSnapshot> GetCurrentSnapshotByObjectId(Guid objectId)
    {
        return await _dbContext.Snapshots.Include(s => s.Commit)
            .DefaultOrder()
            .LastAsync(s => s.EntityId == objectId && (currentTime == null || s.Commit.DateTime <= currentTime));
    }

    public async Task<IObjectBase> GetObjectBySnapshotId(Guid snapshotId)
    {

        var entity = await _dbContext.Snapshots
                         .Where(s => s.Id == snapshotId)
                         .Select(s => s.Entity)
                         .SingleOrDefaultAsync()
                     ?? throw new ArgumentException($"unable to find snapshot with id {snapshotId}");
        return entity;
    }

    public async Task<T> GetCurrent<T>(Guid objectId) where T: IObjectBase
    {
        var snapshot = await _dbContext.Snapshots
            .DefaultOrder()
            .LastAsync(s => s.EntityId == objectId && (currentTime == null || s.Commit.DateTime <= currentTime));
        return snapshot.Entity.Is<T>();
    }

    public IQueryable<T> GetCurrentObjects<T>(Expression<Func<ObjectSnapshot, bool>>? predicate = null) where T : IObjectBase
    {
        var typeName = DerivedTypeHelper.GetEntityDiscriminator<T>();
        var queryable = CurrentSnapshots().Where(s => s.TypeName == typeName && !s.EntityIsDeleted);
        if (predicate is not null) queryable = queryable.Where(predicate);
        return queryable.Select(s => (T)s.Entity);
    }

    public async Task<SyncState> GetCurrentSyncState()
    {
        return new(await _dbContext.Commits.Where(c => currentTime == null || c.DateTime <= currentTime).GroupBy(c => c.ClientId)
            .Select(g => new { ClientId = g.Key, DateTime = g.Max(c => c.DateTime) })
            .ToDictionaryAsync(c => c.ClientId, c => c.DateTime.Ticks));
    }

    public async Task<ChangesResult> GetChanges(SyncState remoteState)
    {
        var newHistory = new List<Commit>();
        var localSyncState = await GetCurrentSyncState();
        foreach (var (clientId, localTimestamp) in localSyncState.ClientHeads)
        {
            if (!remoteState.ClientHeads.TryGetValue(clientId, out var otherTimestamp))
            {
                //todo slow, it would be better if we could query on client id and get latest changes per client
                //client is new to the other history
                newHistory.AddRange(await _dbContext.Commits.Include(c => c.ChangeEntities).DefaultOrder().Where(c => c.ClientId == clientId)
                    .ToArrayAsync());
            }
            else if (localTimestamp > otherTimestamp)
            {
                var otherDt = new DateTimeOffset(otherTimestamp, TimeSpan.Zero);
                //todo even slower we want to also filter out changes that are already in the other history
                //client has newer history than the other history
                newHistory.AddRange(await _dbContext.Commits.Include(c => c.ChangeEntities).DefaultOrder()
                    .Where(c => c.ClientId == clientId && c.DateTime > otherDt)
                    .ToArrayAsync());
            }
        }

        return new (newHistory, localSyncState);
    }

    public async Task AddSnapshots(IEnumerable<ObjectSnapshot> snapshots)
    {
        _dbContext.Snapshots.AddRange(snapshots);
        await _dbContext.SaveChangesAsync();
    }

    public void AddIfNew(ObjectSnapshot snapshot)
    {
        if (!_dbContext.Snapshots.Local.Contains(snapshot))
            _dbContext.Snapshots.Add(snapshot);
    }
    
    public CrdtRepository GetScopedRepository(DateTimeOffset newCurrentTime)
    {
        return new CrdtRepository(_dbContext, newCurrentTime);
    }

    public async Task AddCommit(Commit commit)
    {
        _dbContext.Commits.Add(commit);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCommits(IEnumerable<Commit> commits)
    {
        _dbContext.Commits.AddRange(commits);
        await _dbContext.SaveChangesAsync();
    }
}