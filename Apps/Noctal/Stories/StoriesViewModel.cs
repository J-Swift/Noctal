using DynamicData;
using Noctal.Stories.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

namespace Noctal.Stories;

public class StoriesViewModel : BaseViewModel
{
    public ReadOnlyObservableCollection<StoriesFeedItem> Items { get; }

    public StoriesViewModel(StoriesService storiesService)
    {
        Items = storiesService.Stories;
    }

    public StoriesFeedItem GetItem(int id)
    {
        return Items.First(it => it.Id == id);
    }
}
