namespace Noctal.Account;

public class AccountViewModel : BaseViewModel
{
    public string Text { get; private set; }

    public AccountViewModel()
    {
        Text = "Account Page";
    }
}
