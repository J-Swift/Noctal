using System.Collections.ObjectModel;
using System.Reactive.Disposables;

#if ANDROID
using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Noctal.Stories.Models;
#elif IOS
using CoreGraphics;
using Foundation;
using UIKit;
#endif

namespace Noctal.Stories;

public partial class StoriesPage : BasePage<StoriesViewModel>
{
    protected override StoriesViewModel CreateViewModel() => new();
}

#if ANDROID
public partial class StoriesPage
{
    private RecyclerView Feed { get; set; } = null!;

    protected override void BindView(CompositeDisposable disposables)
    {
    }

    protected override View CreateView(Context ctx)
    {
        var layoutManager = new LinearLayoutManager(ctx, LinearLayoutManager.Vertical, false);
        var adapter = new MyAdapter(SafeViewModel.Items);

        Feed = new RecyclerView(ctx);
        Feed.SetLayoutManager(layoutManager);
        Feed.SetAdapter(adapter);
        return Feed;
    }
}

class MyAdapter : RecyclerView.Adapter
{
    private readonly ReadOnlyObservableCollection<StoriesFeedItem> Items;

    public MyAdapter(ReadOnlyObservableCollection<StoriesFeedItem> items) : base()
    {
        Items = items;
    }

    public override int ItemCount => Items.Count;

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var view = LayoutInflater.FromContext(parent.Context!)?.Inflate(Android.Resource.Layout.SimpleListItem1, parent, false)!;
        return new ViewHolder(view);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        var tHolder = (ViewHolder)holder;
        var item = Items[position];

        tHolder.LblText.Text = item.Title;
    }

    private class ViewHolder : RecyclerView.ViewHolder
    {
        public readonly TextView LblText;

        public ViewHolder(View view) : base(view)
        {
            LblText = view.FindViewById<TextView>(Android.Resource.Id.Text1)!;
        }
    }
}
#elif IOS
public partial class StoriesPage : IUICollectionViewDataSource//, IUITableViewDelegate
{
    private UICollectionView Feed { get; set; } = null!;

    protected override void BindView(CompositeDisposable disposables)
    {
    }

    protected override UIView CreateView()
    {
        var view = new UIView();

        var itemSize = NSCollectionLayoutSize.Create(
            NSCollectionLayoutDimension.CreateFractionalWidth(1),
            NSCollectionLayoutDimension.CreateEstimated(160)
        );
        var item = NSCollectionLayoutItem.Create(itemSize);

        var groupSize = NSCollectionLayoutSize.Create(
            NSCollectionLayoutDimension.CreateFractionalWidth(1),
            NSCollectionLayoutDimension.CreateEstimated(160)
        );
        var group = NSCollectionLayoutGroup.CreateHorizontal(groupSize, item);

        var section = NSCollectionLayoutSection.Create(group);
        var layout = new UICollectionViewCompositionalLayout(section);

        Feed = new UICollectionView(CGRect.Empty, layout)
        {
            DataSource = this,
        };
        Feed.RegisterClassForCell(typeof(UICollectionViewCell), "cell");

        return Feed;
    }

    [Export("numberOfSectionsInCollectionView:")]
    public nint NumberOfSections(UICollectionView collectionView)
    {
        return 1;
    }

    [Export("collectionView:numberOfItemsInSection:")]
    public nint GetItemsCount(UICollectionView collectionView, nint section)
    {
        return SafeViewModel.Items.Count;
    }

    public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
    {
        var item = SafeViewModel.Items[indexPath.Row];
        var cell = (UICollectionViewCell)collectionView.DequeueReusableCell("cell", indexPath);

        var contentConfig = UIListContentConfiguration.ValueCellConfiguration;
        contentConfig.Text = item.Title;
        cell.ContentConfiguration = contentConfig;

        return cell;
    }
}
#endif
