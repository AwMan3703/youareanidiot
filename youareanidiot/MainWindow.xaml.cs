using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace youareanidiot;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DispatcherTimer _locationChangeTimer;
    private readonly Random _random = new Random();
    
    private readonly int _xBound, _yBound;

    public MainWindow()
    {
        _locationChangeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _locationChangeTimer.Tick += OnLocationChangeEnded;

        LocationChanged += OnLocationChanged;
        Closing += OnWindowClosing;
        InitializeComponent();
        
        _xBound = (int) (SystemParameters.WorkArea.Width - Width);
        _yBound = (int) (SystemParameters.WorkArea.Height - Height);
        
        Left = _random.Next(1, _xBound);
        Top = _random.Next(1, _yBound);
    }

    private void OnLocationChanged(object? sender, EventArgs e)
    {
        _locationChangeTimer.Stop();
        _locationChangeTimer.Start();
    }

    private void OnLocationChangeEnded(object? sender, EventArgs e)
    {
        _locationChangeTimer.Stop();
        
        if (Left > _xBound)
        { Left = _random.Next(1, _xBound); }
        if (Top > _yBound)
        { Top = _random.Next(1, _yBound); }
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