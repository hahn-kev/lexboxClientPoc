using SIL.LCModel;
using StructureMap;

namespace AppLayer;

public class ProjectLoader
{
    private const string ProjectFolder = "/Projects";
    private const string TemplatesFolder = "/Templates";

    static ProjectLoader()
    {
        Directory.CreateDirectory(ProjectFolder);
        Directory.CreateDirectory(TemplatesFolder);
    }

    public static async Task<FlexProject> LoadProject(Stream projectFile, string fileName)
    {
        fileName = Path.GetFileName(fileName);
        var projectFilePath = Path.Combine(ProjectFolder, Path.GetFileNameWithoutExtension(fileName), fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(projectFilePath));
        var localFileCopy = File.Create(projectFilePath);
        await projectFile.CopyToAsync(localFileCopy);
        localFileCopy.Flush();
        var lcmDirectories = new LcmDirectories(ProjectFolder, TemplatesFolder);
        var progress = new LcmThreadedProgress();
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
            .Select(e => new Entry(e.LexemeFormOA.Form.VernacularDefaultWritingSystem.Text));
        Console.WriteLine("Done loading project");
        return new FlexProject(entries.ToArray());
    }
}

public class FlexProject(Entry[] entries)
{
    public Entry[] Entries { get; init; } = entries;
}

public record Entry(string LexemeForm);