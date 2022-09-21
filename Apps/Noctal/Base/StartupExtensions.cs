using HN.Api;
using MetaFetcher;
using Noctal.ImageLoading;
using Noctal.Stories;

namespace Noctal;

public static class StartupExtensions
{
    public static void RegisterServices()
    {
        var registry = new ServiceProvider();

        var hnApi = new HNApiMock();
        var fetcher = new MetaFetcher.MetaFetcher();
        var storiesSvc = new StoriesService(hnApi, fetcher);
        var imageLoader = new ImageLoader();

        registry.RegisterService<IHNApi>(hnApi);
        registry.RegisterService<IMetaFetcher>(fetcher);
        registry.RegisterService<IImageLoader>(imageLoader);
        registry.RegisterService(storiesSvc);
    }
}
