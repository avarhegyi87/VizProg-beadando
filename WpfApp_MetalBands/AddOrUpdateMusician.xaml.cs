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
        /// <summary>
        /// Interaction for the window for adding or updating musicians
        /// </summary>
        /// <param name="cn">The context object - necessary to get the MetalBands table 
        /// so that the musician can be added to one or multiple bands</param>
        /// <param name="musician">an optional Musician object - only necessary in case of modifying one already in the database</param>
        public AddOrUpdateMusician(cnMetalBands cn, enMetalBands.enMusician? musician = null) {
            InitializeComponent();
            
            //ordering the bands and inserting them into the listbox
            lbBands.ItemsSource = cn.enMetalBands.OrderBy(x => x.Band_name).ToList();
            
            //if we are modifying the current musician, the program loads their data into the form
            if (musician is not null) {
                tbMusFirstName.Text = musician.First_name;
                tbMusLastName.Text = musician.Last_name;
                tbMusInstruments.Text = musician.Instrument;

                //all bands the musician to modify is already a member of will get selected in the listbox
                foreach (var item in lbBands.Items) {
                    if (musician.MetalBands.Contains(item)) {
                        lbBands.SelectedItems.Add(item);
                    }
                }
            }
        }

        //public properties that the MainWindow will be able to access to update the database once we click OK on this window
        /// <summary>
        /// a public property to store the first name of the musician, for the main window event to access and store it in the database
        /// </summary>
        public string NewFirstName => tbMusFirstName.Text;

        /// <summary>
        /// a public property to store the last name of the musician, for the main window event to access and store it in the database
        /// </summary>
        public string NewLastName => tbMusLastName.Text;

        /// <summary>
        /// a public property to store the the instruments played, for the main window event to access and store it in the database
        /// </summary>
        public string NewInstrument => tbMusInstruments.Text;
        
        /// <summary>
        /// a public property to store the selected bands, for the main window event to access and store it in the database
        /// </summary>
        public List<object> SelBands {
            get {
                //creating an empty object list, then adding each selected listbox item to it
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
