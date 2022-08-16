using DynamicData;
using Noctal.Stories.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

namespace Noctal;

public class StoriesViewModel : BaseViewModel
{
    private readonly SourceCache<StoriesFeedItem, int> _itemsCache = new(it => it.Id);
    public ReadOnlyObservableCollection<StoriesFeedItem> Items { get; }

    public StoriesViewModel()
    {
        _itemsCache.Connect()
            .Bind(out var items)
            .Subscribe()
            .DisposeWith(Subscriptions);

        Items = items;

        _itemsCache.AddOrUpdate(new StoriesFeedItem(1, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(2, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(3, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(4, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(5, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(6, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(7, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(8, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(9, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(10, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(11, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _itemsCache.AddOrUpdate(new StoriesFeedItem(12, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
    }
}
