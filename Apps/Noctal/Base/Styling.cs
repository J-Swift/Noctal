using Noctal.UI.Theming;

namespace Noctal;

public class LightTheme : ITheme
{
    public Color BackgroundColor { get; } = Colors.White;
    public Color OnBackgroundColor { get; } = Colors.Black;
    public Color SurfaceColor { get; } = Colors.White.AddLuminosity(-0.1f);
    public Color OnSurfaceColor { get; } = Colors.Black;
    public Color ErrorColor { get; } = Colors.Red;
    public Color OnErrorColor { get; } = Colors.White;

    public Color PrimaryColor { get; } = Color.FromRgb(60, 165, 255);
    public Color OnPrimaryColor { get; } = Colors.White;
    public Color SecondaryColor { get; } = Color.FromRgb(230, 135, 104);
    public Color OnSecondaryColor { get; } = Colors.White;
}

public class DarkTheme : ITheme
{
    public Color BackgroundColor { get; } = Color.FromRgb(42, 43, 45);
    public Color OnBackgroundColor { get; } = Colors.White;
    public Color SurfaceColor { get; } = Color.FromRgb(42, 43, 45).AddLuminosity(0.1f);
    public Color OnSurfaceColor { get; } = Colors.White;
    public Color ErrorColor { get; } = Colors.Red;
    public Color OnErrorColor { get; } = Colors.White;

    public Color PrimaryColor { get; } = Color.FromRgb(60, 165, 255);
    public Color OnPrimaryColor { get; } = Colors.White;
    public Color SecondaryColor { get; } = Color.FromRgb(230, 135, 104);
    public Color OnSecondaryColor { get; } = Colors.White;
}
