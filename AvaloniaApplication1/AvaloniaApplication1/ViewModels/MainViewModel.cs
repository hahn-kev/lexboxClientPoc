using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AppLayer;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.DependencyInjection;
using Mono.Unix.Native;

namespace AvaloniaApplication1.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public ObservableCollection<Entry> Entries { get; init; } =
    [
        new Entry("Hello"),
        new Entry("World"),
    ];

    public static Action? PlatformSpecificAction { get; set; }
    
    public static IStorageProvider GetStorageProvider()
    {
        PlatformSpecificAction?.Invoke();
        return App.Services.GetRequiredService<IStorageProvider>();
    }

    public async Task LoadProject()
    {
        var provider = GetStorageProvider();
        if (!provider.CanOpen) throw new InvalidOperationException("Cannot open");
        var result = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            AllowMultiple = false,
            FileTypeFilter = [new("FD Data") { Patterns = ["*.fwdata"] }]
        });
        var project = result switch 
        {
            [{ } file] => await ProjectLoader.LoadProject(await file.OpenReadAsync(), file.Name),
            _ => null
        };
        if (project is null)
            return;
        Entries.Clear();
        foreach (var projectEntry in project.Entries)
        {
            Entries.Add(projectEntry);
        }
    }
}