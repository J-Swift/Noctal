namespace Noctal.Stories.Models;

public record StoriesFeedItem(int Id, string Url, string Title, string Submitter, string TimeAgo, int Score, int NumComments, string? FavIconPath = null, string? ImagePath = null)
{
    public string? DisplayUrl { get; } = GetDisplayUrl(Url);
    public string? PlaceholderLetter { get; } = GetPlaceholderLetter(Url);

    private static string? GetDisplayUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return null;
        }

        var host = uri.Host;

        if (host.StartsWith("www."))
        {
            host = host[4..];
        }

        return host;
    }

    private static string? GetPlaceholderLetter(string url)
    {
        Uri.TryCreate(url, UriKind.Absolute, out var uri);

        string? text = null;
        if (uri is not null)
        {
            var parts = uri.Host.Split(".");
            var x = parts[^2];
            text = x[0].ToString();
        }

        return text;
    }
}
