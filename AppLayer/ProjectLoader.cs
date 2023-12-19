using System.Buffers;
using System.Globalization;
using SIL.LCModel;
using SIL.LCModel.Core.WritingSystems;
using SIL.LCModel.Utils;
using SIL.WritingSystems;

namespace AppLayer;

public class ProjectLoader
{
    private static string _basePath = "/lexbox-client";
    private static string ProjectFolder => Path.Join(_basePath, "Projects");
    private static string TemplatesFolder => Path.Join(_basePath, "Templates");
    private static bool _init;

    public static void Init(string basePath = "/lexbox-client")
    {
        if (_init)
        {
            return;
        }

        _basePath = basePath;

        Directory.CreateDirectory(ProjectFolder);
        Directory.CreateDirectory(TemplatesFolder);
        Directory.CreateDirectory(Path.Combine(GlobalWritingSystemRepository.DefaultBasePath, LdmlDataMapper.CurrentLdmlLibraryVersion.ToString()));
        Icu.Wrapper.Init();
        Sldr.Initialize();
        _init = true;
    }

    public static async Task<FlexProject> LoadProject(Stream projectFile, string fileName)
    {
        Init();
        var writingSystemRepository = new CoreGlobalWritingSystemRepository();
        Console.WriteLine("Writing system count: " + writingSystemRepository.Count);
        fileName = Path.GetFileName(fileName);
        var projectFilePath = Path.Combine(ProjectFolder, Path.GetFileNameWithoutExtension(fileName), fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(projectFilePath));
        var localFileCopy = File.Create(projectFilePath);
        Console.WriteLine("Copying file");
        await Copy(projectFile, localFileCopy, 81920, new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
        localFileCopy.Flush();
        localFileCopy.Close();
        Console.WriteLine("Done copying file");
        var lcmDirectories = new LcmDirectories(ProjectFolder, TemplatesFolder);
        var progress = new LcmThreadedProgress();
        Console.WriteLine("Creating cache");
        var cache = LcmCache.CreateCacheFromExistingData(
            new LcmProjectIdentifier(lcmDirectories, Path.GetFileNameWithoutExtension(fileName)),
            null,
            new LfLcmUi(progress.SynchronizeInvoke),
            lcmDirectories,
            new LcmSettings(),
            progress
        );
        Console.WriteLine("Done creating cache");
        var entries = cache.ServiceLocator.GetInstance<IRepository<ILexEntry>>()
            .AllInstances()
            .Select(e => new Entry(e.LexemeFormOA.Form.VernacularDefaultWritingSystem.Text))
            .ToArray();
        var first = entries.First();
        Console.WriteLine("Done loading project");
        Console.WriteLine($"First entry: {first.LexemeForm}");
        return new FlexProject(entries);
    }

    private static async Task Copy(Stream source,
        Stream destination,
        int bufferSize,
        CancellationToken cancellationToken)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        try
        {
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, cancellationToken)) != 0)
            {
                await destination.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}

public class FlexProject(Entry[] entries)
{
    public Entry[] Entries { get; init; } = entries;
}

public record Entry(string LexemeForm);