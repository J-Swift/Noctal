namespace Noctal;

public class ServiceProvider
{
    private static ServiceProvider _current = null!;
    private readonly Dictionary<Type, object> _registry = new();

    public ServiceProvider()
    {
        _current = this;
    }

    public static TService GetService<TService>() where TService : class
    {
        return _current.GetRequiredService<TService>();
    }

    public void RegisterService<TService>(TService service) where TService : notnull
    {
        _registry[typeof(TService)] = service;
    }

    public TService GetRequiredService<TService>() where TService : class
    {
        var svc = _registry[typeof(TService)] as TService;
        if (svc is null)
        {
            throw new Exception($"nothing registered for [{typeof(TService)}]");
        }

        return svc;
    }

//     public static IServiceProvider Current =>
// #if IOS
// 			AppDelegate.Current.Services;
// #elif ANDROID
//         MainApplication.Current.Services;
// #else
//             throw new NotImplementedException("Not implemented");
// #endif
//     public static TSerr
}
