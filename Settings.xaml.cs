namespace NightShift;

public partial class Settings : ContentPage
{
    public Settings()
    {
        InitializeComponent();
    }

    private async void Backarrow_Settings_Clicked(object sender, EventArgs e)
    {
        SBB.Color = Colors.LightGray;
        await Shell.Current.GoToAsync("..");
        SBB.Color = Colors.White;
    }

    private async void Credit_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Credits));
    }
}

