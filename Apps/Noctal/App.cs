namespace Noctal;

public class App : Application
{
    public App()
    {
        var coord = new ApplicationCoordinator();
        MainPage = coord.RootPage;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        coord.Start();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }
}
