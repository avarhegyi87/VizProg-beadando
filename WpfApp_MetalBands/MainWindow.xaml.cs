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
using Microsoft.EntityFrameworkCore;

namespace enMetalBands {
    public partial class enMetalBand {
        public virtual int? NoOfAlbums => Albums?.Count;

        public virtual decimal? AvgRating => Math.Round((decimal)Albums.Select(p => p.Album_rating).Average(), 2);

        public virtual int? NoOfBandMembers => Musicians?.Count;

        public virtual string? GenreName => Genres?.Genre_name;
    }

    public partial class enMusician {
        public virtual string? FullName => First_name + " " + Last_name;

        public virtual string? PlayingInBands => MetalBands?.Aggregate("", (a, b) => a + (a.Length > 0 ? ", " : "") + b.Band_name);
    }

    public partial class enAlbum {
        public virtual string? ArtistName => MetalBand.Band_name;
    }

    public partial class enGenre {
         
    }
}

namespace WpfApp_MetalBands {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        cnMetalBands context;

        public MainWindow() {
            InitializeComponent();
            var wndLogin = new LoginWindow();
            var auth = wndLogin.ShowDialog();
            if (auth == true) {
                if (wndLogin.UserName == "Adam" && wndLogin.Password == "jelszo") {
                    context = new cnMetalBands();
                    return;
                }
                MessageBox.Show("Wrong login credentials.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            } else {
                Application.Current.Shutdown();
            }
        }

        //private void btTest_Click(object sender, RoutedEventArgs e) {
        //    try {
        //        var myBand = new enMetalBand {
        //            Band_name = "Soen",
        //            Date_founding = 2004,
        //            Genre_name = Genres.progressive_metal
        //        };
        //        context.enMetalBands.Add(myBand);
        //        context.SaveChanges();
        //    }
        //    catch (Exception ex) {

        //        throw;
        //    }
            
        //}

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            hideAllGrids();

            var result = context.enMetalBands.Include(x => x.Genres).Include(x => x.Albums).Include(x => x.Musicians).ToList();

            dgBands.Visibility = Visibility.Visible;
            dgBands.ItemsSource = result;
        }

        private void miExit_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void miListBands_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();

            var result = context.enMetalBands.Include(x => x.Genres).Include(x => x.Albums).Include(x => x.Musicians).ToList();

            dgBands.Visibility = Visibility.Visible;
            dgBands.ItemsSource = result;
        }

        private void miListAlbums_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();

            var result = context.enAlbums.Include(x => x.MetalBand).ToList();

            dgAlbums.Visibility = Visibility.Visible;
            dgAlbums.ItemsSource = result;
        }

        private void miListMusicians_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();

            var result = context.enMusicians.Include(x => x.MetalBands).ToList();

            dgMusicians.Visibility = Visibility.Visible;
            dgMusicians.ItemsSource = result;
        }

        private void miUpdBand_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();
            grBand.Visibility = Visibility.Visible;
            grBand.DataContext = context.enMetalBands.ToList();
        }

        private void miUpdMusician_Click(object sender, RoutedEventArgs e) {

        }

        private void cbBandName_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var b = ((ComboBox)sender).SelectedItem as enMetalBand;
            tbBandName.Text = b.Band_name;
            cbGenre.SelectedItem = b.GenreName;
            tbYoF.Text = b.Date_founding.ToString();
        }

        private void btBack_Click(object sender, RoutedEventArgs e) {
            grBand.Visibility = Visibility.Collapsed;
        }

        private void hideAllGrids() {
            dgAll.Visibility = Visibility.Collapsed;
            dgBands.Visibility = Visibility.Collapsed;
            dgAlbums.Visibility = Visibility.Collapsed;
            dgMusicians.Visibility = Visibility.Collapsed;
            grBand.Visibility = Visibility.Collapsed;
        }

        private void miUpdtAlbum_Click(object sender, RoutedEventArgs e) {

        }

        private void btSaveBand_Click(object sender, RoutedEventArgs e) {
            
            if (tbBandName.Text.Length == 0) {
                MessageBox.Show("Enter a band name.");
                return;
            } else {
                //var id = (from x in context.enMetalBands
                //          where x.Band_name == tbBandName.Text
                //          select x).FirstOrDefault();
                //if (id != null) {
                //    MessageBox.Show("There is already a band with this name.");
                //    return;
                //}
            }

            var res = int.TryParse(tbYoF.Text, out int yr);

            if (!res || (res && (yr < 1950 || yr > DateTime.Today.Year))) {
                MessageBox.Show("Provide a valid date - it should be between 1950 and " + DateTime.Today.Year);
                return;
            }

            if (cbBandName.SelectedItem is not enMetalBand mb) {
                return;
            } else {
                mb.Band_name = tbBandName.Text;
                mb.Date_founding = yr;
                var id = context.enGenres.Where(x => x.Genre_name == cbGenre.Text).Select(x => x.Genre_id).ToList();
                //var id = (from x in context.enGenres
                //          where x.Genre_name == cbGenre.Text
                //          select x.Genre_id).ToList();
                mb.Genre_id = id[0];

                context.enMetalBands.Update(mb);
                context.SaveChanges();

            }

            
        }

        private void btSaveAsNewBand_Click(object sender, RoutedEventArgs e) {

        }

        private void cbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }
    }
}
