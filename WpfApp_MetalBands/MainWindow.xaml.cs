using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        public virtual decimal? AvgRating => Albums?.Count == 0 ? 0 : Math.Round((decimal)Albums.Select(p => p.Album_rating).Average(), 2);

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
    //class MusicianData {
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Bands { get; set; }
    //}

    //public static class BovitoHelpers {
    //    public static string OsszesBanda(this enMusician enSzemely) {
    //        var s = "";
    //        foreach (var x in enSzemely.MetalBands) {
    //            s = s + x.Band_name;
    //            //ha nem az ustolsó elem akkor az s változóhoz hozzáad még egy y karaktert. A Last a collection tipusú adatokon értelmezett művelet mint pl. First, Next stb
    //            if (x != enSzemely.MetalBands.Last())
    //                s = s + ", ";
    //        }
    //        return s;
    //    }
    //}

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window {
        cnMetalBands context;
        private bool blockHandlers;

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

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            miListBands_Click(sender, e);
        }

        private void grBand_Loaded(object sender, RoutedEventArgs e) {
            var ds = (from x in context.enGenres select x.Genre_name).ToList();
            cbGenre.ItemsSource = ds;
        }

        private void grAlbum_Loaded(object sender, RoutedEventArgs e) {

        }

        private void hideAllGrids() {
            tbarEditing.Visibility = Visibility.Collapsed;
            dgAll.Visibility = Visibility.Collapsed;
            dgBands.Visibility = Visibility.Collapsed;
            dgAlbums.Visibility = Visibility.Collapsed;
            dgMusicians.Visibility = Visibility.Collapsed;
            dgMusicianEdit.Visibility = Visibility.Collapsed;
            grBand.Visibility = Visibility.Collapsed;
            grAlbum.Visibility = Visibility.Collapsed;
        }

        private void miExit_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void miListBands_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();

            var result = context.enMetalBands.Include(x => x.Genres).Include(x => x.Albums).Include(x => x.Musicians).OrderBy(x => x.Band_name).ToList();

            dgBands.Visibility = Visibility.Visible;
            dgBands.ItemsSource = result;
        }

        private void miListAlbums_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();

            var result = context.enAlbums.Include(x => x.MetalBand).OrderBy(x=> x.MetalBand).ThenBy(x => x.Release_Year).ToList();

            dgAlbums.Visibility = Visibility.Visible;
            dgAlbums.ItemsSource = result;
        }

        private void miListMusicians_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();

            var result = context.enMusicians.Include(x => x.MetalBands).OrderBy(x => x.Last_name).ToList();

            dgMusicians.Visibility = Visibility.Visible;

            //var result = new List<MusicianData>();
            //foreach (var x in context.enMusicians) {
            //    result.Add(new MusicianData() {
            //        FirstName = x.First_name,
            //        LastName = x.Last_name,
            //        Bands = x.OsszesBanda()
            //    });
            //}

            dgMusicians.ItemsSource = result;
        }

        private void miUpdBand_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();
            grBand.Visibility = Visibility.Visible;
            grBand.DataContext = context.enMetalBands.ToList();
        }

        private void miUpdMusician_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();

            var result = context.enMusicians.ToList();

            tbarEditing.Visibility = Visibility.Visible;
            dgMusicianEdit.Visibility = Visibility.Visible;
            dgMusicianEdit.ItemsSource = result;
        }

        private void miUpdtAlbum_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();
            grAlbum.Visibility = Visibility.Visible;
            grAlbum.DataContext = context.enMetalBands.Include(x => x.Albums).ToList();
            tbRelYear.Text = "";
            tbRating.Text = "";
        }

        private void btBack_Click(object sender, RoutedEventArgs e) {
            miListBands_Click(sender, e);
        }

        private void cbBandName_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var metalBand = ((ComboBox)sender).SelectedItem as enMetalBand;
            tbBandName.Text = metalBand.Band_name;
            if (metalBand.GenreName is null) {
                var genreName = (from x in context.enGenres where x.Genre_id == metalBand.Genre_id select x.Genre_name).ToList();
                cbGenre.SelectedItem = genreName[0];
            } else {
                cbGenre.SelectedItem = metalBand.GenreName;
            }
            tbYoF.Text = metalBand.Date_founding.ToString();
        }

        private void cbAlbumArtist_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var band = ((ComboBox)sender).SelectedItem as enMetalBand;
            if (band is not null) {
                tbArtist.Text = band.Band_name;

                var ds = (from x in context.enAlbums where x.Band_id == band.Band_id orderby x.Release_Year select x.Album_title).ToList();
                blockHandlers = true;
                cbAlbumTitle.ItemsSource = ds;
                tbAlbumTitle.Text = "";
                blockHandlers = false;
            }
        }

        private void cbAlbumTitle_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            enAlbum album = (from x in context.enAlbums where x.Album_title == ((ComboBox)sender).SelectedItem select x).First();
            if (blockHandlers || album is null) {
                tbAlbumTitle.Text = "";
                tbRelYear.Text = "";
                tbRating.Text = "";
            } else {
                tbAlbumTitle.Text = album.Album_title;
                if (album.ArtistName is null) {
                    var artistName = (from x in context.enMetalBands where x.Band_id == album.Band_id select x.Band_id).ToList();
                    cbAlbumTitle.SelectedItem = artistName[0];
                }
                tbRelYear.Text = album.Release_Year.ToString();
                tbRating.Text = album.Album_rating.ToString();
            }
        }

        private void btSaveBand_Click(object sender, RoutedEventArgs e) {
            try {
                if (tbBandName.Text.Length == 0) {
                    MessageBox.Show("Enter a band name.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var res = int.TryParse(tbYoF.Text, out int yr);

                if (!res || (res && (yr < 1950 || yr > DateTime.Today.Year))) {
                    MessageBox.Show("Provide a valid date - it should be between 1950 and " + DateTime.Today.Year,
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cbBandName.SelectedItem is not enMetalBand metalBand) {
                    return;
                } else {
                    metalBand.Band_name = tbBandName.Text;
                    metalBand.Date_founding = yr;
                    var genreId = context.enGenres.Where(x => x.Genre_name == cbGenre.Text).Select(x => x.Genre_id).ToList();
                    metalBand.Genre_id = genreId[0];

                    context.enMetalBands.Update(metalBand);
                    context.SaveChanges();
                    MessageBox.Show("Changes saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void btSaveAsNewBand_Click(object sender, RoutedEventArgs e) {
            try {
                if (tbBandName.Text.Length == 0) {
                    MessageBox.Show("Enter a band name.", "Missing entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                } else {
                    var id = (from x in context.enMetalBands
                              where x.Band_name == tbBandName.Text
                              select x).FirstOrDefault();
                    if (id != null) {
                        MessageBox.Show("There is already a band with this name.\n" +
                            "If you wish to update it, press \"Save\" instead of \"Save As New\", " +
                            "otherwise provide a different band name",
                            "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var res = int.TryParse(tbYoF.Text, out int yr);

                if (!res || (res && (yr < 1950 || yr > DateTime.Today.Year))) {
                    MessageBox.Show("Provide a valid date - it should be between 1950 and " + DateTime.Today.Year,
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var genreId = context.enGenres.Where(x => x.Genre_name == cbGenre.Text).Select(x => x.Genre_id).ToList();

                var newBand = new enMetalBand {
                    Band_name = tbBandName.Text,
                    Genre_id = genreId[0],
                    Date_founding = yr
                };

                context.enMetalBands.Add(newBand);
                context.SaveChanges();
                MessageBox.Show("Band saved" + newBand.Band_name, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                miListBands_Click(sender, e);
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btDeleteBand_Click(object sender, RoutedEventArgs e) {
            try {
                if (cbBandName.SelectedItem is not enMetalBand metalBand) {
                    MessageBox.Show("No such band in the database as " + cbBandName.Text,
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                } else {
                    MessageBoxResult bandRes = MessageBox.Show("Are you sure you wish to delete the following band and all their albums?\n" +
                    metalBand.Band_name + " (" + metalBand.NoOfAlbums + " albums)",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (bandRes == MessageBoxResult.Yes) {
                        context.enMetalBands.Remove(metalBand);
                        context.SaveChanges();
                        MessageBox.Show("The band and all their albums have been deleted",
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        miListBands_Click(sender, e);
                    }
                }
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void btSaveAlbum_Click(object sender, RoutedEventArgs e) {
            try {
                if (tbAlbumTitle.Text.Length == 0) {
                    MessageBox.Show("Enter an album title.", "Missing entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var res_yr = int.TryParse(tbRelYear.Text, out int yr);

                if (!res_yr || (res_yr && (yr < 1950 || yr > DateTime.Today.Year + 1))) {
                    MessageBox.Show("Provide a valid date - it should be between 1950 and " + DateTime.Today.Year + 1,
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var res_rating = int.TryParse(tbRating.Text, out int rating);

                if (!res_rating || (res_rating && (rating < 1 || rating > 10))) {
                    MessageBox.Show("Provide a valid rating - between 1.0 and 10.0",
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var bandId = context.enMetalBands.Where(x => x.Band_name == cbAlbumArtist.Text).Select(x => x.Band_id).FirstOrDefault();
                enAlbum album = (from x in context.enAlbums where x.Album_title == cbAlbumTitle.Text && x.Band_id == bandId select x).First();

                if (album is null) {
                    return;
                } else {
                    var new_band_id = context.enMetalBands.Where(x => x.Band_name == tbArtist.Text).Select(x => x.Band_id).FirstOrDefault();
                    album.Album_title = tbAlbumTitle.Text;
                    album.Release_Year = yr;
                    album.Album_rating = rating;
                    album.Band_id = bandId;

                    context.enAlbums.Update(album);
                    context.SaveChanges();
                    MessageBox.Show("Changes saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btSaveAsNewAlbum_Click(object sender, RoutedEventArgs e) {
            try {
                if (tbAlbumTitle.Text.Length == 0) {
                    MessageBox.Show("Enter an album title.", "Missing entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                } else {
                    var id = (from x in context.enAlbums
                              join y in context.enMetalBands on x.Band_id equals y.Band_id
                              where x.Album_title == tbAlbumTitle.Text
                              select x).FirstOrDefault();
                    if (id != null) {
                        MessageBox.Show("This band already has an album with this title.\n" +
                            "If you wish to update it, press \"Save\" instead of \"Save As New\", " +
                            "otherwise provide a different album title",
                            "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                var res_yr = int.TryParse(tbRelYear.Text, out int yr);

                if (!res_yr || (res_yr && (yr < 1950 || yr > DateTime.Today.Year + 1))) {
                    MessageBox.Show("Provide a valid date - it should be between 1950 and " + DateTime.Today.Year + 1,
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var res_rating = int.TryParse(tbRating.Text, out int rating);

                if (!res_rating || (res_rating && (rating < 1 || rating > 10))) {
                    MessageBox.Show("Provide a valid rating - a whole number between 1 and 10",
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var bandId = context.enMetalBands.Where(x => x.Band_name == tbArtist.Text).Select(x => x.Band_id).FirstOrDefault();

                if (bandId > 0) {
                    var newAlbum = new enAlbum {
                        Album_title = tbAlbumTitle.Text,
                        Band_id = bandId,
                        Release_Year = yr,
                        Album_rating = rating
                    };

                    context.enAlbums.Add(newAlbum);
                    context.SaveChanges();
                    MessageBox.Show("Album saved: " + newAlbum.Album_title, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    miListAlbums_Click(sender, e);
                }
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btDeleteAlbum_Click(object sender, RoutedEventArgs e) {
            try {
                var bandId = context.enMetalBands.Where(x => x.Band_name == cbAlbumArtist.Text).Select(x => x.Band_id).FirstOrDefault();
                enAlbum album = (from x in context.enAlbums where x.Album_title == cbAlbumTitle.Text && x.Band_id == bandId select x).First();
                if (album is null) {
                    MessageBox.Show("No such album in the database as " + cbAlbumTitle.Text,
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                } else {
                    MessageBoxResult albumRes = MessageBox.Show("Are you sure you wish to delete the following album?\n" +
                        album.Album_title + " (" + album.Release_Year + " by " + album.ArtistName + ")",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (albumRes == MessageBoxResult.Yes) {
                        context.enAlbums.Remove(album);
                        context.SaveChanges();
                        MessageBox.Show("Album successfully deleted", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        miListAlbums_Click(sender, e);
                    }
                }
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btAddMusician_Click(object sender, RoutedEventArgs e) {
            var wndMus = new AddOrUpdateMusician(context);
            var wndOk = wndMus.ShowDialog();
            if (wndOk == true) {
                var newMusician = new enMusician {
                    First_name = wndMus.NewFirstName,
                    Last_name = wndMus.NewLastName,
                    Instrument = wndMus.NewInstrument
                };

                context.enMusicians.Add(newMusician);
                MessageBox.Show("Added musician: " + newMusician.FullName, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                newMusician.MetalBands = new List<enMetalBand>();
                
                foreach (enMetalBand band in wndMus.SelBands) {
                    newMusician.MetalBands.Add(band);
                    context.SaveChanges();
                    MessageBox.Show(newMusician.FullName + " added to " + band.Band_name + " as a band member",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                context.SaveChanges();
                miUpdMusician_Click(sender, e);
            }
        }

        private void btUpdateMusician_Click(object sender, RoutedEventArgs e) {
            enMusician? musician = ((FrameworkElement)sender).DataContext as enMusician;
            if (musician is not null) {
                var wndMus = new AddOrUpdateMusician(context, musician);
                var wndOk = wndMus.ShowDialog();
                if (wndOk == true) {
                    musician.First_name = wndMus.NewFirstName;
                    musician.Last_name = wndMus.NewLastName;
                    musician.Instrument = wndMus.NewInstrument;

                    foreach (enMetalBand band in wndMus.SelBands) {
                        if (!musician.MetalBands.Contains(band)) {
                            musician.MetalBands.Add(band);
                        }
                    }
                    for (int i = 0; i < musician.MetalBands.Count; i++) {
                        if (!wndMus.SelBands.Contains(musician.MetalBands[i])) {
                            musician.MetalBands.RemoveAt(i);
                            i--;
                        }
                    }

                    context.SaveChanges();
                    MessageBox.Show("Musician saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    miUpdMusician_Click(sender, e);
                }
            } else {
                MessageBox.Show("Select a row to update then push the \"Update\" button.", "Instructions",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btDeleteMusician_Click(object sender, RoutedEventArgs e) {
            enMusician? musician = ((FrameworkElement)sender).DataContext as enMusician;
            if (musician is not null) {
                MessageBoxResult mbRes = MessageBox.Show("Are you sure you wish to delete this musician from the database?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (mbRes == MessageBoxResult.Yes) {
                    context.enMusicians.Remove(musician);
                    context.SaveChanges();
                    MessageBox.Show("The musician has been deleted from the database",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    miUpdMusician_Click(sender, e);
                }
            } else {
                MessageBox.Show("Select a row to delete then push the \"Delete\" button.", "Instructions",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
