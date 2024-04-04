using Plugin.Maui.Audio;

namespace NightShift;

public partial class Settings : ContentPage
{
    //audioplayer initaliser
    private readonly IAudioManager audioManager;

    double[] volumes;

    public Settings(IAudioManager audioManager)
    {
        InitializeComponent();

        this.audioManager = audioManager;

    }

    public async void clicker()
    {
        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("mouse-click-153941.mp3"));
        player.Play();
    }

    public async void Backarrow_Settings_Clicked(object sender, EventArgs e)
    {

        clicker();

        SBB.Color = Colors.LightGray;
        volumes = [Convert.ToDouble(BgVolume.Text), Convert.ToDouble(SFXVolume.Text)];
        await Shell.Current.GoToAsync("..");
        SBB.Color = Colors.White;
    }

    public async void Credit_Clicked(object sender, EventArgs e)
    {
        clicker();

        volumes = [Convert.ToDouble(BgVolume.Text), Convert.ToDouble(SFXVolume.Text)];
        await Shell.Current.GoToAsync(nameof(Credits));
    }

    public void BackgroundSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        double backgroundValue = e.NewValue;
    }

    public void Sound_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        double soundValue = e.NewValue;

    }

}



