#if ANDROID
using Android.Content;
using Android.Widget;
using Android.App;
using Fragment = AndroidX.Fragment.App.Fragment;
#endif

namespace Noctal.ImageLoading;

public class ImageLoaderMock : IImageLoader
{
#if ANDROID
    public void LoadInto(Activity context, ImageView view, string? urlPath)
    {
        throw new NotImplementedException();
    }

    public void LoadInto(Fragment context, ImageView view, string? urlPath)
    {
        throw new NotImplementedException();
    }

    public void LoadInto(Context context, ImageView view, string? urlPath)
    {
        throw new NotImplementedException();
    }
#elif IOS
    public void LoadInto(UIImageView view, string? urlPath)
    {
        throw new NotImplementedException();
    }
#endif
}
