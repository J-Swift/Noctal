#if ANDROID
using Android.Content;
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
    public void LoadInto(Activity context, IImageLoader.LoadRequest request)
    {
        InternalLoad(Glide.With(context), request);
    }

    public void LoadInto(Fragment context, IImageLoader.LoadRequest request)
    {
        InternalLoad(Glide.With(context), request);
    }

    public void LoadInto(Context context, IImageLoader.LoadRequest request)
    {
        InternalLoad(Glide.With(context), request);
    }

    private void InternalLoad(RequestManager req, IImageLoader.LoadRequest request)
    {
        var config = req.Load(request.UrlPath);

        if (request.Placeholder is not null)
        {
            config = config.Placeholder(request.Placeholder);
        }

        config.Into(request.View);
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
