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
    /// Klasa z lista prognozy pogody co 3 godziny
    /// </summary>
    class WeatherTable
    {
        /// <summary>
        /// Lista prognoz pogody co 3 godziny
        /// </summary>
        [JsonPropertyName("list")]
        public List<WeatherInfo.FullWeather> List { get; set; }
    }

    /// <summary>
    /// Klasa wszystkich informacji o pogodzie co 3 godziny
    /// </summary>
    class WeatherInfo
    {
        /// <summary>
        /// Klasa z pelnym opisem pogody
        /// </summary>
        public class FullWeather
        {
            /// <summary>
            /// Data pomiaru
            /// </summary>
            [JsonPropertyName("dt_txt")]
            public string DateTxt { get; set; }

            /// <summary>
            /// Zbior danych: temperatura, cisnienie i wilgotnosc
            /// </summary>
            [JsonPropertyName("main")]
            public Main Main { get; set; }

            /// <summary>
            /// Lista danych: status pogody, pelny opis pogody oraz oznaczenie dla ikony pogody
            /// </summary>
            [JsonPropertyName("weather")]
            public List<Weather> Weather { get; set; }

        }

        /// <summary>
        /// Klasa ze zbiorem danych o temperaturze, cisnieniu i wilgotnosci
        /// </summary>
        public class Main
        {
            /// <summary>
            /// Temperatura
            /// </summary>
            [JsonPropertyName("temp")]
            public double Temp { get; set; }

            /// <summary>
            /// Cisnienie
            /// </summary>
            [JsonPropertyName("pressure")]
            public double Pressure { get; set; }

            /// <summary>
            /// WIlgotnosc
            /// </summary>
            [JsonPropertyName("humidity")]
            public double Humidity { get; set; }
        }

        /// <summary>
        /// Klasa z opisem pogody
        /// </summary>
        public class Weather
        {
            /// <summary>
            /// Krotki opis pogody
            /// </summary>
            [JsonPropertyName("main")]
            public string Main { get; set; }

            /// <summary>
            /// Pelny opis pogody
            /// </summary>
            [JsonPropertyName("description")]
            public string Description { get; set; }

            /// <summary>
            /// Oznaczenie dla ikony pogody
            /// </summary>
            [JsonPropertyName("icon")]
            public string Icon { get; set; }
        }
    }

    /// <summary>
    /// Klasa z lista stolic
    /// </summary>
    class CitiesTable
    {
        /// <summary>
        /// Lista stolic
        /// </summary>
        [JsonPropertyName("geonames")]
        public List<City> Geonames { get; set; }
    }

    /// <summary>
    /// Klasa z stolica
    /// </summary>
    class City
    {
        /// <summary>
        /// Stolica
        /// </summary>
        [JsonPropertyName("capital")]
        public string Capital { get; set; }
    }

    /// <summary>
    /// Aplikacja pogodowa ktora wyswietla aktualna pogode (temperature, cisnienie i wilgotnosc) oraz 3 kolejne pomiary w odstepach 3 godzinowych, dla wybranego miasta lub dla podanych koordynatow
    /// </summary>
    public partial class MainWindow : Window
    {

        private Dictionary<string, City> _cities = new Dictionary<string, City>();

        /// <summary>
        /// Konstruktor ktory inicjalizuje aplikacje, pobiera liste miast, aktualizuje liste w combo boxie, ustawia wartosc domyslna, ktora jest pierwsze miasto z liczny i pokazuja aktualna pogode dla tego miasta
        /// </summary>
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
                MessageBox.Show("Wrong value of lat or lng");
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

            try
            {
                CurrentCity.Text = selectedCity;
                UpdateWeather(weatherTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UpdateWeatherXY(double x, double y)
        {
            WeatherTable weatherTable = DownloadWeatherXY(x, y);

            try
            {
                CurrentCity.Text = $"lat: {x:F2}, lng: {y:F2}";
                UpdateWeather(weatherTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UpdateWeather(WeatherTable weatherTable)
        {
            try
            {

                WeatherDt1.Text = weatherTable.List[0].DateTxt;
                img1.Source = ImgAdd(weatherTable.List[0].Weather[0].Icon);
                WeatherStatus1.Text = weatherTable.List[0].Weather[0].Main;
                WeatherTemp1.Text = weatherTable.List[0].Main.Temp.ToString() + "°C";
                WeatherPressure1.Text = weatherTable.List[0].Main.Pressure.ToString() + "hPa";
                WeatherHumidity1.Text = weatherTable.List[0].Main.Humidity.ToString() + "%";

                WeatherDt2.Text = weatherTable.List[1].DateTxt;
                img2.Source = ImgAdd(weatherTable.List[1].Weather[0].Icon);
                WeatherStatus2.Text = weatherTable.List[1].Weather[0].Main;
                WeatherTemp2.Text = weatherTable.List[1].Main.Temp.ToString() + "°C";

                if (weatherTable.List[1].Main.Temp > weatherTable.List[0].Main.Temp)
                {
                    WeatherTemp1Arr.Foreground = Brushes.LimeGreen;
                    WeatherTemp1Arr.Text = "↑";
                }
                else if (weatherTable.List[1].Main.Temp == weatherTable.List[0].Main.Temp)
                    WeatherTemp1Arr.Text = "";
                else
                {
                    WeatherTemp1Arr.Foreground = Brushes.Red;
                    WeatherTemp1Arr.Text = "↓";
                }

                WeatherPressure2.Text = weatherTable.List[1].Main.Pressure.ToString() + "hPa";

                if (weatherTable.List[1].Main.Pressure > weatherTable.List[0].Main.Pressure)
                {
                    WeatherPressure1Arr.Foreground = Brushes.LimeGreen;
                    WeatherPressure1Arr.Text = "↑";
                }
                else if (weatherTable.List[1].Main.Pressure == weatherTable.List[0].Main.Pressure)
                    WeatherPressure1Arr.Text = "";
                else
                {
                    WeatherPressure1Arr.Foreground = Brushes.Red;
                    WeatherPressure1Arr.Text = "↓";
                }

                WeatherHumidity2.Text = weatherTable.List[1].Main.Humidity.ToString() + "%";

                if (weatherTable.List[1].Main.Humidity > weatherTable.List[0].Main.Humidity)
                {
                    WeatherHumidity1Arr.Foreground = Brushes.LimeGreen;
                    WeatherHumidity1Arr.Text = "↑";
                }
                else if (weatherTable.List[1].Main.Humidity == weatherTable.List[0].Main.Humidity)
                    WeatherHumidity1Arr.Text = "";
                else
                {
                    WeatherHumidity1Arr.Foreground = Brushes.Red;
                    WeatherHumidity1Arr.Text = "↓";
                }


                WeatherDt3.Text = weatherTable.List[2].DateTxt;
                img3.Source = ImgAdd(weatherTable.List[1].Weather[0].Icon);
                WeatherStatus3.Text = weatherTable.List[2].Weather[0].Main;
                WeatherTemp3.Text = weatherTable.List[2].Main.Temp.ToString() + "°C";

                if (weatherTable.List[2].Main.Temp > weatherTable.List[1].Main.Temp)
                {
                    WeatherTemp2Arr.Foreground = Brushes.LimeGreen;
                    WeatherTemp2Arr.Text = "↑";
                }
                else if (weatherTable.List[2].Main.Temp == weatherTable.List[1].Main.Temp)
                    WeatherTemp2Arr.Text = "";
                else
                {
                    WeatherTemp2Arr.Foreground = Brushes.Red;
                    WeatherTemp2Arr.Text = "↓";
                }

                WeatherPressure3.Text = weatherTable.List[2].Main.Pressure.ToString() + "hPa";

                if (weatherTable.List[2].Main.Pressure > weatherTable.List[1].Main.Pressure)
                {
                    WeatherPressure2Arr.Foreground = Brushes.LimeGreen;
                    WeatherPressure2Arr.Text = "↑";
                }
                else if (weatherTable.List[2].Main.Pressure == weatherTable.List[1].Main.Pressure)
                    WeatherPressure2Arr.Text = "";
                else
                {
                    WeatherPressure2Arr.Foreground = Brushes.Red;
                    WeatherPressure2Arr.Text = "↓";
                }

                WeatherHumidity3.Text = weatherTable.List[2].Main.Humidity.ToString() + "%";

                if (weatherTable.List[2].Main.Humidity > weatherTable.List[1].Main.Humidity)
                {
                    WeatherHumidity2Arr.Foreground = Brushes.LimeGreen;
                    WeatherHumidity2Arr.Text = "↑";
                }
                else if (weatherTable.List[2].Main.Humidity == weatherTable.List[1].Main.Humidity)
                    WeatherHumidity2Arr.Text = "";
                else
                {
                    WeatherHumidity2Arr.Foreground = Brushes.Red;
                    WeatherHumidity2Arr.Text = "↓";
                }


                WeatherDt4.Text = weatherTable.List[3].DateTxt;
                img4.Source = ImgAdd(weatherTable.List[1].Weather[0].Icon);
                WeatherStatus4.Text = weatherTable.List[3].Weather[0].Main;
                WeatherTemp4.Text = weatherTable.List[3].Main.Temp.ToString() + "°C";

                if (weatherTable.List[3].Main.Temp > weatherTable.List[2].Main.Temp)
                {
                    WeatherTemp3Arr.Foreground = Brushes.LimeGreen;
                    WeatherTemp3Arr.Text = "↑";
                }
                else if (weatherTable.List[3].Main.Temp == weatherTable.List[2].Main.Temp)
                    WeatherTemp3Arr.Text = "";
                else
                {
                    WeatherTemp3Arr.Foreground = Brushes.Red;
                    WeatherTemp3Arr.Text = "↓";
                }

                WeatherPressure4.Text = weatherTable.List[3].Main.Pressure.ToString() + "hPa";

                if (weatherTable.List[3].Main.Pressure > weatherTable.List[2].Main.Pressure)
                {
                    WeatherPressure3Arr.Foreground = Brushes.LimeGreen;
                    WeatherPressure3Arr.Text = "↑";
                }
                else if (weatherTable.List[3].Main.Pressure == weatherTable.List[2].Main.Pressure)
                    WeatherPressure3Arr.Text = "";
                else
                {
                    WeatherPressure3Arr.Foreground = Brushes.Red;
                    WeatherPressure3Arr.Text = "↓";
                }

                WeatherHumidity4.Text = weatherTable.List[3].Main.Humidity.ToString() + "%";

                if (weatherTable.List[3].Main.Humidity > weatherTable.List[2].Main.Humidity)
                {
                    WeatherHumidity3Arr.Foreground = Brushes.LimeGreen;
                    WeatherHumidity3Arr.Text = "↑";
                }
                else if (weatherTable.List[3].Main.Humidity == weatherTable.List[2].Main.Humidity)
                    WeatherHumidity3Arr.Text = "";
                else
                {
                    WeatherHumidity3Arr.Foreground = Brushes.Red;
                    WeatherHumidity3Arr.Text = "↓";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DownloadCitiesData()
        {
            MessageBox.Show("Downloading cities list...");

            WebClient client = new WebClient();
            string json = client.DownloadString("http://api.geonames.org/countryInfoJSON?formatted=true&username=marek572");

            CitiesTable citiesTable = JsonSerializer.Deserialize<CitiesTable>(json);

            try
            {
                foreach (City c in citiesTable.Geonames)
                {
                    if (!_cities.ContainsKey(c.Capital))
                        _cities.Add(c.Capital, c);
                }
                MessageBox.Show("Downloaded!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void UpdateCombobox()
        {
            foreach (string city in _cities.Keys)
            {
                SelectCity.Items.Add(city);
            }

            SelectCity.SelectedIndex = 0;
        }

        private void enterX_GotFocus(object sender, RoutedEventArgs e)
        {
            enterX.Text = "";
        }

        private void enterY_GotFocus(object sender, RoutedEventArgs e)
        {
            enterY.Text = "";
        }

    }
}
