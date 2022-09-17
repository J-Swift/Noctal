using HN.Api;
using Noctal.Stories;

namespace Noctal;

public static class StartupExtensions
{
    public static void RegisterServices()
    {
        var registry = new ServiceProvider();

        var hnApi = new HNApiMock();
        var storiesSvc = new StoriesService(hnApi);

        registry.RegisterService<IHNApi>(hnApi);
        registry.RegisterService(storiesSvc);
    }
}
