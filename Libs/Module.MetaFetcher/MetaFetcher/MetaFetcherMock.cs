using MetaFetcher.Models;

namespace MetaFetcher;

public class MetaFetcherMock : IMetaFetcher
{
    public async Task<MetaResult> GetMeta(string urlPath)
    {
        await Task.Delay(2000);

        return new MetaResult(null, null);
    }
}
