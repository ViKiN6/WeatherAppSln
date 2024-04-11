using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;

namespace WeatherApp
{
    public partial class MainPage : ContentPage
    {
        public class OpenWeatherData
        {
            public string Name { get; set; }
            public MainData Main { get; set; }
            public WeatherInformation[] Weather { get; set; }
        }

        public class MainData
        {
            public double Temp { get; set; }
        }

        public class WeatherInformation
        {
            public string Description { get; set; }
        }

        public MainPage()
        {
            InitializeComponent();
            GetWeatherForCurrentLocation();
        }

        private async Task GetWeatherData(string location)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                string url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&units=metric&appid=5d6f986b199000e8861e011bdd4054e7";

                var response = await client.GetStringAsync(url);

                var weatherData = JsonConvert.DeserializeObject<OpenWeatherData>(response);

                CityLabel.Text = $"City: {weatherData.Name}";
                TemperatureLabel.Text = $"Temperature: {weatherData.Main.Temp} °C";
                WeatherDescriptionLabel.Text = $"Weather Description: {weatherData.Weather[0].Description}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch weather data: {ex.Message}");
                CityLabel.Text = "Error fetching data";
                TemperatureLabel.Text = "";
                WeatherDescriptionLabel.Text = "";
            }
        }

        private async void GetWeatherButton_Clicked(object sender, EventArgs e)
        {
            string location = LocationEntry.Text;
            if (!string.IsNullOrWhiteSpace(location))
            {
                await GetWeatherData(location);
            }
            else
            {
                await DisplayAlert("Error", "Please enter a city name.", "OK");
            }
        }

        private async void GetWeatherForCurrentLocation()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();
                if (location != null)
                {
                    string latitude = location.Latitude.ToString();
                    string longitude = location.Longitude.ToString();
                    await GetWeatherData($"{latitude},{longitude}");
                }
                else
                {
                    await DisplayAlert("Error", "Unable to get current location.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                await DisplayAlert("Error", "Unable to get current location.", "OK");
            }
        }
    }
}
