namespace Noctal.UI.Theming;

public static class ColorUtils
{
#if IOS
    public static UIColor Adaptive(Color lightThemeColor, Color darkThemeColor, bool preferLight = false)
    {
        return new UIColor((attrs) =>
        {
            bool isLight = false;
            switch (attrs.UserInterfaceStyle)
            {
                case UIUserInterfaceStyle.Light:
                    isLight = true;
                    break;
                case UIUserInterfaceStyle.Dark:
                    isLight = false;
                    break;
                case UIUserInterfaceStyle.Unspecified:
                    isLight = preferLight;
                    break;
            }
            return (isLight ? lightThemeColor : darkThemeColor).ToPlatform();
        });
    }
#endif
}
