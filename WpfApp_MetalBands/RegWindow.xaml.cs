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
    /// Interaction logic for RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window {
        public RegWindow() {
            InitializeComponent();
        }
        public string NewUserName => tbNewUserName.Text;
        public string NewPassword => pbNewPassword.Password;
        public string ConfirmedPassword => pbConfirmPassword.Password;

        private void btSendReg_Click(object sender, RoutedEventArgs e) {
            if (NewPassword != ConfirmedPassword) {
                MessageBox.Show("The confirmed password is different from the password!",
                    "INVALID ENTRY", MessageBoxButton.OK, MessageBoxImage.Warning);
                pbNewPassword.Clear();
                pbConfirmPassword.Clear();
                return;
            } else {
                DialogResult = true;
            }
        }
    }
}
