namespace HN.Api.Models;

public enum StoryType
{
    Story,
    ShowHn,
    AskHn,
}

public record Story(string Id, string Title, string Author, string? UrlPath, DateTimeOffset CreatedAt, int Score, int NumComments, StoryType TypeOfStory, string? StoryText);
