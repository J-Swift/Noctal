using ReactiveUI;
using Noctal.Models;

namespace Noctal;

public class FeedDetailViewModel : BaseViewModel
{
    private string title;
    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    private readonly FeedItem _item;

    public FeedDetailViewModel(FeedItem item)
    {
        _item = item;

        Title = item.Title;
    }
}
