using System.ComponentModel;
using System.Windows;

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
        
        Console.WriteLine($"Shouldn't have closed that! Resurrecting {App.ResurrectedWindowsPerOneDead} new windows...");
        
        for (int i = 0; i < App.ResurrectedWindowsPerOneDead; i++)
        {
            MainWindow andanotherone = new MainWindow();
            andanotherone.Show();
        }
    }

    private void VideoControl_OnMediaEnded(object sender, RoutedEventArgs e)
    {
        VideoControl.Position = new TimeSpan(0, 0, 0, 0, 1);
    }
}