#if ANDROID
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using Fragment = AndroidX.Fragment.App.Fragment;
#endif

namespace Noctal.ImageLoading;

public interface IImageLoader
{
#if ANDROID
    public record LoadRequest(ImageView View, string? UrlPath, Drawable? Placeholder = null);
    public void LoadInto(Activity context, LoadRequest request);
    public void LoadInto(Fragment context, LoadRequest request);
    public void LoadInto(Context context, LoadRequest request);
#elif IOS
    public record LoadRequest(UIImageView View, string? UrlPath, UIImage? Placeholder = null);
    public void LoadInto(LoadRequest request);
#endif
}
