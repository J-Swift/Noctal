namespace Noctal;

public class App : Application
{
    public App()
    {
        var coord = new ApplicationCoordinator();
        MainPage = coord.RootPage;
        coord.Start();
    }
}
