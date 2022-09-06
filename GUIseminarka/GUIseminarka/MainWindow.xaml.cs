using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;

namespace GUIseminarka
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Listy do kterých se ukládají časy aby se s nimi mohlo dál počítat
        List<double> normalAvgTime = new List<double>();
        List<double> asyncAvgTime = new List<double>();
        List<double> paraAvgTime = new List<double>();
        List<string> output = new List<string>();
        int counter = 0;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Owner = this;
        }

        private void download_Click_1(object sender, RoutedEventArgs e)
        {
            output.Clear();
            download dl = new download();
            dl.Show();
        }

        public void Data(string file)
        {
            output.Add(file);
        }

        public void ReadDataSync() 
        {
            foreach (string file in output)
            {
                string text = Reader(file);
                ReportInfo(text);
            }
        }

        private string Reader(string path)
        {
            StringBuilder stringReader = new StringBuilder();
            PdfReader reader = new PdfReader(path);

            for (int page = 1; page <= reader.NumberOfPages; page++) 
            {
                stringReader.Append(PdfTextExtractor.GetTextFromPage(reader, page));
            }
            reader.Close();
            string text = stringReader.ToString();

            return text;
        }

        private void ReportInfo(string text) 
        {
            int progressBar = 100 / output.Count;
            progress.Value = progress.Value + progressBar;
            string name = output[counter].Remove(0, (AppDomain.CurrentDomain.BaseDirectory + "downloads\\").Count());
            textblock.Text += ($"{name} obsahuje: {text.Length} znaků.{Environment.NewLine}");
            counter = counter + 1;
        }

        private void start_Click(object sender, RoutedEventArgs e) 
        {
            counter = 0;
            progress.Value = 0;
            if (output.Count == 0)
            {
                MessageBox.Show("Nenašla se žádná data", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            textblock.Text = string.Empty;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            ReadDataSync();
            progress.Value = 100;

            watch.Stop();
            double elm = watch.ElapsedMilliseconds;
            textblock.Text += ($"Celkový čas procesu: {elm}ms");
            times.Text += ($"Normal: {elm}{Environment.NewLine}");

            normalAvgTime.Add(elm);
        }

        private async void asyncbt_Click(object sender, RoutedEventArgs e) 
        {
            counter = 0;
            progress.Value = 0;
            if (output.Count == 0)
            {
                MessageBox.Show("Nenašla se žádná data", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            textblock.Text = string.Empty;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            await ReadDataASync();
            progress.Value = 100;

            watch.Stop();
            double elm = watch.ElapsedMilliseconds;
            textblock.Text += ($"celkový čas procesu: {elm}ms");
            times.Text += ($"Async: {elm}{Environment.NewLine}");
            asyncAvgTime.Add(elm);
        }

        public async Task ReadDataASync() 
        {
            foreach (string file in output)
            {
                string results = await Task.Run(() => Reader(file));
                ReportInfo(results);
            }
        }

        public void Compare() 
        {
            double asyncNormal = 0;
            double paraNormal = 0;
            double paraAsync = 0;

            if (normalAvgTime.Count > 0 && asyncAvgTime.Count > 0)
            {
                asyncNormal = normalAvgTime.Average() / asyncAvgTime.Average();
                asyncNormal = Math.Round(asyncNormal, 3);
                porovnani.Text += ($"Asynchroní metoda je {asyncNormal}x rychlejší než normální{Environment.NewLine}");
            }
            if (paraAvgTime.Count > 0 && normalAvgTime.Count > 0)
            {
                paraNormal = normalAvgTime.Average() / paraAvgTime.Average();
                paraNormal = Math.Round(paraNormal, 3);
                porovnani.Text += ($"Paralelní metoda je {paraNormal}x rychlejší než normální{Environment.NewLine}");
            }
            if (paraAvgTime.Count > 0 && asyncAvgTime.Count > 0)
            {
                paraAsync = asyncAvgTime.Average() / paraAvgTime.Average();
                paraAsync = Math.Round(paraAsync, 3);
                porovnani.Text += ($"Paralelní metoda je {paraAsync}x rychlejší než asynchroní{Environment.NewLine}");
            }
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            textblock.Text = string.Empty;
            List<string> paths = output;
            string path = AppDomain.CurrentDomain.BaseDirectory + "downloads\\";

            foreach (string file in Directory.EnumerateFiles(path, "*.pdf"))
            {
                File.Delete(file);
            }
            textblock.Text += "Všechny soubory smazány";
        }

        private void CompareMethods_Click(object sender, RoutedEventArgs e)
        {
            porovnani.Text = string.Empty;
            Compare();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
