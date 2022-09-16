using DynamicData;
using HN.Api;
using HN.Api.Models;
using Noctal.Stories.Models;
using System.Collections.ObjectModel;

namespace Noctal.Stories;

public class StoriesService
{
    private readonly IHNApi _hnApi;
    private readonly SourceCache<StoriesFeedItem, int> _storiesCache = new(it => it.Id);

    public StoriesService(IHNApi hnApi)
    {
        _hnApi = hnApi;
        _storiesCache.Connect()
            .Bind(out var stories)
            .Subscribe();

        Stories = stories;

        FetchStories();
    }

    public ReadOnlyObservableCollection<StoriesFeedItem> Stories { get; }

    private async void FetchStories()
    {
        var stories = await _hnApi.GetStoriesAsync().ConfigureAwait(false);

        _storiesCache.Edit(list =>
        {
            list.Clear();
            foreach (var story in stories)
            {
                list.AddOrUpdate(ItemFrom(story));
            }
        });
    }

    public StoriesFeedItem? GetStory(int id)
    {
        var res = _storiesCache.Lookup(id);
        return res.HasValue ? res.Value : null;
    }

    private StoriesFeedItem ItemFrom(Story story)
    {
        return new StoriesFeedItem(
            int.Parse(story.Id),
            story.UrlPath,
            story.Title,
            story.Author,
            ToTimeAgo(story.CreatedAt),
            story.Score,
            story.NumComments
        );
    }

    private string ToTimeAgo(DateTimeOffset date)
    {
        var delta = DateTimeOffset.UtcNow.Subtract(date);

        if (delta.TotalSeconds < 60)
        {
            return "Just now";
        }

        if (delta.TotalMinutes < 60)
        {
            return $"{(int)delta.TotalMinutes}m ago";
        }

        if (delta.TotalHours < 24)
        {
            return $"{(int)delta.TotalHours}h ago";
        }

        if (delta.TotalDays < 365)
        {
            return $"{(int)delta.TotalDays}d ago";
        }

        return $"{(int)(delta.TotalDays / 365)}y ago";
    }
}
