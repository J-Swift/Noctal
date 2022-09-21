#if ANDROID
using Android.Content;
using Android.Widget;
using Android.App;
using Bumptech.Glide;
using Fragment = AndroidX.Fragment.App.Fragment;

#elif IOS
using Foundation;

// using Xamarin.Nuke;
#endif

namespace Noctal.ImageLoading;

public class ImageLoader : IImageLoader
{
#if ANDROID
    public void LoadInto(Activity context, ImageView view, string? urlPath)
    {
        InternalLoad(Glide.With(context), view, urlPath);
    }

    public void LoadInto(Fragment context, ImageView view, string? urlPath)
    {
        InternalLoad(Glide.With(context), view, urlPath);
    }

    public void LoadInto(Context context, ImageView view, string? urlPath)
    {
        InternalLoad(Glide.With(context), view, urlPath);
    }

    private void InternalLoad(RequestManager req, ImageView view, string? urlPath)
    {
        req.Load(urlPath).Into(view);
    }
#elif IOS
    public void LoadInto(UIImageView view, string? urlPath)
    {
        if (urlPath is null)
        {
            view.Image = null;
            return;
        }

        ImageCaching.Nuke.ImagePipeline.Shared.LoadImageWithUrl(
            new NSUrl(urlPath),
            (img, url) => view.Image = img
        );
    }
#endif
}
