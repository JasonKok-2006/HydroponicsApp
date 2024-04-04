using System;
using System.Timers;
using System.Diagnostics;
using System.Text.Json;
using Plugin.Maui.Audio;

namespace NightShift;

public partial class DataPage : ContentPage
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
        public object field5 { get; set; }
    }

    public double FL1State = 0;
    public double FL2State = 0;

    //audioplayer initaliser
    private readonly IAudioManager audioManager;

    private System.Timers.Timer Data = new System.Timers.Timer(5000);

    public DataPage(IAudioManager audioManager)
    {
        InitializeComponent();

        this.audioManager = audioManager;


        MakeGetRequest(DFL1, FL1State, DFL2, FL2State, DWL, DT, DH);
        dataRefresh();
    }

    private async void clicker()
    {
        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("mouse-click-153941.mp3"));
        player.Play();
    }

    private void dataRefresh()
    {
        Data.Start();
        Data.Enabled = true;
        Data.Elapsed += reload;
        Data.AutoReset = true;
    }

    private async void reload(object? sender, ElapsedEventArgs e)
    {
        MakeGetRequest(DFL1, FL1State, DFL2, FL2State, DWL, DT, DH);
    }

    //this is a public method this allows it to be used from outside of the class, there are 2 arguments for lables
    //add more if you need to or remove if you need to. 
    public static async void MakeGetRequest(Label FL1, double State1, Label FL2, double State2, Label WL, Label T, Label H)
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
                        // assigns the text of the lable you pass through to the previous data point 
                        State1 = Convert.ToDouble(deserialisedContent.feeds[last - 2].field1);
                    }
                    else
                    {
                        // assigns the text of the lable you pass through to the latest data point
                        State1 = Convert.ToDouble(deserialisedContent.feeds[last - 1].field1);
                    }

                    if (Convert.ToString(deserialisedContent.feeds[last - 1].field2) == null)
                    {
                        State2 = Convert.ToDouble(deserialisedContent.feeds[last - 2].field2);
                    }
                    else
                    {
                        State2 = Convert.ToDouble(deserialisedContent.feeds[last - 1].field2);
                    }

                    if (Convert.ToString(deserialisedContent.feeds[last - 1].field3) == null)
                    {
                        T.Text = Convert.ToString(deserialisedContent.feeds[last - 2].field3) + "°C";
                    }
                    else
                    {
                        T.Text = Convert.ToString(deserialisedContent.feeds[last - 1].field3) + "°C";
                    }

                    if (Convert.ToString(deserialisedContent.feeds[last - 1].field4) == null)
                    {
                        H.Text = Convert.ToString(deserialisedContent.feeds[last - 2].field4) + "%";
                    }
                    else
                    {
                        H.Text = Convert.ToString(deserialisedContent.feeds[last - 1].field4) + "%";
                    }

                    if (State1 > 0.5)
                    {
                        FL1.Text = "ON";
                    }
                    else
                    {
                        FL1.Text = "OFF";
                    }

                    if (State2 > 0.5)
                    {
                        FL2.Text = "ON";
                    }
                    else
                    {
                        FL2.Text = "OFF";
                    }

                    switch (State1 + State2)
                    {
                        case > 1.5:
                            WL.Text = "High";
                            break;
                        case > 0.5:
                            WL.Text = "Mid";
                            break;
                        default:
                            WL.Text = "Low";
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

    private async void Backarrow_DataPage_Clicked(object sender, EventArgs e)
    {
        clicker();

        DPBB.Color = Colors.LightGray;
        await Shell.Current.GoToAsync("..");
        DPBB.Color = Colors.White;
    }
}
