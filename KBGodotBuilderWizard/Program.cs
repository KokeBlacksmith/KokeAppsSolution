using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.ReactiveUI;
using KBAvaloniaCore.MessageBox;

namespace KBGodotBuilderWizard;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            AppDomain.CurrentDomain.UnhandledException += Program._OnUnhandledException;
            Program.BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Program._ShowUnhandledExceptionDialog(e);
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace().UseReactiveUI();
    }
    
    /// <summary>
    /// Handles exceptions that occur outside the Avalonia dispatcher
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void _OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Program._ShowUnhandledExceptionDialog(e.ExceptionObject as Exception);
    }
    
    private static void _ShowUnhandledExceptionDialog(Exception e)
    {
        Result result = Result.CreateFailure(e);
        MessageBoxHelper.ShowResultMessageDialog("Critical unhandled exception:", result);
    }
}