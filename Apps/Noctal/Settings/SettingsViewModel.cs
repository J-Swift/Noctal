namespace Noctal.Settings;

public class SettingsViewModel : BaseViewModel
{
    public string Text { get; private set; }

    public SettingsViewModel()
    {
        Text = "Settings Page";
    }
}
