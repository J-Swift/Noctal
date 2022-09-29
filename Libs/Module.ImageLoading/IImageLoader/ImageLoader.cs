#if ANDROID
using Android.Content;
using Android.App;
using Bumptech.Glide;
using Fragment = AndroidX.Fragment.App.Fragment;

#elif IOS
using Foundation;

// ReSharper disable once RedundantUsingDirective
using ImageCaching.Nuke;

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
    public void LoadInto(IImageLoader.LoadRequest request)
    {
        var url = request.UrlPath == null ? null : new NSUrl(request.UrlPath);
        ImageCaching.Nuke.ImagePipeline.Shared.LoadImageWithUrl(
            url,
            request.Placeholder,
            request.Placeholder,
            request.View
        );
    }
#endif
}
