using System;
using System.Timers;
using System.Diagnostics;
using Plugin.Maui.Audio;
using System.Security.AccessControl;
using System.Runtime.CompilerServices;
using System.Reflection;
using static NightShift.Humidity;
using static NightShift.Settings;



namespace NightShift
{
    public class Rootobject
    {
        public Channel channel { get; set; }
        public Feed[] feeds { get; set; }
    }

    public class Channel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string field1 { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int last_entry_id { get; set; }
    }

    public class Feed
    {
        public DateTime created_at { get; set; }
        public int entry_id { get; set; }
        public string field1 { get; set; }
    }


    public partial class MainPage : ContentPage
    {
        //this timer will make sure to make it unable to send a piece of data through to thingspeak
        //due to the 15 second interval it has between being able to send data.
        private System.Timers.Timer sendTimer = new System.Timers.Timer(15000);
        private System.Timers.Timer playTimer = new System.Timers.Timer(20000);


        //audioplayer initaliser
        private readonly IAudioManager audioManager;

        int dotClickCount = 0;
        public double pumpClickCount = 0;
        int count = 15;
        public string pumpDataSend = "0";
        bool bgPlay = true;

        //an if statement with this boolean is used. if the timer is activated (sendAllowed = false), 
        //the user can not navigate between pages to resolve a bug which would have prohibited any data sent to thiinspeak.
        public bool sendAllowed = true;

        //string to concatenate at the end of the label. This will be at on/off. telling the user what state the pump is on.  
        string end = "on.";

        public MainPage(IAudioManager audioManager)
        {
            InitializeComponent();

            this.audioManager = audioManager;

            //even though this locks the app for the first 15 seconds of the app, it ensures that the pump will always be switched off when the first user input is made.
            SendGetRequest(pumpDataSend);
            SendData();
            backgroundMusic();
        }

        //background music sound effect
        private async void backgroundMusic()
        {
            var bgMusic = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("wandering-6394.wav"));
            
            if (bgPlay == true)
            {
                bgMusic.Play();
            }
            else
            {
                bgMusic.Stop();
                //bgMusic.Dispose();
            }

            playTimer.AutoReset = true;
            playTimer.Enabled = true;
            playTimer.Elapsed += loopSong;
            playTimer.Start();
        }

        private void loopSong(object? sender, ElapsedEventArgs e)
        {
            backgroundMusic();
        }

        //startup sound
        private async void build()
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("hammer-6145.wav"));
            player.Play();
        }

        //pump turn on sound effect
        private async void water()
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("water-125071.wav"));
            player.Play();
        }

        //pump turn off sound effect
        private async void drain()
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("draining-72763.wav"));
            player.Play();
        }

        //ready/refresh sound effect
        private async void chime()
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("chime-6346.mp3"));
            player.Play();
        }

        //clicker sound effect
        private async void clicker()
        {
            var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("mouse-click-153941.mp3"));
            player.Play();
        }

        private async void SendData()
        {
            sendTimer.Start();
            sendTimer.Elapsed += resetAllowance;
            sendTimer.Enabled = true;

            if(pumpClickCount == 0)
            {
                sendAllowed = false;
                Locker.Text = "Please wait while we get the app ready.";

                build();
            }
            else if(pumpClickCount % 2 == 1)
            {
                Locker.Text = "Please wait to turn the pump " + end;
                water();
            }
            else
            {
                Locker.Text = "Please wait to turn the pump " + end;
                drain();
            }

        }

        private async void resetAllowance(object? sender, ElapsedEventArgs e)
        {
            //this has to be done on main thread to avoid a bug causing a bigger data send delay.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //this allows the data to be sent again
                sendAllowed = true;
                Locker.Text = "You can now turn the pump " + end;


            });

            sendTimer.Stop();
            chime();
            bgPlay = true;
            backgroundMusic();
        }


        //Public method to send data to the API, one string argument for data from user input
        //public allows it to be called from outside of the class 
        public static async void SendGetRequest(string dataToSend)
        {
            //Url String, you will need to update this with your own URL, and change the field to what ever you need to be. 
            string apiurl = "https://api.thingspeak.com/update?api_key=0GSA7MHT09DYM7DP&field1=" + dataToSend;

            //creates a new http client for the connection
            using (HttpClient client = new HttpClient())
            {
                //uses a try catch block to catch any errors
                try
                {
                    //sends request
                    HttpResponseMessage response = await client.GetAsync(apiurl);
                    //if success it will write to the debug console with success
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Data write Success");
                    }
                    else
                    // else it will write the error 
                    {
                        Debug.WriteLine("Error: " + response.StatusCode);
                    }
                }
                //will write the error if there is one in the try catch block
                catch (HttpRequestException ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                }
            }
        }

        //Menu icon button event
        private async void Dots_Clicked(object sender, EventArgs e)
        {
            clicker();

            await dots.RelRotateTo(90);
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
            clicker();

            bgPlay = false;
            backgroundMusic();

            gearBg.Color = Colors.LightGray;
            await Shell.Current.GoToAsync("Settings");
            gearBg.Color = Colors.White;
     
        }

        //graph button event
        private async void Graph_Clicked(object sender, EventArgs e)
        {
            clicker();

            bgPlay = false;
            backgroundMusic();

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
            bgPlay = false;
            backgroundMusic();
            //This will not change anything due to it already being on the same page
        }

        private async void PumpTemperature_Clicked(object sender, EventArgs e)
        {
            bgPlay = false;
            backgroundMusic();
            clicker();
            await Shell.Current.GoToAsync(nameof(Temperature));   
        }

        private async void PumpHumidity_Clicked(object sender, EventArgs e)
        {
            bgPlay = false;
            backgroundMusic();
            clicker();
            await Shell.Current.GoToAsync(nameof(Humidity));
        }

        private async void PumpLevel_Clicked(object sender, EventArgs e)
        {
            bgPlay = false;
            backgroundMusic();
            clicker();
            await Shell.Current.GoToAsync(nameof(WaterLevel));
        }

        private async void PumpControl_Clicked(object sender, EventArgs e)
        {
            bgPlay = false;
            backgroundMusic();

            //this stops the dta from being meesed up if the button is pressed multiple times.
            if (sendAllowed == true)
            {
                clicker();

                //this will make sure that the data can not be sent (as there is a 15 second time cooldown to send the data to the channel).
                sendAllowed = false;

                //MakeGetRequest(PumpLabel, Locker, ThrowAwayStore, pumpClickCount);
                pumpClickCount++;

                if (pumpClickCount % 2 == 0)
                {
                    PumpControl.Source = "pumpoff.svg";
                    PumpLabel.Text = "The pump is currently turned off.";
                    //Locker.Text = "Please wait to turn the pump on.";
                    pumpDataSend = "0";
                    end = "on.";
                    SendGetRequest(pumpDataSend);
                    SendData();
                }
                else if (pumpClickCount % 2 == 1)
                {
                    PumpControl.Source = "pumpon.svg";
                    PumpLabel.Text = "The pump is currently turned on.";
                    //Locker.Text = "Please wait to turn the pump off.";
                    pumpDataSend = "1";
                    end = "off.";
                    SendGetRequest(pumpDataSend);
                    SendData();
                }
  

            }


        }

  
    }

    
}

