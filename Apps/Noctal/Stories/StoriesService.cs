using DynamicData;
using HN.Api;
using HN.Api.Models;
using MetaFetcher.Models;
using Microsoft.Maui.ApplicationModel;
using Noctal.Stories.Models;
using System.Collections.ObjectModel;

namespace Noctal.Stories;

public class StoriesService
{
    private readonly IHNApi _hnApi;
    private readonly MetaFetcherQueue _queue;
    private readonly SourceCache<StoriesFeedItem, int> _storiesCache = new(it => it.Id);

    public StoriesService(IHNApi hnApi, MetaFetcher.MetaFetcher metaFetcher)
    {
        _hnApi = hnApi;
        _queue = new MetaFetcherQueue(3, metaFetcher);
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
        var items = new List<StoriesFeedItem>();

        _storiesCache.Edit(
            list =>
            {
                list.Clear();
                foreach (var story in stories)
                {
                    var item = ItemFrom(story);
                    items.Add(item);
                    list.AddOrUpdate(item);
                }
            }
        );

        foreach (var item in items)
        {
#pragma warning disable CS4014
            _queue.FetchMeta(
                item.Url,
                res =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _storiesCache.AddOrUpdate(item with
                        {
                            ImagePath = res.OgImagePath, FavIconPath = res.FavIconPath,
                        });
                    });
                }
            );
#pragma warning restore CS4014
        }
    }

    public StoriesFeedItem? GetStory(int id)
    {
        var res = _storiesCache.Lookup(id);
        return res.HasValue ? res.Value : null;
    }

    private StoriesFeedItem ItemFrom(Story story)
    {
        var urlPath = story.UrlPath;
        if (urlPath is null)
        {
            urlPath = story.TypeOfStory == StoryType.AskHn ? "https://news.ycombinator.com" : "<unknown>";
        }

        return new StoriesFeedItem(
            int.Parse(story.Id),
            urlPath,
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

        if (delta.TotalDays < 7)
        {
            return $"{(int)delta.TotalDays}d ago";
        }

        return date.ToString("MMMM d, yyyy");
    }
}

internal class MetaFetcherQueue
{
    private readonly MetaFetcher.MetaFetcher _fetcher;
    private readonly SemaphoreSlim _pool;

    public MetaFetcherQueue(int concurrentOpsLimit, MetaFetcher.MetaFetcher fetcher)
    {
        _fetcher = fetcher;
        _pool = new SemaphoreSlim(concurrentOpsLimit, concurrentOpsLimit);
    }

    public void FetchMeta(string urlPath, Action<MetaResult> onComplete)
    {
        Task.Run(async () =>
            {
                try
                {
                    await _pool.WaitAsync().ConfigureAwait(false);

                    var result = await _fetcher.GetMeta(urlPath).ConfigureAwait(false);
                    onComplete(result);

                    _pool.Release();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        );
    }
}
