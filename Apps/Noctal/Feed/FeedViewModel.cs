using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using ReactiveUI;
using DynamicData;

namespace Noctal;

public class FeedViewModel : BaseViewModel
{
    public record FeedItem(int Id, string Url, string Title, string Submitter, string TimeAgo, int Score, int NumComments)
    {
        public int Index { get; } = Id;
    };

    private readonly SourceCache<FeedItem, int> _itemsCache = new(it => it.Id);
    public ReadOnlyObservableCollection<FeedItem> Items { get; }

    public FeedViewModel()
    {
        _itemsCache.Connect()
            .Bind(out var items)
            .Subscribe()
            .DisposeWith(Subscriptions);

        Items = items;

        _itemsCache.AddOrUpdate(new FeedItem(1, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _itemsCache.AddOrUpdate(new FeedItem(2, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _itemsCache.AddOrUpdate(new FeedItem(3, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
        _itemsCache.AddOrUpdate(new FeedItem(4, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _itemsCache.AddOrUpdate(new FeedItem(5, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _itemsCache.AddOrUpdate(new FeedItem(6, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
        _itemsCache.AddOrUpdate(new FeedItem(7, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _itemsCache.AddOrUpdate(new FeedItem(8, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _itemsCache.AddOrUpdate(new FeedItem(9, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
        _itemsCache.AddOrUpdate(new FeedItem(10, "redhat.com", "Podman can transfer container images without a registry", "kukx", "5h ago", 25, 5));
        _itemsCache.AddOrUpdate(new FeedItem(11, "thinkcomposer.com", "ThinkComposer. Flowcharts, Concept Maps, Mind Maps, Diagrams and Models", "Tomte", "18h ago", 35, 9));
        _itemsCache.AddOrUpdate(new FeedItem(12, "meyerweb.com", "When or If", "J-Swift", "5h ago", 5, 24));
    }
}
