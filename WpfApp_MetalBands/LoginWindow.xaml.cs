using enMetalBands;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window {
        public LoginWindow() {
            InitializeComponent();
        }
        public string UserName => tbName.Text;
        public string Password => pbPassword.Password;

        private void btOK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void btRegister_Click(object sender, RoutedEventArgs e) {
            var wndReg = new RegWindow();
            var auth = wndReg.ShowDialog();
            if (auth == true) {
                //var opt = new HashingOptions();
                //PasswordHasher hasher = new PasswordHasher();
                //enUser user = new enUser {
                //    UserName = wndReg.NewUserName,
                    
                //};
            }
        }
    }

    public sealed class HashingOptions {
        public int Iterations { get; set; } = 10000;
    }

    public sealed class PasswordHasher : IPasswordHasher {
        private const int SALTSIZE = 16;
        private const int KEYSIZE = 32;

        public PasswordHasher(IOptions<HashingOptions> options) {
            Options = options.Value;
        }

        private HashingOptions Options { get; }

        public string Hash(string password) {
            using (var algorithm = new Rfc2898DeriveBytes(
                password, SALTSIZE, Options.Iterations, HashAlgorithmName.SHA256)) {
                var key = Convert.ToBase64String(algorithm.GetBytes(KEYSIZE));
                var salt = Convert.ToBase64String(algorithm.GetBytes(SALTSIZE));

                return $"{Options.Iterations}.{salt}.{key}";
            }
        }

        public (bool Verified, bool NeedsUpgrade) Check(string hash, string password) {
            var parts = hash.Split('.', 3);

            if (parts.Length != 3) {
                throw new FormatException("Unexpected hash format. " +
                    "Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            var needsUpgrade = iterations != Options.Iterations;

            using (var algorithm = new Rfc2898DeriveBytes(
                password, salt, iterations, HashAlgorithmName.SHA256)) {
                {
                    var keyToCheck = algorithm.GetBytes(KEYSIZE);
                    var verified = keyToCheck.SequenceEqual(key);

                    return (verified, needsUpgrade);
                }

            }
        }
    }
}
