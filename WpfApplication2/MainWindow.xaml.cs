using System;
using System.Windows;
using System.IO;
using System.Diagnostics;


namespace WpfApplication2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Lands land;
        public MainWindow()
        {
            Process.Start("run.bat");
            InitializeComponent();
           DataContext = this;
           land = new Lands();
           Land.Items.Clear();
           Land.ItemsSource = land;
           icon.DataContext = land;
        }
        private void Next_day_Click(object sender, RoutedEventArgs e)
        {            
            var i = Land.SelectedContent;
            land.Day();
            string inf1 = "Ежедневный налог составил: ";
            string inf2 = "";
            int tax = 0;
            foreach (village t in land)
            {
                tax += t.Tax;
                if (t.Robbery && t.Status == 5)
                {
                    inf2 += "Разбойники напали на " + t.Name + Environment.NewLine;
                }
            }
            inf1 += tax.ToString() + " золотых" + Environment.NewLine;
            Info.Text += inf1 + inf2;
     }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if ((village)Land.SelectedItem != null)
            {
                string inf = "Деревня " + ((village)Land.SelectedItem).Name+" уничтожена, сумма награбленного 75 золотых";
                Info.Text += inf;
                
                land.Remove(((village)Land.SelectedItem).Name);
            }
        }
        private void Add_money_Click(object sender, RoutedEventArgs e)
        {
            MarginsDialogBox dig = new MarginsDialogBox("Введите название новой деревни: ");
            dig.ShowDialog();
            if (dig.DialogResult == true) 
            {
                if (dig.answ == "") {
                    MessageBox.Show("Деревня не может быть безымянной");
                }
                else
                {
                    if (land.Add(dig.answ, true))
                    {
                        string inf = "Деревня успешно построена";
                        Info.Text += inf;
                        
                    }
                    else
                    {
                        MessageBox.Show("Недостаточно денег либо деревня с таким именем уже существует =(");
                    }
                }
            }
        }
        private void Add_army_Click(object sender, RoutedEventArgs e)
        {
            MarginsDialogBox dig = new MarginsDialogBox("Введите название деревни ");
            dig.ShowDialog();
            if (dig.DialogResult == true)
            {
                if (dig.answ != "")
                {
                    if (land.Add(dig.answ, false)) 
                    {
                        string inf = "Деревня успешно захвачена";
                        Info.Text += inf;
                        
                    }
                    else
                    {

                        MessageBox.Show("Армии оказалось недостаточно либо деревня с таким именем уже Ваша =(");
                    }
                }
            }
        }
        private void By_army_Click(object sender, RoutedEventArgs e)
        {
            int l;
            MarginsDialogBox dig = new MarginsDialogBox("Введите количество (1воин - 5 золотых): ");
            dig.ShowDialog();
            if (dig.DialogResult == true)
            {
                if (dig.answ != "")
                {
                    if (int.TryParse((dig.answ), out l))
                    {
                        if (land.army(l)) 
                        {
                            string inf = "Воины готовы вам служить";
                            Info.Text += inf;
                          
                        }
                        else
                        {
                            MessageBox.Show("Мало денег");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка ввода");
                    }
                }
            }
        }
      
        private void Smithy_Click(object sender, RoutedEventArgs e)
        {
            land.smithy(((village)Land.SelectedItem).Name);
        }
        private void Sawmill_Click(object sender, RoutedEventArgs e)
        {
            land.sawmill(((village)Land.SelectedItem).Name);
        }
        private void Mill_Click(object sender, RoutedEventArgs e)
        {
            land.mill(((village)Land.SelectedItem).Name);
        }

        private void Protect_Button_Click(object sender, RoutedEventArgs e)
        {
            if (!land.Protect(((village)Land.SelectedItem).Name))
            {
                MessageBox.Show("Армии оказалось недостаточно");
            }
            else
            {
                string inf = "Деревня " + ((village)Land.SelectedItem).Name + " защищена, разбойники повержены!";
                Info.Text += inf;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Myserializer f = new Myserializer();
            var file = new FileInfo(@"Save.bin");
            FileStream fs = file.Open(FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            f.Serialize(fs, land);
            File.WriteAllText(@"Info.txt", Info.Text);
            fs.Close();

        }
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var file = new FileInfo(@"C:\Users\Dina\Documents\Visual Studio 2013\Projects\WpfApplication2\Save.bin");
                FileStream fs = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                Myserializer f = new Myserializer();
                land =(Lands)f.Deserialize(fs);
                fs.Close();
                Land.UpdateLayout();
            }
            catch (FileNotFoundException a)
            {
                MessageBox.Show("Файл не найден", "Ошибка", MessageBoxButton.OK);
            }
        }
        private void Open_inf_Click(object sender, RoutedEventArgs e)
        {
            Window2 dm = new Window2();
            dm.ShowDialog();
        }
    }

   
}
