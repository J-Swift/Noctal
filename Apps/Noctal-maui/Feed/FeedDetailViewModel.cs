using ReactiveUI;
using Noctal.Models;

namespace Noctal;

public class FeedDetailViewModel : BaseViewModel
{
    private string title = null!;
    public string Title
    {
        get => title;
        set => this.RaiseAndSetIfChanged(ref title, value);
    }

    private string url = null!;
    public string Url
    {
        get => url;
        set => this.RaiseAndSetIfChanged(ref url, value);
    }

    private int score;
    public int Score
    {
        get => score;
        set => this.RaiseAndSetIfChanged(ref score, value);
    }

    private string submitter = null!;
    public string Submitter
    {
        get => submitter;
        set => this.RaiseAndSetIfChanged(ref submitter, value);
    }

    private string timeAgo = null!;
    public string TimeAgo
    {
        get => timeAgo;
        set => this.RaiseAndSetIfChanged(ref timeAgo, value);
    }

    private readonly FeedItem _item;

    public FeedDetailViewModel(FeedItem item)
    {
        _item = item;

        Title = item.Title;
        Url = item.Url;
        Score = item.Score;
        Submitter = item.Submitter;
        TimeAgo = item.TimeAgo;
    }
}
