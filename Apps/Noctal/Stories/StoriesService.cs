using DynamicData;
using Noctal.Stories.Models;
using System.Collections.ObjectModel;

namespace Noctal.Stories;

public class StoriesService
{
    private readonly SourceCache<StoriesFeedItem, int> _storiesCache = new(it => it.Id);
    public ReadOnlyObservableCollection<StoriesFeedItem> Stories { get; }

    public StoriesService()
    {
        _storiesCache.Connect()
            .Bind(out var stories)
            .Subscribe();

        Stories = stories;

        _storiesCache.AddOrUpdate(new StoriesFeedItem(1, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(2, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(3, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(4, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(5, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(6, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(7, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(8, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(9, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(10, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(11, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _storiesCache.AddOrUpdate(new StoriesFeedItem(12, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
    }

    public StoriesFeedItem? GetStory(int id)
    {
        var res = _storiesCache.Lookup(id);
        return res.HasValue ? res.Value : null;
    }
}

