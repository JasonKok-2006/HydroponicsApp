using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace NightShift
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<Settings>();
            

            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<Temperature>();
            builder.Services.AddTransient<Humidity>();
            builder.Services.AddTransient<WaterLevel>();
            builder.Services.AddTransient<Credits>();
            builder.Services.AddTransient<Settings>();
            builder.Services.AddTransient<DataPage>();

            return builder.Build();
        }
    }
}
