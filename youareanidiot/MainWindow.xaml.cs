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

    private static void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        ((App) Application.Current).AWindowWasClosed();
    }

    private void VideoControl_OnMediaEnded(object sender, RoutedEventArgs e)
    {
        VideoControl.Position = new TimeSpan(0, 0, 0, 0, 1);
    }
}