using MetaFetcher.Models;
using System.Text;
using System.Text.RegularExpressions;

namespace MetaFetcher;

public class MetaFetcher : IMetaFetcher
{
    private const int BytesToRead = 2000;

    private static readonly Dictionary<string, int> FavIconScoreMappings = new()
    {
        { "apple-touch-icon", 20 }, { "shortcut icon", 10 },
    };

    public async Task<MetaResult> GetMeta(string urlPath)
    {
        var result = new MetaResult(null, null);

        try
        {
            if (!Uri.TryCreate(urlPath, UriKind.Absolute, out var url))
            {
                Console.WriteLine($"Invalid uri: [{urlPath}]");
                return result;
            }

            // https://stackoverflow.com/a/37937238/1273175
            using (var httpclient = new HttpClient())
            {
                var timeout = new CancellationTokenSource();
                timeout.CancelAfter(TimeSpan.FromSeconds(5));
                using (var response = await httpclient.GetAsync(urlPath, HttpCompletionOption.ResponseHeadersRead, timeout.Token).ConfigureAwait(false))
                {
                    await using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        var keepGoing = true;
                        var parser = new MetaParser();

                        while (keepGoing)
                        {
                            var buffer = new byte[BytesToRead];
                            var bytesRead = await stream.ReadAsync(buffer, timeout.Token).ConfigureAwait(false);
                            var partialHtml = Encoding.UTF8.GetString(buffer);

                            keepGoing = parser.Parse(partialHtml) && bytesRead > 0;
                        }

                        var favIconScore = -1;

                        foreach (var parserResult in parser.GetResults())
                        {
                            switch (parserResult)
                            {
                                case MetaParser.OgImageResult og:
                                    {
                                        if (og.OgProperty.ToLowerInvariant() == "og:image")
                                        {
                                            var fullUrl = new Uri(url, og.UrlPath);
                                            result = result with { OgImagePath = fullUrl.AbsoluteUri, };
                                        }

                                        break;
                                    }
                                case MetaParser.FaviconResult favIcon:
                                    {
                                        if (FavIconScoreMappings.TryGetValue(favIcon.IconType.ToLowerInvariant(), out var score))
                                        {
                                            if (score > favIconScore)
                                            {
                                                favIconScore = score;
                                                var fullUrl = new Uri(url, favIcon.UrlPath);
                                                result = result with { FavIconPath = fullUrl.AbsoluteUri, };
                                            }
                                        }

                                        break;
                                    }
                            }
                        }

                        return result;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching meta [{urlPath}]");
            Console.WriteLine(e.Message);
            return result;
        }
    }
}

internal class MetaParser
{
    private static readonly Regex HeadRegex = new("<head>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex HeadCloseRegex = new("</head>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    // TODO(jpr): single quotes

    private static readonly Regex MetaRegex = new("<meta [^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex MetaPropertyRegex = new("\\bproperty=\"(?<value>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex MetaContentRegex = new("\\bcontent=\"(?<value>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex LinkRegex = new("<link [^>]+>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex LinkRelRegex = new("\\brel=\"(?<value>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex LinkHrefRegex = new("\\bhref=\"(?<value>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private string _buffer = "";
    private bool _inHead;

    // <meta property="og:image" content="https://assets.reedpopcdn.com/ace_attorney_2_u3krTvK.jpg/BROK/thumbnail/1600x900/format/jpg/quality/80/ace_attorney_2_u3krTvK.jpg">
    // <link rel="apple-touch-icon" href="/static/a9f2b837e17a27f9bcc4c9fe84dd94ec/icon/apple-touch-icon-152x152.png">
    // <link rel="shortcut icon" href="/static/a9f2b837e17a27f9bcc4c9fe84dd94ec/icon/favicon.ico">
    // <link rel="icon" type="image/png" href="/path/to/icons/favicon-192x192.png" sizes="192x192">
    // <link rel="apple-touch-icon" sizes="180x180" href="/path/to/icons/apple-touch-icon-180x180.png">
    // <link rel="shortcut icon" href="/path/to/icons/favicon.ico">


    private bool _keepGoing = true;

    public bool Parse(string nextHtmlChunk)
    {
        _buffer += nextHtmlChunk;

        if (!_inHead && HeadRegex.IsMatch(_buffer))
        {
            _inHead = true;
        }

        if (_inHead && HeadCloseRegex.IsMatch(_buffer))
        {
            _keepGoing = false;
        }

        return _keepGoing;
    }

    public IList<ImageResult> GetResults()
    {
        var result = new List<ImageResult>();

        foreach (var link in LinkRegex.Matches(_buffer) as IList<Match>)
        {
            var rel = LinkRelRegex.Match(link.Value);
            var urlPath = LinkHrefRegex.Match(link.Value);

            if (rel.Success && urlPath.Success)
            {
                result.Add(new FaviconResult(urlPath.Groups["value"].Value, rel.Groups["value"].Value));
            }
        }

        foreach (var meta in MetaRegex.Matches(_buffer) as IList<Match>)
        {
            var ogType = MetaPropertyRegex.Match(meta.Value);
            var urlPath = MetaContentRegex.Match(meta.Value);

            if (ogType.Success && urlPath.Success)
            {
                result.Add(new OgImageResult(urlPath.Groups["value"].Value, ogType.Groups["value"].Value));
            }
        }

        return result;
    }

    public abstract record ImageResult(string UrlPath);
    public record OgImageResult(string UrlPath, string OgProperty) : ImageResult(UrlPath);
    public record FaviconResult(string UrlPath, string IconType) : ImageResult(UrlPath);
}
