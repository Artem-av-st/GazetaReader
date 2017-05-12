
namespace GazetaReaderFrontend.Model
{
    public class MyWeather
    {
        public string Temperature { get; private set; }
        public string WeatherIcon { get; private set; }
        public string Wind { get; private set; }

        public MyWeather(string temperature, string wind, string icon)
        {
            Temperature = temperature;
            Wind = wind;
            WeatherIcon = icon;
        }
    }
}
