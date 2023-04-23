using ArtHouse2.Models;

namespace ArtHouse2;

public partial class App : Application
{
    public bool needArtTypeRefresh;
    public List<ArtType> AllArtTypes;
    public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}
}
