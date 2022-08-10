namespace Noctal.Models;

public record FeedItem(int Id, string Url, string Title, string Submitter, string TimeAgo, int Score, int NumComments)
{
    public int Index { get; } = Id;
};
