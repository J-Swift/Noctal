namespace Noctal.Stories.Models;

public record StoriesFeedItem(int Id, string Url, string Title, string Submitter, string TimeAgo, int Score, int NumComments)
{
};
