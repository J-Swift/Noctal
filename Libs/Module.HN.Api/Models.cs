namespace HN.Api.Models;

public record Story(string Id, string Title, string Author, string UrlPath, DateTimeOffset CreatedAt, int Score, int NumComments);
