using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KursyWalut_WindowsForms
{
    public partial class Form1 : Form
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
        static public Rate[] listaKursow;
        public Form1()
        {
            InitializeComponent();
            listView1.Clear();
            listView1.Columns.Add("Kod");
            listView1.Columns.Add("Nazwa");
            listView1.Columns.Add("Przelicznik");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {


                String skad = "http://api.nbp.pl/api/exchangerates/tables/a?format=json";
                using (WebClient client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    client.DownloadDataCompleted += DownloadDataCompleted;
                    client.DownloadDataAsync(new Uri(skad));
                }
            }
            finally
            {
                Console.WriteLine("[Done]");
            }
        }
        static void DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Console.WriteLine("-> Przerwano pobieranie");
            }
            var raw = e.Result; // Zamiast tego zapisu -> byte[] raw = e.Result; może być krótszy
            var pobrano = System.Text.Encoding.UTF8.GetString(raw, 0, raw.Length);
            var obiekt = pobrano.Substring(1, pobrano.Length - 2);
            var kursywalut = JsonConvert.DeserializeObject<KursyWalutZaDzien>(obiekt);
            listaKursow = kursywalut.rates;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            foreach (var kurs in listaKursow)
            {
                ListViewItem lvi = new ListViewItem(kurs.code);
                lvi.SubItems.AddRange(new String[] { kurs.currency, kurs.mid.ToString() });
                listView1.Items.Add(lvi);
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            String skad = "http://api.nbp.pl/api/exchangerates/tables/a?format=json";
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                String danePobrane = await client.DownloadStringTaskAsync(skad);
                var obiekt = danePobrane.Substring(1, danePobrane.Length - 2);
                var kursywalut = JsonConvert.DeserializeObject<KursyWalutZaDzien>(obiekt);
                listaKursow = kursywalut.rates;
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                foreach (var kurs in listaKursow)
                {
                    ListViewItem lvi = new ListViewItem(kurs.code);
                    lvi.SubItems.AddRange(new String[] { kurs.currency, kurs.mid.ToString() });
                    listView1.Items.Add(lvi);
                }
            }
            pictureBox1.Visible = false;
        }
    }
}
