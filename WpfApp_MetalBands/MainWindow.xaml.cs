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
using System.Windows.Navigation;
using System.Windows.Shapes;
using enMetalBands;

namespace WpfApp_MetalBands {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        enMetalBands.cnMetalBands context;

        public MainWindow() {
            InitializeComponent();
            var wndLogin = new LoginWindow();
            var auth = wndLogin.ShowDialog();
            if (auth == true) {
                if (wndLogin.UserName == "Adam" && wndLogin.Password == "jelszo") {
                    context = new enMetalBands.cnMetalBands();
                    return;
                }
                MessageBox.Show("Wrong login credentials.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            } else {
                Application.Current.Shutdown();
            }
        }

        private void btTest_Click(object sender, RoutedEventArgs e) {
            try {
                var myBand = new enMetalBand {
                    Band_name = "Soen",
                    Date_founding = 2004,
                    Genre_name = Genres.progressive_metal
                };
                context.enMetalBands.Add(myBand);
                context.SaveChanges();
            }
            catch (Exception ex) {

                throw;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            var result = (from x in context.enMetalBands
                          select new {x.Band_name, 
                              x.Genre_name, 
                              x.Date_founding}).ToList();
            dgBands.ItemsSource = result;
        }
    }
}
