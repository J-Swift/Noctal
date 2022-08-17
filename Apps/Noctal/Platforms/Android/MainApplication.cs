using Android.App;
using Android.Runtime;
using AndroidX.AppCompat.App;

namespace Noctal;

[Application]
public class MainApplication : Application
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }
}
