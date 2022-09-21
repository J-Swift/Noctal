#if ANDROID
using Android.App;
using Android.Content;
using Android.Widget;
using Fragment = AndroidX.Fragment.App.Fragment;
#endif

namespace Noctal.ImageLoading;

public interface IImageLoader
{
#if ANDROID
    public void LoadInto(Activity context, ImageView view, string? urlPath);
    public void LoadInto(Fragment context, ImageView view, string? urlPath);
    public void LoadInto(Context context, ImageView view, string? urlPath);
#elif IOS
    public void LoadInto(UIImageView view, string? urlPath);
#endif
}
