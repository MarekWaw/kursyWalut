using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KursyWalut_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public class KursyWalutZaDzien
    {
        public String table { get; set; }
        public String no { get; set; }
        public String effectiveDate { get; set; }
        public Rate[] rates { get; set; }
    };
    public class Rate
    {
        public String currency { get; set; }
        public String code { get; set; }
        public Decimal mid { get; set; }
    };
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            String skad = "http://api.nbp.pl/api/exchangerates/tables/a?format=json";
            var client = new HttpClient();
            var resp = await client.GetAsync(new Uri(skad));
            var danePobrane = await resp.Content.ReadAsStringAsync();
            var obiekt = danePobrane.Substring(1, danePobrane.Length - 2);
            var kursywalut = JsonConvert.DeserializeObject<KursyWalutZaDzien>(obiekt);
        }
    }
}
