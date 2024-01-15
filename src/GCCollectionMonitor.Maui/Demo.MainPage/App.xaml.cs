using System.ComponentModel;

namespace Demo.MainPage;

public partial class App
{
    private readonly GCCollectionMonitor.Maui.GCCollectionMonitor _monitor;
    private Page? _currentPage;

    public App()
    {
        InitializeComponent();

        _monitor = new GCCollectionMonitor.Maui.GCCollectionMonitor();

        PropertyChanged += HandleMainPageChanged;

        MainPage = new NavigationPage(new MainPage());
    }

    private void HandleMainPageChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MainPage))
            return;

        Page? lastPage = _currentPage;
        if (lastPage is NavigationPage lastNavPage)
            lastNavPage.Popped -= NavPageOnPopped;
        
        _currentPage = MainPage;
        if(_currentPage is NavigationPage currentNavPage)
            currentNavPage.Popped += NavPageOnPopped;

        if (lastPage != null)
            _monitor.MonitorForCollection(lastPage);

        _monitor.CollectAndCheck();
    }

    private void NavPageOnPopped(object? sender, NavigationEventArgs e)
    {
        _monitor.MonitorForCollection(e.Page);
        _monitor.CollectAndCheck();
    }
}