using System.Diagnostics;

namespace youareanidiot_macos;

[Register("AppDelegate")]
public class AppDelegate : NSApplicationDelegate
{
    public uint WindowCount = 0;

    public override void DidFinishLaunching(NSNotification notification)
    {
    }

    public override void WillTerminate(NSNotification notification)
    {
    }
    
    public override NSApplicationTerminateReply ApplicationShouldTerminate(NSApplication sender)
    {
        // Prevent the app from closing
        return NSApplicationTerminateReply.Cancel;
    }
}