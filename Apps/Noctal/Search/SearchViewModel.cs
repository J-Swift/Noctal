namespace Noctal.Search;

public class SearchViewModel : BaseViewModel
{
    public string Text { get; private set; }

    public SearchViewModel()
    {
        Text = "Search Page";
    }
}
