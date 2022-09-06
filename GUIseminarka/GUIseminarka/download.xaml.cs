using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.IO;

namespace GUIseminarka
{
    /// <summary>
    /// Interakční logika pro download.xaml
    /// </summary>
    public partial class download : Window
    {
        List<string> dwFiles = new List<string>();

        string path = AppDomain.CurrentDomain.BaseDirectory + "downloads\\";

        public download()
        {
            InitializeComponent();
            final.Clear();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Refresh();
        }

        public void downloadData()
        {
            string address = insertdata.Text;
            string fileName = filename.Text;
            string cesta = @path + fileName;
            using (WebClient client = new WebClient())
            {
                if (filename.Text == string.Empty | insertdata.Text == string.Empty)
                    MessageBox.Show("Zadejte data!");
                else
                {
                    client.DownloadFile(address, cesta);
                    dwFiles.Add(cesta);
                }
            }
            Refresh();
        }
        public void DefaultData()
        {
            List<string> url = new List<string>()
            { "https://s3.amazonaws.com/scschoolfiles/112/j-r-r-tolkien-lord-of-the-rings-01-the-fellowship-of-the-ring-retail-pdf.pdf",
              "https://eg4.nic.in/jnv/DFILES/EBOOKS/IR/Harry-potter-sorcerers-stone.pdf",
              "https://bible21.cz/wp-content/uploads/2010/12/BIBLE21.pdf",
              "https://dl1.cuni.cz/pluginfile.php/507146/mod_resource/content/1/Koran.pdf",
              "https://www.nothuman.net/images/files/discussion/2/1815b71a2e633176b1c509f3a186605b.pdf"
            };
            List<string> names = new List<string>()
            {
                "lotr.pdf","harry.pdf","bible.pdf","koran.pdf", "got.pdf"
            };
            using (WebClient client = new WebClient())
            {
                foreach(string urlPath in url)
                {
                   client.DownloadFile(urlPath, @path + names[url.IndexOf(urlPath)]);
                   dwFiles.Add(@path + names[url.IndexOf(urlPath)]);
                }
            }
        }
        public void Soubory()
        {
            checkboxs.Clear();
            dwFiles.Clear();
            list.Clear();
            checklist.Clear();
            foreach (string file in Directory.EnumerateFiles(path, "*.pdf"))
            {
                dwFiles.Add(file);
            }
        }
        public void transferData()
        {
            foreach(CheckBox a in checklist)
            {
                if(a.IsChecked ?? false)
                {
                    int index = checklist.IndexOf(a);
                    final.Add(checkboxs[index]);
                }
            }
            foreach (string file in final)
            {
                (Application.Current.MainWindow as MainWindow).Data(path + file);
            }
        }

        List<string> checkboxs = new List<string>();

        public void vsechnySoubory()
        {
            foreach (string x in dwFiles)
            {
                string nazvy = x.Remove(0, path.Count());
                checkboxs.Add(nazvy);
            }
            foreach (string x in checkboxs)
            {
                AddTtBox();
                int index = checkboxs.IndexOf(x);
                list[index].Text += x;
            }
        }

        public void stahujdata_Click(object sender, RoutedEventArgs e)
        {
            downloadData();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            transferData();
        }

        public void Refresh()
        {
            canvas.Children.Clear();
            counter = 0;
            top = 0;
            Soubory();
            vsechnySoubory();
        }

        List<string> final = new List<string>();
        int counter = 0;
        int top = 0;
        List<TextBox> list = new List<TextBox>();
        List<CheckBox> checklist = new List<CheckBox>();

        private void AddTtBox()
        {
            bool helper = false;
            TextBox txb = new TextBox();
            CheckBox chb = new CheckBox();
            txb.Width = 90;
            txb.Height = 20;
            if (counter == 0)
            {
                txb.Margin = new Thickness(20, top, 20, 20);
                chb.Margin = new Thickness(120, top, 20, 20);
                canvas.Children.Add(txb);
                canvas.Children.Add(chb);
                list.Add(txb);
                checklist.Add(chb);
                helper = true;
            }
            if (counter > 0 & counter <= 4)
            {
                top = top + 40;
                txb.Margin = new Thickness(20, top, 20, 20);
                chb.Margin = new Thickness(120, top, 20, 20);
                canvas.Children.Add(txb);
                canvas.Children.Add(chb);
                list.Add(txb);
                checklist.Add(chb);
                helper = true;
            }
            if (counter == 5)
            {
                top = 0;
                txb.Margin = new Thickness(170, top, 20, 20);
                chb.Margin = new Thickness(290, top, 20, 20);
                canvas.Children.Add(txb);
                canvas.Children.Add(chb);
                list.Add(txb);
                checklist.Add(chb);
                helper = true;
            }

            if (counter > 5 & counter <= 9)
            {
                top = top + 40;
                txb.Margin = new Thickness(170, top, 20, 20);
                chb.Margin = new Thickness(290, top, 20, 20);
                canvas.Children.Add(txb);
                canvas.Children.Add(chb);
                list.Add(txb);
                checklist.Add(chb);
                helper = true;
            }
            if (helper == true)
                counter++;
        }

        private void _default_Click(object sender, RoutedEventArgs e)
        {
            DefaultData();
            MessageBox.Show("Všechny soubory staženy");
            Refresh();
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach(CheckBox c in checklist)
            {
                c.IsChecked = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Remove();
            Refresh();
        }
        public void Remove()
        {
            foreach (CheckBox a in checklist)
            {
                if (a.IsChecked ?? false)
                {
                    int index = checklist.IndexOf(a);
                    string name = dwFiles[index];
                    File.Delete(name);
                }
            }
        }
    }
}
