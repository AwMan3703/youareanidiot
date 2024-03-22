using ObjCRuntime;

namespace youareanidiot_macos;

public partial class ViewController : NSViewController
{
    private AppDelegate GetDelegate() { return NSApplication.SharedApplication.Delegate as AppDelegate; }
    protected ViewController(NativeHandle handle) : base(handle)
    {
        // This constructor is required if the view controller is loaded from a xib or a storyboard.
        // Do not put any initialization here, use ViewDidLoad instead.
    }

    public override void ViewDidLoad()
    {
        
    }
}