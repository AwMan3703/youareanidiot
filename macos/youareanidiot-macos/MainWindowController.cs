using AppKit;
using Foundation;

namespace youareanidiot_macos
{
    [Register("MainWindowController")]
    public class MainWindowController : NSWindowController
    {
        public MainWindowController() : base("MainWindow")
        {
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            // Subscribe to the window's WillClose event
            Window.WillClose += WindowWillClose;
        }
        
        void WindowWillClose(object sender, EventArgs e)
        {
            // Create two new instances of the main window
            for (int i = 0; i < 2; i++)
            {
                var newWindowController = new MainWindowController();
                //newWindowController.Window.MakeKeyAndOrderFront(null);
                Console.WriteLine("doing");
                newWindowController.Window.Display();
            }
        }
    }
}