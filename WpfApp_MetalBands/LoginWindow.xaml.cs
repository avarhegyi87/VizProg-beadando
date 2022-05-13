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
        cnMetalBands? cn;

        public LoginWindow() {
            // create a new instance of the context if the password 
            cn = new cnMetalBands();

            InitializeComponent();
        }
        public string UserName => tbName.Text;
        public string Password => pbPassword.Password;

        private void btOK_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        /// <summary>
        /// Register with a username and a password, encode the password and store the credentials in the database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btRegister_Click(object sender, RoutedEventArgs e) {
            var wndReg = new RegWindow();
            var auth = wndReg.ShowDialog();
            if (auth == true) {
                // take the username from the Login window, and see if there is any user with this name
                var userCount = (from x in cn.enUsers where x.UserName == wndReg.NewUserName select x).ToList();

                // if the user name already exists, abort the registration process.
                if (userCount.Count != 0) {
                    MessageBox.Show("There is already such a user name registered - please, choose a different one.",
                        "User name already taken", MessageBoxButton.OK, MessageBoxImage.Warning);
                    cn = null;
                    return;
                }
                // create a new instance of the HashingOptions class
                HashingOptions opt = new HashingOptions();
                IOptions<HashingOptions> options = Options.Create(opt);

                // create a new instance of the PasswordHasher class, passing the options parameter
                PasswordHasher hasher = new PasswordHasher(options);

                // get the password hash from the PasswordHasher object
                string codedPwd = hasher.Hash(wndReg.NewPassword);

                // take the hashed password and the original password, hash the latter, and compare if they're the same (1st bool)
                // also, check if the options parameter has changed, in which the input needs updating (2nd bool)
                (bool, bool) checkPwd = hasher.Check(codedPwd, wndReg.NewPassword);

                if (!checkPwd.Item1) {
                    MessageBox.Show("The encoding of the password was unsuccessful.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                } else if (checkPwd.Item2) {
                    MessageBox.Show("The password hashing options need updating", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                // add the credentials to the Users database
                var newUser = new enUser {
                    UserName = wndReg.NewUserName,
                    Password = codedPwd
                };
                cn.enUsers.Add(newUser);
                cn.SaveChanges();
                MessageBox.Show("Successful registration", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }

    public sealed class HashingOptions {
        /// <summary>
        /// The code will get encoded as many times as in the Iterations property
        /// </summary>
        public int Iterations { get; set; } = 10000;
    }

    /// <summary>
    /// Password hasher class from https://medium.com/dealeron-dev/storing-passwords-in-net-core-3de29a3da4d2
    /// </summary>
    public sealed class PasswordHasher : IPasswordHasher {
        /// <summary>
        /// The byte size of the randomised salt that will be added to the password hash
        /// </summary>
        private const int SALTSIZE = 16;
        /// <summary>
        /// The byte size of the hash generated from the encoded password
        /// </summary>
        private const int KEYSIZE = 32;

        public PasswordHasher(IOptions<HashingOptions> options) {
            Options = options.Value;
        }

        private HashingOptions Options { get; }

        /// <summary>
        /// Takes the password, and runs an SHA256 algorithm on it, 
        /// completed with random salt, as many times as in the Iterations property
        /// </summary>
        /// <param name="password">The unencoded password, as string</param>
        /// <returns>The hash string, consisting of: Iterations.Salt.Key
        /// E.g., 10000.4QtTb7jPc272uy8MmoWBkw==.CNTvz9T6kHq2/Bo7K7MdJfRvgItamXGR3lUtXL1IgrI=</returns>
        public string Hash(string password) {
            using (var algorithm = new Rfc2898DeriveBytes(
                password, SALTSIZE, Options.Iterations, HashAlgorithmName.SHA256)) {
                var key = Convert.ToBase64String(algorithm.GetBytes(KEYSIZE));
                var salt = Convert.ToBase64String(algorithm.Salt);

                // return the Iterations number, the encoded salt and encoded key
                // such as: 10000.4QtTb7jPc272uy8MmoWBkw==.CNTvz9T6kHq2/Bo7K7MdJfRvgItamXGR3lUtXL1IgrI=
                return $"{Options.Iterations}.{salt}.{key}";
            }
        }

        /// <summary>
        /// Verifies if the encoding was correct (by encoding the password again and comparing it with the hash
        /// as well as checking if the options parameter needs updating
        /// </summary>
        /// <param name="hash">The hash consisting of the iterations, salt and key hash</param>
        /// <param name="password">The original password as string</param>
        /// <returns>1st bool: encoded password matches with the already encoded version, meaning successful encoding. 
        /// 2nd bool: the received iteration number is not the same as in the store version, 
        /// they keys sequences will not match, the code needs upgrading.</returns>
        /// <exception cref="FormatException"></exception>
        public (bool Verified, bool NeedsUpgrade) Check(string hash, string password) {
            // split the hash into 3 parts by the dots
            var parts = hash.Split('.', 3);

            // if the list does not contain 3 parts, the format is incorrect, raise exception
            if (parts.Length != 3) {
                throw new FormatException("Unexpected hash format. " +
                    "Should be formatted as `{iterations}.{salt}.{hash}`");
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            // if new iteration number provided in the hash parameter, code needs updating
            var needsUpgrade = iterations != Options.Iterations;

            // encode the raw password, and compare it with the already encoded key
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
