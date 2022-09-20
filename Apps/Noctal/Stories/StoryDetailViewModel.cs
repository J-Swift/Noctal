using Noctal.Stories.Models;

namespace Noctal.Stories;

public class StoryDetailViewModel : BaseViewModel
{
    private readonly int ItemId;

    public StoryDetailViewModel(int itemId, StoriesService storiesService)
    {
        ItemId = itemId;

        Item = storiesService.GetStory(itemId);
    }

    public StoriesFeedItem? Item { get; }
}
