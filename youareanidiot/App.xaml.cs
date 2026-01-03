
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace youareanidiot;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string RunNormalArg = "--run-normal";

    private bool _isWatchdog;
    
    
    private const string FlagsFile = "iamanidiot.txt";
    private static string[] GetFlags()
    {
        return !File.Exists(FlagsFile) ? [] : File.ReadAllLines(FlagsFile);
    }
    private const string ALLOW_FORCEFUL_KILL_FLAG = "allow-forceful-kill";
    
    private const string WindowCountFile = "wnct-DELETE-ME.txt";
    public static void SaveWindowCount(int count)
    {
        File.WriteAllText(WindowCountFile, count.ToString());
    }
    public static int LoadWindowCount()
    {
        if (File.Exists(WindowCountFile))
            return int.Parse(File.ReadAllText(WindowCountFile));
        return 1; // default to 1
    }


    public const bool DoResurrectWindows = true;
    public const int ResurrectedWindowsPerOneDead = 2;

    // On start:
    /*
     * If this is the main process, start a helper process from the same executable;
     * if this is the helper process, watch the main process and resurrect it when killed.
     */
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (e.Args.Contains($"{RunNormalArg}")) { UiStartup(); }
        else { WatchdogStartup(); }
    }

    private void UiStartup()
    {
        _isWatchdog = false;
        int windowCount = LoadWindowCount();
        
        Console.WriteLine($"Restoring {windowCount} window(s)...");
        
        // Open as many main windows as needed
        for (int i = 0; i < windowCount; i++)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }

    private void WatchdogStartup()
    {
        _isWatchdog = true;
        ShutdownMode = ShutdownMode.OnExplicitShutdown;
        const int backoffTimeout = 100;
        
        while (true)
        {
            var mainProcess = Process.Start(
                Environment.ProcessPath!,
                $"{RunNormalArg}"
            );
            mainProcess.WaitForExit();

            if (mainProcess.ExitCode != 0 && GetFlags().Contains(ALLOW_FORCEFUL_KILL_FLAG))
            {
                Console.WriteLine("Main process crashed or was forcefully killed, " +
                                  $"and {FlagsFile} contains \"{ALLOW_FORCEFUL_KILL_FLAG}\". " +
                                  $"Standing down and terminating watchdog process!");
                Process.GetProcessById(Environment.ProcessId).Kill();
                break;
            }
            
            Thread.Sleep(backoffTimeout);
        }
    }

    // On graceful exit, restart the watchdog
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        
        File.Delete(WindowCountFile);

        if (_isWatchdog) { Process.Start(Environment.ProcessPath!); }
    }
}