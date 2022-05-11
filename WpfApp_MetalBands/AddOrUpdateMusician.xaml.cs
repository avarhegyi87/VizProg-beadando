using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp_MetalBands {
    /// <summary>
    /// Interaction logic for AddOrUpdateMusician.xaml
    /// </summary>
    public partial class AddOrUpdateMusician : Window {
        public AddOrUpdateMusician(enMetalBands.enMusician musician = null) {
            InitializeComponent();
            if (musician is not null) {
                tbMusFirstName.Text = musician.First_name;
                tbMusLastName.Text = musician.Last_name;
                tbMusInstruments.Text = musician.Instrument;
            }
        }

        public string NewFirstName => tbMusFirstName.Text;
        public string NewLastName => tbMusLastName.Text;
        public string NewInstrument => tbMusInstruments.Text;

        private void btMusOK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}
