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
        Console.WriteLine("shouldn't have closed that");
        MainWindow window2 = new MainWindow();
        MainWindow window3 = new MainWindow();
        window2.Show();
        window3.Show();
    }

    private void VideoControl_OnMediaEnded(object sender, RoutedEventArgs e)
    {
        VideoControl.Position = new TimeSpan(0, 0, 0, 0, 1);
    }
}