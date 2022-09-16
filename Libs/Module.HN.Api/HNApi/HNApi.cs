using HN.Api.Models;
using System.Net.Http.Json;

namespace HN.Api;

public class HNApi : IHNApi
{
    private readonly HttpClient _httpClient;

    public HNApi()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://hn.algolia.com") };
    }

    public async Task<IReadOnlyCollection<Story>> GetStoriesAsync()
    {
        var resp = await _httpClient.GetAsync("api/v1/search?tags=story");
        resp.EnsureSuccessStatusCode();
        var dto = await resp.Content.ReadFromJsonAsync<HnSearchDto>();
        return (dto?.hits ?? new List<StoryDto>()).Select(StoryFrom).ToList();
    }

    private Story StoryFrom(StoryDto dto)
    {
        return new Story(
            dto.objectID,
            dto.title,
            dto.author,
            dto.url,
            DateTimeOffset.Parse(dto.created_at),
            dto.points,
            dto.num_comments
        );
    }
}

// DTOs

internal class HnSearchDto
{
    public IList<StoryDto> hits { get; set; }
}

internal class StoryDto
{
    public string author { get; set; }
    public string created_at { get; set; }
    public int num_comments { get; set; }
    public string objectID { get; set; }
    public int points { get; set; }
    public string title { get; set; }
    public string url { get; set; }
}
