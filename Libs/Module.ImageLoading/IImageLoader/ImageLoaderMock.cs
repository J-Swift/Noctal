#if ANDROID
using Android.Content;
using Android.App;
using Fragment = AndroidX.Fragment.App.Fragment;
#endif

namespace Noctal.ImageLoading;

public class ImageLoaderMock : IImageLoader
{
#if ANDROID
    public void LoadInto(Activity context, IImageLoader.LoadRequest request)
    {
        throw new NotImplementedException();
    }

    public void LoadInto(Fragment context, IImageLoader.LoadRequest request)
    {
        throw new NotImplementedException();
    }

    public void LoadInto(Context context, IImageLoader.LoadRequest request)
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
