using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
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
        static void Main(string[] args)
        {

            try
            {
                String dokad = "d:\\wynik.xml"; 
                String skad = "http://api.nbp.pl/api/exchangerates/tables/a?format=json";

                // Download url.
                using (WebClient client = new WebClient())
                {
                    KursyWalutZaDzien kw = new KursyWalutZaDzien();
                    var obiekt = JsonConvert.SerializeObject(kw);
                    var kursywalut = JsonConvert.DeserializeObject<KursyWalutZaDzien>(obiekt);
                    // Dodałem https://msdn.microsoft.com/pl-pl/library/system.net.webclient.encoding(v=vs.110).aspx
                    client.Encoding = System.Text.Encoding.UTF8;

                    string value =  client.DownloadString(skad);

                    obiekt = value.Substring(1, value.Length - 2);
                    kursywalut = JsonConvert.DeserializeObject<KursyWalutZaDzien>(obiekt);

                    Console.WriteLine("Uzyskano obiekt z pobranych danych");
                    Console.WriteLine(kursywalut.table + " / " + kursywalut.effectiveDate);
                    var listaKursow = kursywalut.rates;
                    foreach (var kurs in listaKursow)
                    {
                        Console.WriteLine(kurs.currency + " - " + kurs.mid);
                    }

                    String dane = "{ 'dane': " + value.Substring(1, value.Length - 2) + "}";
                    JObject o = JObject.Parse(dane);

                    var listakursow = from c in o["dane"]["rates"] select (String)c["currency"] + "/" + (String)c["mid"];
                    foreach (var item in listakursow)
                    {
                        Console.WriteLine(item);
                    }

                    File.AppendAllText(dokad, string.Format("--- {0} ---\n", DateTime.Now) + value);

                    // Asynchroniczny dostęp
                    client.DownloadDataCompleted += DownloadDataCompleted;
                    client.DownloadDataAsync(new Uri(skad));
                    // client.CancelAsync();
                }
            }
            finally
            {
                Console.WriteLine("[Done]");
                Console.ReadLine();
            }
        }
        static void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("-> Przerwano pobieranie");
            }
            var raw = e.Result; // Zamiast tego zapisu -> byte[] raw = e.Result; może być krótszy
            var pobrano =  System.Text.Encoding.UTF8.GetString(raw, 0, raw.Length);
            var obiekt = pobrano.Substring(1, pobrano.Length - 2);
            var kursywalut = JsonConvert.DeserializeObject<KursyWalutZaDzien>(obiekt);
            Console.WriteLine("Uzyskano obiekt z pobranych danych");
            Console.WriteLine(kursywalut.table + " / " + kursywalut.effectiveDate);
            var listaKursow = kursywalut.rates;
            foreach (var kurs in listaKursow)
            {
                Console.WriteLine("===> " +kurs.currency + " - " + kurs.mid);
            }

        }
    }
}
