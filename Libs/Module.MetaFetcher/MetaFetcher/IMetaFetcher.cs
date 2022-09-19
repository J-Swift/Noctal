using MetaFetcher.Models;

namespace MetaFetcher;

public interface IMetaFetcher
{
    public Task<MetaResult> GetMeta(string urlPath);
}
