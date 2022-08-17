using Noctal.Stories.Models;

namespace Noctal.Stories;

public class StoryDetailViewModel : BaseViewModel
{
    public StoriesFeedItem? Item { get; private set; }

    private readonly int ItemId;

    public StoryDetailViewModel(int itemId, StoriesService storiesService)
    {
        ItemId = itemId;

        Item = storiesService.GetStory(itemId);
    }
}
