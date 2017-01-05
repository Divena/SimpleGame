using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;


namespace WpfApplication2
{
    public partial class MarginsDialogBox : Window
    {
        public string answ { get; set; }
        public MarginsDialogBox(string request)
        {
            InitializeComponent();
            Request.Text = request;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            answ = Answ.Text;
        }

    }

}
