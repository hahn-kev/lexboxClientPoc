using Windows.Storage.Pickers;
using AppLayer;
using Uno.Extensions;

namespace UnoApp1;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _file;

    [ObservableProperty]
    private string? _firstEntry;

    private FlexProject? _flexProject;

    [RelayCommand]
    private async Task OpenProject()
    {
        var folder = ApplicationData.Current.LocalFolder;
        var lexboxFolder = await folder.CreateFolderAsync("lexbox-client", CreationCollisionOption.OpenIfExists);
        try
        {
            ProjectLoader.Init(lexboxFolder.Path);
        }
        catch (Exception e)
        {
            this.Log().LogError(e, "Failed to initialize project loader, folder path:" + lexboxFolder.Path);
            throw;
        }

        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".fwdata");
        var result = await picker.PickSingleFileAsync();
        File = result.Path;
        var stream = await result.OpenStreamForReadAsync();
        _flexProject = await ProjectLoader.LoadProject(stream, result.Name);
        FirstEntry = _flexProject.Entries.First().LexemeForm;
    }
}
