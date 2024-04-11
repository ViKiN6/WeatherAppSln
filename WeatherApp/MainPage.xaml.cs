using System;
using System.Collections.Generic;
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
            public List<WeatherInformation> Weather { get; set; }
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
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await GetLocationAndWeatherData();
        }

        private async Task GetLocationAndWeatherData()
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();
                if (location != null)
                {
                    latitude = location.Latitude;
                    longitude = location.Longitude;
                    await GetWeatherData();
                }
                else
                {
                    City.Text = "Unable to get location.";
                    Temperature.Text = "";
                    WeatherDescriptionLabel.Text = "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                City.Text = "Unable to get location.";
                Temperature.Text = "";
                WeatherDescriptionLabel.Text = "";

            }
        }

        private async Task GetWeatherData()
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                string url = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&appid=5d6f986b199000e8861e011bdd4054e7";

                var response = await client.GetStringAsync(url);

                var weatherData = JsonConvert.DeserializeObject<OpenWeatherData>(response);

                City.Text = weatherData.Name;
                Temperature.Text = $"{weatherData.Main.Temp} °C";
                WeatherDescriptionLabel.Text = weatherData.Weather[0].Description;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch weather data: {ex.Message}");
            }
        }

        private async void LocationBtn_Clicked(object sender, EventArgs e)
        {
            await GetLocationAndWeatherData();
        }

        private double latitude;
        private double longitude;
    }
}
