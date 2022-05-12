using enMetalBands;
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
        public AddOrUpdateMusician(cnMetalBands cn, enMetalBands.enMusician musician = null) {
            InitializeComponent();
            lbBands.ItemsSource = cn.enMetalBands.OrderBy(x => x.Band_name).ToList();
            if (musician is not null) {
                tbMusFirstName.Text = musician.First_name;
                tbMusLastName.Text = musician.Last_name;
                tbMusInstruments.Text = musician.Instrument;

                foreach (var item in lbBands.Items) {
                    if (musician.MetalBands.Contains(item)) {
                        lbBands.SelectedItems.Add(item);
                    }
                }
            }
        }

        public string NewFirstName => tbMusFirstName.Text;
        public string NewLastName => tbMusLastName.Text;
        public string NewInstrument => tbMusInstruments.Text;
        public List<object> SelBands {
            get {
                List<object> list = new List<object>();
                foreach (var item in lbBands.SelectedItems) {
                    list.Add(item);
                }
                return list;
            }
        }

        private void btMusOK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
    }
}
