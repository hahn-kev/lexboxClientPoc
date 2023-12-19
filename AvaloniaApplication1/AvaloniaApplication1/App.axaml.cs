using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.ViewModels;
using AvaloniaApplication1.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddAppServices();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            services.AddSingleton(desktop.MainWindow.StorageProvider);
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
            services.AddSingleton(() =>
            {
                var topLevel = TopLevel.GetTopLevel(singleViewPlatform.MainView);
                if (topLevel is null) throw new InvalidOperationException("Top level is null");
                return topLevel.StorageProvider;
            });
        }

        Services = services.BuildServiceProvider();

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public static IServiceProvider Services { get; private set; }
}