namespace NightShift;

public partial class DataPage : ContentPage
{
    public DataPage()
    {
        InitializeComponent();
    }

    private async void Backarrow_DataPage_Clicked(object sender, EventArgs e)
    {
        DPBB.Color = Colors.LightGray;
        await Shell.Current.GoToAsync("..");
        DPBB.Color = Colors.White;
    }
}
