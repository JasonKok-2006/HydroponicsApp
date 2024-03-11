namespace NightShift;

public partial class Credits : ContentPage
{
    public Credits()
    {
        InitializeComponent();
    }

    private async void Backarrow_Credits_Clicked(object sender, EventArgs e)
    {
        CBB.BackgroundColor = Colors.LightGray;
        await Shell.Current.GoToAsync("..");
        CBB.BackgroundColor = Colors.White;
    }
}
