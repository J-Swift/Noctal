using HN.Api;
using MetaFetcher;
using Noctal.Stories;

namespace Noctal;

public static class StartupExtensions
{
    public static void RegisterServices()
    {
        var registry = new ServiceProvider();

        var hnApi = new HNApiMock();
        var storiesSvc = new StoriesService(hnApi);
        var fetcher = new MetaFetcher.MetaFetcher();

        registry.RegisterService<IHNApi>(hnApi);
        registry.RegisterService<IMetaFetcher>(fetcher);
        registry.RegisterService(storiesSvc);
    }
}
