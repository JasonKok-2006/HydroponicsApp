using System.Reflection;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using System;
using System.Timers;
using System.Threading.Channels;
using System.Diagnostics;
using System.Text.Json;
using static NightShift.Humidity;
//using static Android.Provider.Contacts.Intents;
using Microsoft.Maui.Controls.Shapes;
using System.Net;

namespace NightShift;

public partial class Humidity : ContentPage
{
    int dotClickCount = 0;
    public double humidity = 0;
    public double rotator = 0;
    public double rotatorReset = 0;

    private System.Timers.Timer hTimer = new System.Timers.Timer(2000);

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

    public Humidity()
    {
        InitializeComponent();

        MakeGetRequest(HumidityDisplay, humidity, rotator, rotatorReset, Pointer);


        //rotator calculation
        rotator = ((humidity / 100) * 180);

        //rotates the pointer
        Pointer.RotateTo(rotator);

        //Display Text
        HumidityDisplay.Text = "The Greenhouse Humidity is at " + humidity + "%.";

        HumTimer();
    }

    private async void HumDots_Clicked(object sender, EventArgs e)
    {
        await HumDots.RelRotateTo(90);
        dotClickCount++;

        if (dotClickCount % 2 == 0)
        {
            HumGear.IsVisible = false;
            HumGear.IsEnabled = false;
            HumGraph.IsVisible = false;
            HumGraph.IsEnabled = false;
            HumBorder1.IsVisible = false;
            HumBorder2.IsVisible = false;
            HumdotsBg.Color = Colors.White;
        }

        if (dotClickCount % 2 == 1)
        {
            HumGear.IsVisible = true;
            HumGear.IsEnabled = true;
            HumGraph.IsVisible = true;
            HumGraph.IsEnabled = true;
            HumBorder1.IsVisible = true;
            HumBorder2.IsVisible = true;
            HumdotsBg.Color = Colors.LightGray;
        }
    }

    public static async void MakeGetRequest(Label lableName, double humidityValue, double rotatorValue, double resetValue, Image pic)
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
                    if (Convert.ToString(deserialisedContent.feeds[last - 1].field4) == null)
                    {
                        // assigns the text of the lable you pass through to the previous data point 
                        lableName.Text = "The Current GreenHouse Humidity is " + Convert.ToString(deserialisedContent.feeds[last - 2].field4) + "%";
                        
                        //this will set the humidity value into a variable
                        humidityValue = Convert.ToDouble(deserialisedContent.feeds[last - 2].field4);
                        
                        //this will reset the pointer
                        await pic.RotateTo(resetValue);

                        //this will make the new rotator value and reset value
                        rotatorValue = humidityValue;
                        resetValue = 0 - rotatorValue;

                        //this will rotate the pointer
                        await pic.RotateTo(rotatorValue);

                    }
                    else
                    {
                        // assigns the text of the lable you pass through to the previous data point 
                        lableName.Text = "The Current GreenHouse Humidity is " + Convert.ToString(deserialisedContent.feeds[last - 1].field4) + "%";

                        //this will set the humidity value into a variable
                        humidityValue = Convert.ToDouble(deserialisedContent.feeds[last - 1].field4);

                        //this will reset the pointer
                        await pic.RotateTo(resetValue);

                        //this will make the new rotator value and reset value
                        rotatorValue = humidityValue;
                        resetValue = 0 - rotatorValue;

                        //this will rotate the pointer
                        await pic.RotateTo(rotatorValue);
                    }

                    //if (Convert.ToString(deserialisedContent.feeds[last - 1].field2) == null)
                    //{
                    //    labelName2.Text = Convert.ToString(deserialisedContent.feeds[last - 2].field2);
                    //}
                    //else
                    //{
                    //    labelName2.Text = Convert.ToString(deserialisedContent.feeds[last - 1].field2);
                    //}

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
    private void HumTimer()
    {
        hTimer.Start();
        hTimer.Elapsed += hTimerEvent;
        hTimer.Enabled = true;
        hTimer.AutoReset = true;
    }

    private void hTimerEvent(object? sender, ElapsedEventArgs e)
    {
        MakeGetRequest(HumidityDisplay, humidity, rotator, rotatorReset, Pointer);

        //humidity = Convert.ToDouble(valueStore.Text);

        //rotator calculation
        rotator = ((humidity / 100) * 180);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (rotator != rotatorReset)
            {
                //puts pointer back to the original position
                Pointer.RotateTo(rotatorReset);

                //sets up new rotator reset
                rotatorReset = rotator;

                //rotates the pointer
                Pointer.RotateTo(rotator);

                //Display Text
                HumidityDisplay.Text = "The Greenhouse Humidity is at " + humidity + "%.";
            }
        });
    }

    private async void HumGear_Clicked(object sender, EventArgs e)
    {
        HumgearBg.Color = Colors.LightGray;
        await Shell.Current.GoToAsync("Settings");
        HumgearBg.Color = Colors.White;
    }

    private async void HumGraph_Clicked(object sender, EventArgs e)
    {
        HumgraphBg.Color = Colors.LightGray;
        await Shell.Current.GoToAsync("DataPage");
        HumgraphBg.Color = Colors.White;
    }

    private async void HumPump_Clicked(object sender, EventArgs e)
    {
        hTimer.Stop();
        await Shell.Current.GoToAsync("..");
    }

    private async void HumTemperature_Clicked(object sender, EventArgs e)
    {
        hTimer.Stop();
        await Shell.Current.GoToAsync("../Temperature");

    }

    private void HumHumidity_Clicked(object sender, EventArgs e)
    {
        //This will change nothing
    }

    private async void HumLevel_Clicked(object sender, EventArgs e)
    {
        hTimer.Stop();
        await Shell.Current.GoToAsync("../WaterLevel");
    }
}
