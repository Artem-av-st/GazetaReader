using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.Globalization;
using System.Runtime.Serialization.Json;
using BingMapsRESTService.Common.JSON;
using OpenWeatherMap;
using System.Runtime.Serialization;
using Windows.UI.Popups;
using GazetaReaderFrontend.Model;

namespace GazetaReaderFrontend.ViewModel.Services
{
    static class WeatherService
    {
        public static async Task<MyWeather> GetWeather()
        {
            // Ключ доступа к OpenWeatherMap.
            const string owmApiKey = "7a29ddb27ed8939647a190357541d344";
            string city = String.Empty;

            try
            {
                // Запрашиваем коодинаты устройства, если они доступны, то отпределяем по координатам город,
                // по названию города определяем погоду.
                city = await GetCity(await Geolocate());
            }

            // Если запрещен доступ приложению к геоданным
            catch (UnauthorizedAccessException)
            {
                await new MessageDialog("Невозможно определить Ваши координаты").ShowAsync();

                // TODO: реализовать открытие окна настроек доступа к геоданным, чтобы открыть приложению доступ к ним.
                /*if (await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location")))
                {
                    await GetWeather();
                }*/

                return null;
            }

            // Другие исключения, например, отсуствие подключения к сети, требуемое для определения координат.
            catch (Exception)
            {
                return null;
            }



            var weatherClient = new OpenWeatherMapClient(owmApiKey);

            // В качестве параметров задаем название города, и другие параметры. Также OWM клиент имеет метод для получения погоды по координатам
            // устройства. В данном конкретном примере определение названия города реализовано в учебных целях.
            var currentWeather = await weatherClient.CurrentWeather.GetByName(city, MetricSystem.Metric, OpenWeatherMapLanguage.RU);

            // Форматируем должным образом данные текущей погоды, в том числе задаем URI иконки, определенной OWM для текущей погоды.
            return new MyWeather(
                (Int32)currentWeather.Temperature.Value + "°C",
                (Int32)currentWeather.Wind.Speed.Value + " м/с",
                "http://openweathermap.org/img/w/" + currentWeather.Weather.Icon + ".png");



        }
        private static async Task<Geoposition> Geolocate()
        {
            
            var geolocator = new Geolocator {DesiredAccuracy = PositionAccuracy.High};
            var locationStatus = geolocator.LocationStatus;
            var position = await geolocator.GetGeopositionAsync();
            return position;
        }
        

        private static async Task<string> GetCity(Geoposition position)
        {
            var culture=new CultureInfo("en-US");

            // Ключ доступа к BingMaps API.
            const string bingApiKey = "AgDeteITUMycomwytDapPxEtYzVsXGgl7rjygsBaEiZKrNS7mvIGBBc_M3J4ky-8";

            var serializer=new DataContractJsonSerializer(typeof(Response));

            // Отправляем запрос к API, в качестве параметров указывая координаты устройства и ключ.
            var response = await ViewModelServices.GetHttpResponseAsync(
                String.Format(culture, "http://dev.virtualearth.net/REST/v1/Locations/{0},{1}?o=json&key={2}",
                position.Coordinate.Point.Position.Latitude,
                position.Coordinate.Point.Position.Longitude,
                bingApiKey));

           
            Response restApiResponse;
            try
            {
                restApiResponse = serializer.ReadObject(response) as Response;

            }
            catch (SerializationException)
            {

                throw;
            }

            // Покопавшись в документации к API, выясняем, где в ответе найти город. Возвращаем его.
            return restApiResponse.ResourceSets[0].Resources[0].Address.AdminDistrict2;

            
        }

        
    }
}
