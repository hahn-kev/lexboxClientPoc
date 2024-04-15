using Microsoft.Extensions.Internal;

namespace CrdtLib.Helpers;

public record HybridDateTime
{
    public HybridDateTime(DateTimeOffset dateTime, long counter)
    {
        DateTime = dateTime;
        Counter = counter;
    }

    public static HybridDateTime ForTestingNow => new(DateTimeOffset.UtcNow, 0);
    public DateTimeOffset DateTime { get; init; }
    public long Counter { get; init; }
}

public interface IHybridDateTimeProvider
{
    HybridDateTime GetDateTime();
}

public class HybridDateTimeProvider(TimeProvider timeProvider, HybridDateTime lastDateTime) : IHybridDateTimeProvider
{
    public static readonly HybridDateTime DefaultLastDateTime = new(DateTimeOffset.MinValue, 0);
    private readonly object _lockObject = new();

    public HybridDateTime GetDateTime()
    {
        var now = new HybridDateTime(timeProvider.GetUtcNow(), 0);
        lock (_lockObject)
        {
            if (now.DateTime <= lastDateTime.DateTime)
            {
                now = new HybridDateTime(lastDateTime.DateTime, lastDateTime.Counter + 1);
            }

            lastDateTime = now;
        }

        return now;
    }
}