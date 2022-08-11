namespace Noctal.UI.Controls;

#if ANDROID
public class Label : Android.Widget.TextView
{
    public Label(Android.Content.Context? context) : base(context) { }
}
#elif IOS
public class Label : UIKit.UILabel { }
#endif

