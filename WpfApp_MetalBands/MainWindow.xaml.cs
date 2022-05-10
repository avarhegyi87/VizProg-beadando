﻿using System;
using System.Collections.Generic;
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
            //var ds = (from x in context.enMetalBands select x.Band_name).ToList();
            //cbAlbumArtist.ItemsSource = ds;
        }

        private void hideAllGrids() {
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

            var result = context.enMusicians.Include(x => x.MetalBands).OrderBy(x => x.First_name).ToList();

            dgMusicians.Visibility = Visibility.Visible;
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

            dgMusicianEdit.Visibility = Visibility.Visible;
            dgMusicianEdit.ItemsSource = result;
        }

        private void miUpdtAlbum_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();
            grAlbum.Visibility = Visibility.Visible;
            //grAlbum.DataContext = context.enAlbums.Include(x => x.MetalBand).ToList();
            grAlbum.DataContext = context.enMetalBands.Include(x => x.Albums).ToList();
        }

        private void btBack_Click(object sender, RoutedEventArgs e) {
            miListBands_Click(sender, e);
        }

        private void cbBandName_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var band = ((ComboBox)sender).SelectedItem as enMetalBand;
            tbBandName.Text = band.Band_name;
            if (band.GenreName is null) {
                var gn = (from x in context.enGenres where x.Genre_id == band.Genre_id select x.Genre_name).ToList();
                cbGenre.SelectedItem = gn[0];
            } else {
                cbGenre.SelectedItem = band.GenreName;
            }
            tbYoF.Text = band.Date_founding.ToString();
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
            if (blockHandlers) {
                tbAlbumTitle.Text = "";
                tbRelYear.Text = "";
                tbRating.Text = "";
                return;
            }

            enAlbum album = (from x in context.enAlbums where x.Album_title == ((ComboBox)sender).SelectedItem select x).First();
            if (album is null) {
                tbAlbumTitle.Text = "";
                tbRelYear.Text = "";
                tbRating.Text = "";
            } else {
                tbAlbumTitle.Text = album.Album_title;
                if (album.ArtistName is null) {
                    var an = (from x in context.enMetalBands where x.Band_id == album.Band_id select x.Band_id).ToList();
                    cbAlbumTitle.SelectedItem = an[0];
                } else {
                    //cbAlbumArtist.SelectedItem = album.ArtistName;
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

                if (cbBandName.SelectedItem is not enMetalBand mb) {
                    return;
                } else {
                    mb.Band_name = tbBandName.Text;
                    mb.Date_founding = yr;
                    var gid = context.enGenres.Where(x => x.Genre_name == cbGenre.Text).Select(x => x.Genre_id).ToList();
                    mb.Genre_id = gid[0];

                    context.enMetalBands.Update(mb);
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

                var gid = context.enGenres.Where(x => x.Genre_name == cbGenre.Text).Select(x => x.Genre_id).ToList();

                var new_band = new enMetalBand {
                    Band_name = tbBandName.Text,
                    Genre_id = gid[0],
                    Date_founding = yr
                };

                context.enMetalBands.Add(new_band);
                context.SaveChanges();
                MessageBox.Show("Band saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                miListBands_Click(sender, e);
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btDeleteBand_Click(object sender, RoutedEventArgs e) {
            try {
                if (cbBandName.SelectedItem is not enMetalBand mb) {
                    MessageBox.Show("No such band in the database as " + cbBandName.Text,
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                } else {
                    MessageBoxResult mbres = MessageBox.Show("Are you sure you wish to delete the following band and all their albums?\n" +
                    mb.Band_name + " (" + mb.NoOfAlbums + " albums)",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mbres == MessageBoxResult.Yes) {
                        context.enMetalBands.Remove(mb);
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

                var band_id = context.enMetalBands.Where(x => x.Band_name == cbAlbumArtist.Text).Select(x => x.Band_id).FirstOrDefault();
                enAlbum album = (from x in context.enAlbums where x.Album_title == cbAlbumTitle.Text && x.Band_id == band_id select x).First();
                //enAlbum album = context.enAlbums.Where(x => x.Album_id == band_id).Where(x => x.Album_title == cbAlbumTitle.Text).Select(x => x).First();

                if (album is null) {
                    return;
                } else {
                    var new_band_id = context.enMetalBands.Where(x => x.Band_name == tbArtist.Text).Select(x => x.Band_id).FirstOrDefault();
                    album.Album_title = tbAlbumTitle.Text;
                    album.Release_Year = yr;
                    album.Album_rating = rating;
                    album.Band_id = band_id;

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

                var band_id = context.enMetalBands.Where(x => x.Band_name == tbArtist.Text).Select(x => x.Band_id).FirstOrDefault();

                if (band_id > 0) {
                    var new_album = new enAlbum {
                        Album_title = tbAlbumTitle.Text,
                        Band_id = band_id,
                        Release_Year = yr,
                        Album_rating = rating
                    };

                    context.enAlbums.Add(new_album);
                    context.SaveChanges();
                    MessageBox.Show("Album saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    miListAlbums_Click(sender, e);
                }
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btDeleteAlbum_Click(object sender, RoutedEventArgs e) {
            try {
                var band_id = context.enMetalBands.Where(x => x.Band_name == cbAlbumArtist.Text).Select(x => x.Band_id).FirstOrDefault();
                enAlbum album = (from x in context.enAlbums where x.Album_title == cbAlbumTitle.Text && x.Band_id == band_id select x).First();
                if (album is null) {
                    MessageBox.Show("No such album in the database as " + cbAlbumTitle.Text,
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                } else {
                    MessageBoxResult mares = MessageBox.Show("Are you sure you wish to delete the following album?\n" +
                        album.Album_title + " (" + album.Release_Year + " by " + album.ArtistName + ")",
                        "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mares == MessageBoxResult.Yes) {
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

        private void dgMusicianEdit_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) {
            try {

            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgMusicianEdit_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
 
        }

        private void dgMusicianEdit_CurrentCellChanged(object sender, EventArgs e) {
            
        }

        private void dgMusicianEdit_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) {
            try {
                
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
