using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace projekt_zaliczeniowy_po
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class WeatherTable
        {
            [JsonPropertyName("list")]
            public List<WeatherInfo.FullWeather> List { get; set; }
        }
        class WeatherInfo
        {
            public class FullWeather
            {
                [JsonPropertyName("dt_txt")]
                public string DateTxt { get; set; }

                [JsonPropertyName("main")]
                public Main Main { get; set; }

                [JsonPropertyName("weather")]
                public List<Weather> Weather { get; set; }

            }
            public class Main
            {
                [JsonPropertyName("temp")]
                public double Temp { get; set; }

                [JsonPropertyName("pressure")]
                public double Pressure { get; set; }

                [JsonPropertyName("humidity")]
                public double Humidity { get; set; }
            }

            public class Weather
            {
                [JsonPropertyName("main")]
                public string Main { get; set; }

                [JsonPropertyName("description")]
                public string Description { get; set; }

                [JsonPropertyName("icon")]
                public string Icon { get; set; }
            }
        }

        Dictionary<string, City> Cities = new Dictionary<string, City>();
        class CitiesTable
        {
            [JsonPropertyName("geonames")]
            public List<City> Geonames { get; set; }
        }

        class City
        {
            [JsonPropertyName("capital")]
            public string Capital { get; set; }
        }


        public MainWindow()
        {
            InitializeComponent();
            DownloadCitiesData();
            UpdateCombobox();

            string selectedCity = (string)SelectCity.SelectedItem;
            UpdateWeatherSelected(selectedCity);
        }

        private static BitmapImage ImgAdd(string id)
        {
            var fullFilePath = @$"http://openweathermap.org/img/wn/{id}@2x.png";

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            return bitmap;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(enterX.Text, out double x) && double.TryParse(enterY.Text, out double y))
                UpdateWeatherXY(x, y);
            else
                MessageBox.Show("Wrong value of x or y");
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            string selectedCity = (string)SelectCity.SelectedItem;

            UpdateWeatherSelected(selectedCity);
        }

        private static WeatherTable DownloadWeatherSelected(string selectedCity)
        {
            WebClient client = new WebClient();
            string apiCity = $"http://api.openweathermap.org/data/2.5/forecast?q={selectedCity}&appid=fa1186561a4ddbcc983bb31e06264c44&units=metric";
            string json = client.DownloadString(apiCity);

            WeatherTable weatherTable = JsonSerializer.Deserialize<WeatherTable>(json);
            return weatherTable;
        }

        private static WeatherTable DownloadWeatherXY(double x, double y)
        {
            WebClient client = new WebClient();
            string apiCity = $"http://api.openweathermap.org/data/2.5/forecast?lat={x}&lon={y}&appid=fa1186561a4ddbcc983bb31e06264c44&units=metric";
            string json = client.DownloadString(apiCity);

            WeatherTable weatherTable = JsonSerializer.Deserialize<WeatherTable>(json);
            return weatherTable;
        }

        private void UpdateWeatherSelected(string selectedCity)
        {
            WeatherTable weatherTable = DownloadWeatherSelected(selectedCity);

            CurrentCity.Text = selectedCity;

            WeatherDt1.Text = weatherTable.List[0].DateTxt;
            img1.Source = ImgAdd(weatherTable.List[0].Weather[0].Icon);
            WeatherStatus1.Text = weatherTable.List[0].Weather[0].Main.ToString();
            WeatherTemp1.Text = weatherTable.List[0].Main.Temp.ToString() + "°C";

            WeatherDt2.Text = weatherTable.List[1].DateTxt;
            img2.Source = ImgAdd(weatherTable.List[1].Weather[0].Icon);
            WeatherStatus2.Text = weatherTable.List[1].Weather[0].Main.ToString();
            WeatherTemp2.Text = weatherTable.List[1].Main.Temp.ToString() + "°C";

            WeatherDt3.Text = weatherTable.List[2].DateTxt;
            img3.Source = ImgAdd(weatherTable.List[1].Weather[0].Icon);
            WeatherStatus3.Text = weatherTable.List[2].Weather[0].Main.ToString();
            WeatherTemp3.Text = weatherTable.List[2].Main.Temp.ToString() + "°C";

            WeatherDt4.Text = weatherTable.List[3].DateTxt;
            img4.Source = ImgAdd(weatherTable.List[1].Weather[0].Icon);
            WeatherStatus4.Text = weatherTable.List[3].Weather[0].Main.ToString();
            WeatherTemp4.Text = weatherTable.List[3].Main.Temp.ToString() + "°C";
        }

        private void UpdateWeatherXY(double x, double y)
        {
            WeatherTable weatherTable = DownloadWeatherXY(x,y);

            CurrentCity.Text = $"coords x: {x}, y: {y}";

            WeatherDt1.Text = weatherTable.List[0].DateTxt;
            WeatherStatus1.Text = weatherTable.List[0].Weather[0].Main.ToString();
            WeatherTemp1.Text = weatherTable.List[0].Main.Temp.ToString() + "°C";

            WeatherDt2.Text = weatherTable.List[1].DateTxt;
            WeatherStatus2.Text = weatherTable.List[1].Weather[0].Main.ToString();
            WeatherTemp2.Text = weatherTable.List[1].Main.Temp.ToString() + "°C";

            WeatherDt3.Text = weatherTable.List[2].DateTxt;
            WeatherStatus3.Text = weatherTable.List[2].Weather[0].Main.ToString();
            WeatherTemp3.Text = weatherTable.List[2].Main.Temp.ToString() + "°C";

            WeatherDt4.Text = weatherTable.List[3].DateTxt;
            WeatherStatus4.Text = weatherTable.List[3].Weather[0].Main.ToString();
            WeatherTemp4.Text = weatherTable.List[3].Main.Temp.ToString() + "°C";
        }

        private void DownloadCitiesData()
        {
            MessageBox.Show("Downloading cities list...");

            WebClient client = new WebClient();
            string json = client.DownloadString("http://api.geonames.org/countryInfoJSON?formatted=true&username=marek572");

            CitiesTable citiesTable = JsonSerializer.Deserialize<CitiesTable>(json);

            foreach(City c in citiesTable.Geonames)
            {
                if(!Cities.ContainsKey(c.Capital))
                    Cities.Add(c.Capital, c);
            }

            MessageBox.Show("Downloaded!");
        }

        private void UpdateCombobox()
        {
            foreach(string city in Cities.Keys)
            {
                SelectCity.Items.Add(city);
            }

            SelectCity.SelectedIndex = 0;
        }
    }
}
