using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Noctal.Models;

namespace Noctal;

public class FeedDetailViewModel : BaseViewModel
{
    [Reactive] public string Title { get; private set; }

    private readonly FeedItem _item;

    public FeedDetailViewModel(FeedItem item)
    {
        _item = item;

        Title = item.Title;
    }
}
