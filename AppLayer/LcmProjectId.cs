using SIL.LCModel;

namespace AppLayer;

public record LcmDirectories(string ProjectsDirectory, string TemplateDirectory) : ILcmDirectories;

class LcmProjectIdentifier : IProjectIdentifier
{
    public LcmProjectIdentifier(ILcmDirectories LcmDirs, string database)
    {
        LcmDirectories = LcmDirs;
        Path = GetPathToDatabase(LcmDirs, database);
    }

    private static string GetPathToDatabase(ILcmDirectories LcmDirs, string database)
    {
        return System.IO.Path.Combine(LcmDirs.ProjectsDirectory,
            database,
            database + LcmFileHelper.ksFwDataXmlFileExtension);
    }

    public ILcmDirectories LcmDirectories { get; private set; }

    public bool IsLocal => true;

    public string Path { get; set; }

    public string ProjectFolder => System.IO.Path.GetDirectoryName(Path);

    public string SharedProjectFolder => ProjectFolder;

    public string ServerName => null;

    public string Handle => Name;

    public string PipeHandle => throw new NotSupportedException();

    public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

    public BackendProviderType Type => BackendProviderType.kXML;

    public string UiName => Name;
}