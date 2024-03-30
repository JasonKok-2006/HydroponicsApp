using System.Reflection;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using System;
using System.Timers;
using System.Threading.Channels;
using System.Diagnostics;
using System.Text.Json;
using static NightShift.WaterLevel;
//using static Android.Provider.Contacts.Intents;
using Microsoft.Maui.Controls.Shapes;
using System.Net;
using Aspose.Imaging;

namespace NightShift;

public partial class WaterLevel : ContentPage
{
    int dotClickCount = 0;
    double floatSwitch1 = 0;
    double floatSwitch2 = 0;

    private System.Timers.Timer lTimer = new System.Timers.Timer(3000);

    public class Rootobject
    {
        public Channel channel { get; set; }
        public Feed[] feeds { get; set; }
    }
    public class Channel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string field1 { get; set; }
        public string field2 { get; set; }
        public string field3 { get; set; }
        public string field4 { get; set; }
        public string field5 { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int last_entry_id { get; set; }
    }


    public class Feed
    {
        public DateTime created_at { get; set; }
        public int entry_id { get; set; }
        public string field1 { get; set; }
        public string field2 { get; set; }
        public string field3 { get; set; }
        public string field4 { get; set; }
        public string field5 { get; set; }
    }

    public WaterLevel()
    {
        InitializeComponent();

        MakeGetRequest(LevelLabel, floatSwitch1, floatSwitch2);

        switch(LevelLabel.Text)
        {
            case "The water level is currently high.":
                Level.Source = "waterlevelhigh.svg";
                break;
            case "The water level is currently mid.":
                Level.Source = "waterlevelmid.svg";
                break;
            case "The water level is currently low.":
                Level.Source = "waterlevelmlow.svg";
                break;
        }

        lvlTimer();
    }

    private void lvlTimer()
    {
        lTimer.Start();
        lTimer.Elapsed += lTimerEvent;
        lTimer.AutoReset = true;
        lTimer.Enabled = true;
    }

    private void lTimerEvent(object? sender, ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Call the method to make a GET request. This also updates the user interface.
            MakeGetRequest(LevelLabel, floatSwitch1, floatSwitch2);

            switch (LevelLabel.Text)
            {
                case "The water level is currently high.":
                    Level.Source = "waterlevelhigh.svg";
                    break;
                case "The water level is currently mid.":
                    Level.Source = "waterlevelmid.svg";
                    break;
                case "The water level is currently low.":
                    Level.Source = "waterlevelmlow.svg";
                    break;
            }
        });
    }


    public static async void MakeGetRequest(Label lableName, double FL1, double FL2)
    {
        // Replace "https://api.example.com" with the actual URL you want to send the GET request to
        string apiUrl = "https://api.thingspeak.com/channels/2365673/feeds.json?api_key=O4WHET556WZUT8BA&results=2";

        // Create an instance of HttpClient
        using (HttpClient httpClient = new HttpClient())
        {
            //using try catch to catch any errors that may occur
            try
            {
                // Send a GET request and get the response
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                // Check if the request was successful (status code in the range 200-299)
                if (response.IsSuccessStatusCode)
                {
                    // Read and display the response content as a string
                    var content = await response.Content.ReadAsStringAsync();

                    //converts the JSON conent into a list
                    var deserialisedContent = JsonSerializer.Deserialize<Rootobject>(content);

                    //Gets the length of the feeds list in desialisedContent for calcuation of the last item in the list
                    var last = deserialisedContent.feeds.Length;

                    //This assigns the value of the desrialsedcontent to a lable on the front end of the app
                    //in order to change this to your needs, change DataRet to what ever you have called your label

                    //deserialisedContent is an object of the RootObject Class, it has 2 attriubtes to access these use the dot notation
                    //feeds is a list which will contain the data from your fields on thingspeak whereas channels gives you information
                    //about the channel.

                    //checks if the field is null, if it is it will use the data from the previous update else it will use the latest. 
                    if (Convert.ToString(deserialisedContent.feeds[last - 1].field1) == null)
                    {
                        FL1 = Convert.ToDouble(deserialisedContent.feeds[last - 2].field1);
                    }
                    else
                    {
                        FL1 = Convert.ToDouble(deserialisedContent.feeds[last - 1].field1);
                    }

                    if (Convert.ToString(deserialisedContent.feeds[last - 1].field2) == null)
                    {
                        FL2 = Convert.ToDouble(deserialisedContent.feeds[last - 2].field2);
                    }
                    else
                    {
                        FL2 = Convert.ToDouble(deserialisedContent.feeds[last - 1].field2);
                    }

                    switch (FL1 + FL2)
                    {
                        case > 1.5:
                            lableName.Text = "The water level is currently high.";
                            break;
                        case > 0.5:
                            lableName.Text = "The water level is currently mid.";
                            break;
                        default:
                            lableName.Text = "The water level is currently low.";
                            break;

                    }

                    Debug.WriteLine(Convert.ToString(deserialisedContent.feeds[last - 1].field2));
                    Debug.WriteLine(Convert.ToString(deserialisedContent.feeds[last - 2].field2));

                   
                }
                else
                {
                    // Display the HTTP status code if the request was not successful
                    Debug.WriteLine("Error: " + response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("Error: " + ex.Message);
            }
        }
    }


    private async void LvlDots_Clicked(object sender, EventArgs e)
    {
        await LvlDots.RelRotateTo(90);
        dotClickCount++;

        if (dotClickCount % 2 == 0)
        {
            LvlGear.IsVisible = false;
            LvlGear.IsEnabled = false;
            LvlGraph.IsVisible = false;
            LvlGraph.IsEnabled = false;
            LvlBorder1.IsVisible = false;
            LvlBorder2.IsVisible = false;
            LvldotsBg.Color = Colors.White;
        }

        if (dotClickCount % 2 == 1)
        {
            LvlGear.IsVisible = true;
            LvlGear.IsEnabled = true;
            LvlGraph.IsVisible = true;
            LvlGraph.IsEnabled = true;
            LvlBorder1.IsVisible = true;
            LvlBorder2.IsVisible = true;
            LvldotsBg.Color = Colors.LightGray;
        }
    }

    private async void LvlGear_Clicked(object sender, EventArgs e)
    {
        LvlgearBg.Color = Colors.LightGray;
        await Shell.Current.GoToAsync("Settings");
        LvlgearBg.Color = Colors.White;
    }

    private async void LvlGraph_Clicked(object sender, EventArgs e)
    {
        LvlgraphBg.Color = Colors.LightGray;
        await Shell.Current.GoToAsync("DataPage");
        LvlgraphBg.Color = Colors.White;
    }

    private async void LvlPump_Clicked(object sender, EventArgs e)
    {
        lTimer.Stop();
        await Shell.Current.GoToAsync("..");
    }

    private async void LvlTemperature_Clicked(object sender, EventArgs e)
    {
        lTimer.Stop();
        await Shell.Current.GoToAsync("../Temperature");
    }

    private async void LvlHumidity_Clicked(object sender, EventArgs e)
    {
        lTimer.Stop();
        await Shell.Current.GoToAsync("../Humidity");
    }

    private void LvlLevel_Clicked(object sender, EventArgs e)
    {
        //This will not change anything due to it already being on the same page
    }

    private void TempChange_Clicked(object sender, EventArgs e)
    {

    }

    private void TempChange_Clicked_1(object sender, EventArgs e)
    {

    }
}
