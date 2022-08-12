namespace Noctal.UI.Theming;

public interface ITheme
{
    Color BackgroundColor { get; }
    Color OnBackgroundColor { get; }
    Color SurfaceColor { get; }
    Color OnSurfaceColor { get; }
    Color ErrorColor { get; }
    Color OnErrorColor { get; }

    Color PrimaryColor { get; }
    Color OnPrimaryColor { get; }
    Color SecondaryColor { get; }
    Color OnSecondaryColor { get; }
}


#if IOS
public class AdaptiveTheme
{
    public AdaptiveTheme(ITheme lightTheme, ITheme darkTheme, bool prefersLightTheme)
    {
        BackgroundColor = ColorUtils.Adaptive(lightTheme.BackgroundColor, darkTheme.BackgroundColor, prefersLightTheme);
        OnBackgroundColor = ColorUtils.Adaptive(lightTheme.OnBackgroundColor, darkTheme.OnBackgroundColor, prefersLightTheme);
        SurfaceColor = ColorUtils.Adaptive(lightTheme.SurfaceColor, darkTheme.SurfaceColor, prefersLightTheme);
        OnSurfaceColor = ColorUtils.Adaptive(lightTheme.OnSurfaceColor, darkTheme.OnSurfaceColor, prefersLightTheme);
        ErrorColor = ColorUtils.Adaptive(lightTheme.ErrorColor, darkTheme.ErrorColor, prefersLightTheme);
        OnErrorColor = ColorUtils.Adaptive(lightTheme.OnErrorColor, darkTheme.OnErrorColor, prefersLightTheme);
        PrimaryColor = ColorUtils.Adaptive(lightTheme.PrimaryColor, darkTheme.PrimaryColor, prefersLightTheme);
        OnPrimaryColor = ColorUtils.Adaptive(lightTheme.OnPrimaryColor, darkTheme.OnPrimaryColor, prefersLightTheme);
        SecondaryColor = ColorUtils.Adaptive(lightTheme.SecondaryColor, darkTheme.SecondaryColor, prefersLightTheme);
        OnSecondaryColor = ColorUtils.Adaptive(lightTheme.OnSecondaryColor, darkTheme.OnSecondaryColor, prefersLightTheme);
    }

    public UIColor BackgroundColor { get; }
    public UIColor OnBackgroundColor { get; }
    public UIColor SurfaceColor { get; }
    public UIColor OnSurfaceColor { get; }
    public UIColor ErrorColor { get; }
    public UIColor OnErrorColor { get; }
    public UIColor PrimaryColor { get; }
    public UIColor OnPrimaryColor { get; }
    public UIColor SecondaryColor { get; }
    public UIColor OnSecondaryColor { get; }
}
#endif
