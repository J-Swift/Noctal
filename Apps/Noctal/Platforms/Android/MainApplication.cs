using Android.App;
using Android.Runtime;

namespace Noctal;

[Application]
public class MainApplication : Application
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        StartupExtensions.RegisterServices();
    }
}
