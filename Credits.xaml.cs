using Plugin.Maui.Audio;

namespace NightShift;

public partial class Credits : ContentPage
{
    //audioplayer initaliser
    private readonly IAudioManager audioManager;
    public Credits(IAudioManager audioManager)
    {
        InitializeComponent();

        this.audioManager = audioManager;
    }

    private async void clicker()
    {
        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("mouse-click-153941.mp3"));
        player.Play();
    }

    private async void Backarrow_Credits_Clicked(object sender, EventArgs e)
    {
        clicker();

        CBB.BackgroundColor = Colors.LightGray;
        await Shell.Current.GoToAsync("..");
        CBB.BackgroundColor = Colors.White;
    }
}
