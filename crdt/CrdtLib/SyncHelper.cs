using System.Text.Json;

namespace CrdtLib;

public static class SyncHelper
{
    /// <summary>
    /// simple sync example, each ISyncable could be over the wire or in memory
    /// prefer that remote is over the wire for the best performance, however they could both be remote
    /// </summary>
    /// <param name="localModel"></param>
    /// <param name="remoteModel"></param>
    /// <param name="serializerOptions"></param>
    internal static async Task<SyncResults> SyncWith(ISyncable localModel,
        ISyncable remoteModel,
        JsonSerializerOptions serializerOptions)
    {
        var localHeads = await localModel.GetSyncState();

        var (missingFromLocal, remoteHeads) = await remoteModel.GetChanges(localHeads);
        //todo abort if local and remote heads are the same
        var (missingFromRemote, _) = await localModel.GetChanges(remoteHeads);
        if (localModel is DataModel && remoteModel is DataModel)
        {
            //cloning just to simulate the objects going over the wire
            missingFromLocal = Clone(missingFromLocal, serializerOptions);
            missingFromRemote = Clone(missingFromRemote, serializerOptions);
        }

        if (missingFromLocal.Count > 0)
            await localModel.AddRangeFromSync(missingFromLocal);
        if (missingFromRemote.Count > 0)
            await remoteModel.AddRangeFromSync(missingFromRemote);
        return new SyncResults(missingFromLocal, missingFromRemote);
    }

    public static async Task SyncMany(this ISyncable localModel, ISyncable[] remotes)
    {
        var localSyncState = await localModel.GetSyncState();
        var remoteSyncStates = new SyncState[remotes.Length];
        for (var i = 0; i < remotes.Length; i++)
        {
            var remote = remotes[i];
            var (missingFromLocal, remoteSyncState) = await remote.GetChanges(localSyncState);
            remoteSyncStates[i] = remoteSyncState;
            await localModel.AddRangeFromSync(missingFromLocal);
        }

        for (var i = 0; i < remotes.Length; i++)
        {
            var remote = remotes[i];
            var remoteSyncState = remoteSyncStates[i];
            var (missingFromRemote, _) = await localModel.GetChanges(remoteSyncState);
            await remote.AddRangeFromSync(missingFromRemote);
        }
    }

    private static T Clone<T>(this T source, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(source);
        var json = JsonSerializer.Serialize(source, options);
        var clone = JsonSerializer.Deserialize<T>(json,options);
        return clone ?? throw new NullReferenceException("unable to clone object type " + typeof(T));
    }
}