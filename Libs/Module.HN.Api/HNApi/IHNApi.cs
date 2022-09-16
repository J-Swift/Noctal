using HN.Api.Models;

namespace HN.Api;

public interface IHNApi
{
    public Task<IReadOnlyCollection<Story>> GetStoriesAsync();
}
