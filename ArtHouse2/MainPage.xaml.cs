using ArtHouse2.Data;
using ArtHouse2.Models;
using ArtHouse2.Utilities;
using System.Text;

namespace ArtHouse2;

public partial class MainPage : ContentPage
{
    private App thisapp;
	private List<ArtType> artTypes;
	private List<Artwork> artworks;

	public MainPage()
	{
		InitializeComponent();
        thisapp = Application.Current as App;
        thisapp.needArtTypeRefresh = true;
		artTypes = new List<ArtType> { new ArtType { ID = 0, Type = " All Art Types" } };
		artworks = new List<Artwork>();
	}

	protected override async void OnAppearing()
	{
        base.OnAppearing();
        //Disable Add Button and clear the ListView until data arrives
        btnAdd.IsEnabled = false;
        artworkList.ItemsSource = null;

        if (thisapp.needArtTypeRefresh)
        {
            await ShowArtTypes();
            //Remember, this will also trigger ShowArtworks()
        }
        else
        {
            await ShowArtworks();
        }
        //Enable the Add Button
        btnAdd.IsEnabled = true;
    }

	private async Task ShowArtTypes()
	{
        //Get the artTypes
        ArtTypeRepository atr = new ArtTypeRepository();
        try
        {
            thisapp.AllArtTypes = await atr.GetArtTypes();
            foreach (ArtType p in thisapp.AllArtTypes.OrderBy(d => d.Type))
            {
                artTypes.Add(p);
            }
            ddlArtTypes.ItemsSource = artTypes;
            thisapp.needArtTypeRefresh = false;
            ddlArtTypes.SelectedIndex = 0;
        }
        catch (ApiException apiEx)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Errors:");
            foreach (var error in apiEx.Errors)
            {
                sb.AppendLine("-" + error);
            }
            await DisplayAlert("Problem Getting List of Art Types:", sb.ToString(), "Ok");
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                if (ex.GetBaseException().Message.Contains("connection with the server"))
                {

                    await DisplayAlert("Error", "No connection with the server. Check that the Web Service is running and available and then click the Refresh button.", "Ok");
                }
                else
                {
                    await DisplayAlert("Error", "If the problem persists, please call your system administrator.", "Ok");
                }
            }
            else
            {
                if (ex.Message.Contains("NameResolutionFailure"))
                {
                    await DisplayAlert("Internet Access Error ", "Cannot resolve the Uri: " + Jeeves.DBUri.ToString(), "Ok");
                }
                else
                {
                    await DisplayAlert("General Error ", ex.Message, "Ok");
                }
            }
        }
    }

    private async void ddlArtTypes_SelectedIndexChanged(object sender, EventArgs e)
    {
        await ShowArtworks();
    }

    private async Task ShowArtworks()
    {
        Loading.IsRunning = true;
        int? ArtTypeID = ((ArtType)ddlArtTypes.SelectedItem)?.ID;
        ArtworkRepository r = new ArtworkRepository();
        try
        {
            if (ArtTypeID.GetValueOrDefault() > 0)
            {
                artworks = await r.GetArtworksByArtType(ArtTypeID.GetValueOrDefault());
            }
            else
            {
                artworks = await r.GetArtworks();
            }
            artworkList.ItemsSource = artworks;
            artworkList.SelectedItem = null;

        }
        catch (ApiException apiEx)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Errors:");
            foreach (var error in apiEx.Errors)
            {
                sb.AppendLine("-" + error);
            }
            //artworkList.IsVisible = false;
            await DisplayAlert("Error Getting Artworks:", sb.ToString(), "Ok");
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
            {
                if (ex.GetBaseException().Message.Contains("connection with the server"))
                {

                    await DisplayAlert("Error", "No connection with the server. Check that the Web Service is running and available and then click the Refresh button.", "Ok");
                }
                else
                {
                    await DisplayAlert("Error", "If the problem persists, please call your system administrator.", "Ok");
                }
            }
            else
            {
                await DisplayAlert("General Error", "If the problem persists, please call your system administrator.", "Ok");
            }
        }
        finally
        {
            Loading.IsRunning = false;
        }
    }

    private async void btnAdd_Clicked(object sender, EventArgs e)
    {
        Artwork newArtwork = new Artwork();
        newArtwork.ArtTypeID = 0; //Adding so show the prompt to select an art type
        var artworkDetailPage = new ArtworkDetailsPage();
        artworkDetailPage.BindingContext = newArtwork;
        await Navigation.PushAsync(artworkDetailPage);
    }

    private async void ArtworkSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            var artwork = (Artwork)e.SelectedItem;
            var artworkDetailPage = new ArtworkDetailsPage();
            artworkDetailPage.BindingContext = artwork;
            await Navigation.PushAsync(artworkDetailPage);
        }
    }
}


