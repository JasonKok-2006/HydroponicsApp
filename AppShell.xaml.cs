namespace NightShift
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Routing.RegisterRoute(nameof(Temperature), typeof(Temperature));
            Routing.RegisterRoute(nameof(Humidity), typeof(Humidity));
            Routing.RegisterRoute(nameof(WaterLevel), typeof(WaterLevel));
            Routing.RegisterRoute(nameof(Settings), typeof(Settings));
            Routing.RegisterRoute(nameof(DataPage), typeof(DataPage));
            Routing.RegisterRoute(nameof(Credits), typeof(Credits));


            InitializeComponent();
        }
    }
}
