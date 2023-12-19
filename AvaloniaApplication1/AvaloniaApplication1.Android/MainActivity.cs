using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Android;

[Activity(
    Label = "AvaloniaApplication1.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        MainViewModel.PlatformSpecificAction = () =>
        {
            if (!Environment.IsExternalStorageManager)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.ManageExternalStorage }, 0);
            }
        };
        
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseReactiveUI();
    }
}