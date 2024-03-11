using System.Reflection;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Markup;
using System;
using System.Timers;
using System.Threading.Channels;
using System.Diagnostics;
using System.Text.Json;
using static NightShift.Temperature;
//using static Android.Provider.Contacts.Intents;
using Microsoft.Maui.Controls.Shapes;
using System.Net;

namespace NightShift;

public partial class Temperature : ContentPage
{

    public int dotClickCount = 0;
    public double temperatureValue = 0;
    public double redBarHeight = 0;

    private System.Timers.Timer aTimer = new System.Timers.Timer(20000);


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


    public Temperature()
    {
        InitializeComponent();

        // Call the method to make a GET request. This also updates the user interface.
        MakeGetRequest(CurrentTemp, RedBar, BlankSpace, tip, temperatureValue, redBarHeight);


        //gets timer to run repeat
        setTimer();

    }

    //this is a public method this allows it to be used from outside of the class, there are 2 arguments for lables
    //add more if you need to or remove if you need to. 
    public static async void MakeGetRequest(Label lableName, BoxView red, BoxView white, Ellipse tip, double value, double height)
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
                    if (Convert.ToString(deserialisedContent.feeds[last - 1].field3) == null)
                    {
                        // assigns the text of the lable you pass through to the previous data point 
                        lableName.Text = "The Current GreenHouse Temperature is " + Convert.ToString(deserialisedContent.feeds[last - 2].field3) + "°C";
                        value = Convert.ToDouble(deserialisedContent.feeds[last - 2].field3);
                        if (value < 0)
                        {
                            value = 0;
                        }
                        else if (value > 40)
                        {
                            value = 40;
                        }

                        // Update UI elements
                        height = ((400 / 40) * value);
                        red.LayoutBounds(58, (225 + (400 - height)), 24, height);
                        white.LayoutBounds(58, 225, 24, (400 - height));
                        tip.LayoutBounds(58, (225 + (400 - height)) - 12, 24, 24);
                    }
                    else
                    {
                        // assigns the text of the lable you pass through to the latest data point
                        lableName.Text = "The Current GreenHouse Temperature is " + Convert.ToString(deserialisedContent.feeds[last - 1].field3) + "°C";
                        value = Convert.ToDouble(deserialisedContent.feeds[last - 1].field3);
                        if (value < 0)
                        {
                            value = 0;
                        }
                        else if (value > 40)
                        {
                            value = 40;
                        }

                        // Update UI elements
                        height = ((400 / 40) * value);
                        red.LayoutBounds(58, (225 + (400 - height)), 24, height);
                        white.LayoutBounds(58, 225, 24, (400 - height));
                        tip.LayoutBounds(58, (225 + (400 - height)) - 12, 24, 24);
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


    private void setTimer()
    {
        aTimer.Start();
        aTimer.Elapsed += OnTimedEvent;
        aTimer.AutoReset = true;
        aTimer.Enabled = true;
    }


    private void OnTimedEvent(Object? source, ElapsedEventArgs e)
    {

        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Call the method to make a GET request. This also updates the user interface.
            MakeGetRequest(CurrentTemp, RedBar, BlankSpace, tip, temperatureValue, redBarHeight);
        });

    }

    private async void TempDots_Clicked(object sender, EventArgs e)
    {
        await TempDots.RelRotateTo(90);
        dotClickCount++;

        if (dotClickCount % 2 == 0)
        {
            TempGear.IsVisible = false;
            TempGear.IsEnabled = false;
            TempGraph.IsVisible = false;
            TempGraph.IsEnabled = false;
            TempBorder1.IsVisible = false;
            TempBorder2.IsVisible = false;
            TempdotsBg.Color = Colors.White;
        }

        if (dotClickCount % 2 == 1)
        {
            TempGear.IsVisible = true;
            TempGear.IsEnabled = true;
            TempGraph.IsVisible = true;
            TempGraph.IsEnabled = true;
            TempBorder1.IsVisible = true;
            TempBorder2.IsVisible = true;
            TempdotsBg.Color = Colors.LightGray;
        }
    }

    private async void TempGear_Clicked(object sender, EventArgs e)
    {
        TempgearBg.Color = Colors.LightGray;
        await Shell.Current.GoToAsync("Settings");
        TempgearBg.Color = Colors.White;
    }

    private async void TempGraph_Clicked(object sender, EventArgs e)
    {
        TempgraphBg.Color = Colors.LightGray;
        await Shell.Current.GoToAsync("DataPage");
        TempgraphBg.Color = Colors.White;
    }

    private async void TempPump_Clicked(object sender, EventArgs e)
    {
        aTimer.Stop();
        await Shell.Current.GoToAsync("..");
    }

    private void TempTemperature_Clicked(object sender, EventArgs e)
    {
        //This wont Change anything due to it being on the current page
    }

    private async void TempHumidity_Clicked(object sender, EventArgs e)
    {
        aTimer.Stop();
        await Shell.Current.GoToAsync("../Humidity");
    }

    private async void TempLevel_Clicked(object sender, EventArgs e)
    {
        aTimer.Stop();
        await Shell.Current.GoToAsync("../WaterLevel");
    }

}
