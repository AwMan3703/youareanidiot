using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace youareanidiot;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        Closing += OnWindowClosing;
        InitializeComponent();
    }

    private void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        if (!App.DoResurrectWindows)
        {
            Console.WriteLine("Executable is configured to not resurrect windows");
            return;
        }
        
        for (int i = 0; i < App.ResurrectedWindowsPerOneDead; i++)
        {
            MainWindow andanotherone = new MainWindow();
            andanotherone.Show();
        }
        
        int newWindowCount = App.LoadWindowCount() + App.ResurrectedWindowsPerOneDead - 1;
        App.SaveWindowCount(newWindowCount);
        Console.WriteLine($"Shouldn't have closed that! Window count is now {newWindowCount}");
    }

    private void VideoControl_OnMediaEnded(object sender, RoutedEventArgs e)
    {
        VideoControl.Position = new TimeSpan(0, 0, 0, 0, 1);
    }
}