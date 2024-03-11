using CommunityToolkit.Maui.Markup;

namespace NightShift
{
    public partial class MainPage : ContentPage
    {
        int dotClickCount = 0;
        int pumpClickCount = 0;
        public MainPage()
        {
            InitializeComponent();


        }

        //Menu icon button event
        private void Dots_Clicked(object sender, EventArgs e)
        {

            dots.RelRotateTo(90);
            dotClickCount++;

            if (dotClickCount % 2 == 0)
            {
                gear.IsVisible = false;
                gear.IsEnabled = false;
                graph.IsVisible = false;
                graph.IsEnabled = false;
                border1.IsVisible = false;
                border2.IsVisible = false;
                dotsBg.Color = Colors.White;
            }

            if (dotClickCount % 2 == 1)
            {
                gear.IsVisible = true;
                gear.IsEnabled = true;
                graph.IsVisible = true;
                graph.IsEnabled = true;
                border1.IsVisible = true;
                border2.IsVisible = true;
                dotsBg.Color = Colors.LightGray;
            }
        }

        //settings button event
        private async void Gear_Clicked(object sender, EventArgs e)
        {
            gearBg.Color = Colors.LightGray;
            await Shell.Current.GoToAsync("Settings");
            gearBg.Color = Colors.White;
        }

        //graph button event
        private async void Graph_Clicked(object sender, EventArgs e)
        {
            graphBg.Color = Colors.LightGray;
            await Shell.Current.GoToAsync("DataPage");
            graphBg.Color = Colors.White;
        }

        //currently unused button event
        private void Bell_Clicked(object sender, EventArgs e)
        {

        }

        private void PumpPump_Clicked(object sender, EventArgs e)
        {
            //This will not change anything due to it already being on the same page
        }

        private async void PumpTemperature_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Temperature));
        }

        private async void PumpHumidity_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Humidity));
        }

        private async void PumpLevel_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(WaterLevel));
        }

        private void PumpControl_Clicked(object sender, EventArgs e)
        {
            pumpClickCount++;
            if (pumpClickCount % 2 == 0)
            {
                PumpControl.Source = "pumpoff.svg";
                PumpLabel.Text = "The pump is currently turned off";
            }
            else if (pumpClickCount % 2 == 1)
            {
                PumpControl.Source = "pumpon.svg";
                PumpLabel.Text = "The pump is currently turned on.";
            }
        }

    }

}

