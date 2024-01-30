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

    public static async Task<LcmCache> LoadCache(Stream projectFile, string fileName)
    {
        Init();
        fileName = Path.GetFileName(fileName);
        var projectFilePath = Path.Combine(ProjectFolder, Path.GetFileNameWithoutExtension(fileName), fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(projectFilePath));
        var localFileCopy = File.Create(projectFilePath);
        Console.WriteLine("Copying file");
        await projectFile.CopyToAsync(localFileCopy);
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
        return cache;
    }
}
