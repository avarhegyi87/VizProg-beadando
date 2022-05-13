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
using Microsoft.Extensions.Options;

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
    /* The below commented code blocks are from the project of the professor of this lab.
     * The code can be found at https://github.com/NissinIT/Telefonszamok_DAL_Konzol.
     * The aim was to test if this solution works under MS EntityFrameWorkCore.
     * Under EFC 6, it works, I leave the code here for testing 
     * (data binding will need to be amended on the MainWindow.xaml, for grMusicians.
     * An additional such code block can be found below at the miListMusicians_Click event */

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
        cnMetalBands? context;
        private bool blockHandlers;

        /// <summary>
        /// The login window receving inputs from LoginWindow. 
        /// There is an option to register a new user, in which case a new window opens up. 
        /// To see the underlying code, go to LoginWindow.xaml.cs, which receives inputs from RegWindow.xaml.cs.
        /// </summary>
        public MainWindow() {
            InitializeComponent();
            // initialise a LoginWindow object and show it.
            var wndLogin = new LoginWindow();
            var auth = wndLogin.ShowDialog();
            if (auth == true) {
                // create a new instance of the HashingOptions class
                HashingOptions opt = new HashingOptions();
                IOptions<HashingOptions> options = Options.Create(opt);

                // create a new instance of the PasswordHasher class, passing the options parameter
                PasswordHasher hasher = new PasswordHasher(options);

                // create a context for the database
                context = new cnMetalBands();

                // take the username from the Login window, and see if there is any user with this name
                var userCount = (from x in context.enUsers where x.UserName == wndLogin.UserName select x).ToList();
                
                // if the user name exists, encode the entered password
                // and compare the received hash with the one store in the database
                if (userCount.Count != 0) {
                    enUser user = userCount[0];
                    (bool, bool) checkPwd = hasher.Check(user.Password, wndLogin.Password);

                    // if the first bool (the password hash) is true, return, with the context and the app open.
                    if (checkPwd.Item1) {
                        return;
                    }
                }

                // we get to this point only if the credentials are wrong, meaning
                // the user name does not exist or checkPwd.Item1 is false (the password hash is incorrect)
                context = null;
                MessageBox.Show("Wrong login credentials.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            } else {
                Application.Current.Shutdown();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            // once the main window loads, jump to the datagrid showing the bands and aggregate data from other tables.
            miListBands_Click(sender, e);
        }

        private void grBand_Loaded(object sender, RoutedEventArgs e) {
            // load the genre names into the Genre combobox once a grid with the details of a band are displayed
            var ds = (from x in context.enGenres select x.Genre_name).ToList();
            cbGenre.ItemsSource = ds;
        }

        private void grAlbum_Loaded(object sender, RoutedEventArgs e) {
            // nothing here, kept it in case of later development
        }

        /// <summary>
        /// Hides all the taskbars (this one is only visible when updating musicians), datagrids and grids. 
        /// It runs in the beginning of loading any of these objects. 
        /// Each one is hidden here, then in each event the object in the event is changed back to visible.
        /// </summary>
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

            // to list the bands and their details from other tables, join and load them into the datagrid
            var result = context.enMetalBands.Include(x => x.Genres).Include(x => x.Albums).Include(x => x.Musicians).OrderBy(x => x.Band_name).ToList();

            dgBands.Visibility = Visibility.Visible;
            dgBands.ItemsSource = result;
        }

        private void miListAlbums_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();

            // to list the albums and their details, join them with the MetalBands table,
            // order by band name and release year and load them into the datagrid
            var result = context.enAlbums.Include(x => x.MetalBand).OrderBy(x=> x.MetalBand).ThenBy(x => x.Release_Year).ToList();

            dgAlbums.Visibility = Visibility.Visible;
            dgAlbums.ItemsSource = result;
        }

        private void miListMusicians_Click(object sender, RoutedEventArgs e) {
            hideAllGrids();

            // to list the musicians and their details, join them with the MetalBands table,
            // order by last name and load them into the datagrid
            var result = context.enMusicians.Include(x => x.MetalBands).OrderBy(x => x.Last_name).ToList();

            dgMusicians.Visibility = Visibility.Visible;

            /* The below commented code blocks are from the project of the professor of this lab.
            * The code can be found at https://github.com/NissinIT/Telefonszamok_DAL_Konzol.
            * The aim was to test if this solution works under MS EntityFrameWorkCore.
            * Under EFC 6, it works, I leave the code here for testing 
            * (data binding will need to be amended on the MainWindow.xaml, for grMusicians.
            * An additional code block can be found above the MainWindow class. */

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
            // making only the Band grid visible and loading the MetalBands table into it.
            // In this grid, a single band can be selected and modified, or a new one can be added.
            hideAllGrids();
            grBand.Visibility = Visibility.Visible;
            grBand.DataContext = context.enMetalBands.ToList();
        }

        private void miUpdMusician_Click(object sender, RoutedEventArgs e) {
            // making only the MusicianEdit datagrid visible and loading the Musicians table into it.
            // In this datagrid, there is a button on the taskbar to add a new musician,
            // or buttons in each row of the datagrid to modify or delete that musician.
            hideAllGrids();

            var result = context.enMusicians.OrderBy(x => x.Last_name).ToList();

            tbarEditing.Visibility = Visibility.Visible;
            dgMusicianEdit.Visibility = Visibility.Visible;
            dgMusicianEdit.ItemsSource = result;
        }

        private void miUpdtAlbum_Click(object sender, RoutedEventArgs e) {
            // making only the Album grid visible and loading the MetalBands table joined with the Albums table into it.
            // In this grid, a single album of an artist can be selected and modified, or a new one can be added.
            hideAllGrids();
            grAlbum.Visibility = Visibility.Visible;
            grAlbum.DataContext = context.enMetalBands.Include(x => x.Albums).OrderBy(x => x.Band_name).ToList();

            // clear the release year and rating boxes - they will be filled, once an artist and an album is selected
            tbRelYear.Text = "";
            tbRating.Text = "";
        }

        private void btBack_Click(object sender, RoutedEventArgs e) {
            // the Artist and Album update both have Back buttons calling this same event,
            // it raises the event selecting the band list menu item, listing all the bands and aggregated data from other tables
            miListBands_Click(sender, e);
        }

        private void cbBandName_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // load the selected band into a MetalBand class object
            var metalBand = ((ComboBox)sender).SelectedItem as enMetalBand;
            
            // put the band name into the text box next to it so that it can be modified if the user wants to
            tbBandName.Text = metalBand.Band_name;
            
            // this event might be called when a band has just been added, but the genre get property is not yet loading,
            // in which case it needs to be looked up from the database with LINQ.
            // Make the genre name selected in the genre combobox
            if (metalBand.GenreName is null) {
                var genreName = (from x in context.enGenres where x.Genre_id == metalBand.Genre_id select x.Genre_name).ToList();
                cbGenre.SelectedItem = genreName[0];
            } else {
                cbGenre.SelectedItem = metalBand.GenreName;
            }
            
            // write the year of founding into its textbox
            tbYoF.Text = metalBand.Date_founding.ToString();
        }

        private void cbAlbumArtist_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // On the grid where we modify/delete an album, first we select the artist.
            // Once an artist is selected from the combobox, this event fires, and
            // it looks up all the albums made by this band,
            // set an event blocker to true (to immediately quit from the album combobox selectionchanged event),
            // writes the albums into the combobox, then re-enables events by setting the blocker handle false.
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
            // if an album is already selected, write the details (artist, title, release year, rating) into the textboxes,
            // otherwise if no album object is found, or this event was fired from the artist selection event, clear the textboxes
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

        /// <summary>
        /// This event fires, when a band is modified and then the "Update" button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveBand_Click(object sender, RoutedEventArgs e) {
            try {
                // check if a band name was provided
                if (tbBandName.Text.Trim().Length == 0) {
                    MessageBox.Show("Enter a band name.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // parse the release year into an int (to check if the proper format was given)
                var res = int.TryParse(tbYoF.Text, out int yr);

                // if the release year could not be parsed into an int,
                // or it could, but the year is earlier than 1950 (not many metal bands before that), or later than this year, abort
                if (!res || (res && (yr < 1950 || yr > DateTime.Today.Year))) {
                    MessageBox.Show("Provide a valid date - it should be between 1950 and " + DateTime.Today.Year,
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cbBandName.SelectedItem is not enMetalBand metalBand) {
                    return;
                } else {
                    // find the genre id, and save all details into the MetalBand properties
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

        /// <summary>
        /// This event fires when the "Save As New" button is clicked on a metal band update grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveAsNewBand_Click(object sender, RoutedEventArgs e) {
            try {
                // if the band name is empty, abort
                if (tbBandName.Text.Trim().Length == 0) {
                    MessageBox.Show("Enter a band name.", "Missing entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                } else {
                    // check if there is already a band with the same name, and abort if yes,
                    // we don't want bands having to go to court over copyright debates
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

                // parse the release year into an int (to check if the proper format was given)
                var res = int.TryParse(tbYoF.Text, out int yr);

                // if the release year could not be parsed into an int,
                // or it could, but the year is earlier than 1950 (not many metal bands before that), or later than this year, abort
                if (!res || (res && (yr < 1950 || yr > DateTime.Today.Year))) {
                    MessageBox.Show("Provide a valid date - it should be between 1950 and " + DateTime.Today.Year,
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // get the genre id based on the name selected in the combobox
                var genreId = context.enGenres.Where(x => x.Genre_name == cbGenre.Text).Select(x => x.Genre_id).ToList();

                // save the properties in a var object
                var newBand = new enMetalBand {
                    Band_name = tbBandName.Text,
                    Genre_id = genreId[0],
                    Date_founding = yr
                };

                // add the object to the database
                context.enMetalBands.Add(newBand);
                context.SaveChanges();
                MessageBox.Show("Band saved" + newBand.Band_name, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                miListBands_Click(sender, e);
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// This event fires when the details of a band are displayed, and the Delete button is pushed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDeleteBand_Click(object sender, RoutedEventArgs e) {
            try {
                if (cbBandName.SelectedItem is not enMetalBand metalBand) {
                    // in case such a band doesn't exist - the shouldn't get here as the band is in combobox,
                    // but I've left it here in case a later modification makes it possible.
                    MessageBox.Show("No such band in the database as " + cbBandName.Text,
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                } else {
                    // confirm if deletion is intended, then remove the band from the database (AND ALL THEIR ALBUMS with cascade)
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

        /// <summary>
        /// This event fires, when an album is modified and then the "Update" button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveAlbum_Click(object sender, RoutedEventArgs e) {
            try {
                // check if album title is filled in
                if (tbAlbumTitle.Text.Trim().Length == 0) {
                    MessageBox.Show("Enter an album title.", "Missing entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // parse the release year into an int (to check if the proper format was given)
                var res_yr = int.TryParse(tbRelYear.Text, out int yr);

                // if the release year could not be parsed into an int,
                // or it could, but the year is earlier than 1950 (not many metal bands before that),
                // or later than next year, abort. (We allow to store albums we know will be released next year.)
                if (!res_yr || (res_yr && (yr < 1950 || yr > DateTime.Today.Year + 1))) {
                    MessageBox.Show("Provide a valid date - it should be between 1950 and " + DateTime.Today.Year + 1,
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // parse the rating into an int (to check if the propar format was written in the textbox)
                var res_rating = int.TryParse(tbRating.Text, out int rating);

                // if rating is not a number or is not between 1 and 10, abort
                if (!res_rating || (res_rating && (rating < 1 || rating > 10))) {
                    MessageBox.Show("Provide a valid rating - between 1.0 and 10.0",
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // get the band id based on the artist textbox (not the combobox, the user might have changed the artist of the album)
                var bandId = context.enMetalBands.Where(x => x.Band_name == cbAlbumArtist.Text).Select(x => x.Band_id).FirstOrDefault();

                // get the album loaded into the class object based on the artist id, and the album title
                enAlbum album = (from x in context.enAlbums where x.Album_title == cbAlbumTitle.Text && x.Band_id == bandId select x).First();

                if (album is null) {
                    return;
                } else {
                    // get the band id - we need this, because it might have been changed in the textbox
                    var new_band_id = context.enMetalBands.Where(x => x.Band_name == tbArtist.Text).Select(x => x.Band_id).FirstOrDefault();
                    // load all the other properties into the album class object
                    album.Album_title = tbAlbumTitle.Text;
                    album.Release_Year = yr;
                    album.Album_rating = rating;
                    album.Band_id = bandId;

                    // save the new details in the database
                    context.enAlbums.Update(album);
                    context.SaveChanges();
                    MessageBox.Show("Changes saved", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception err) {

                MessageBox.Show("Error in the code: " + err.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// This event fires when the "Save As New" button is clicked on a metal band update grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btSaveAsNewAlbum_Click(object sender, RoutedEventArgs e) {
            try {
                // check if an album title was written in the textbox
                if (tbAlbumTitle.Text.Trim().Length == 0) {
                    MessageBox.Show("Enter an album title.", "Missing entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                } else {
                    // check if an album with the same title, and of the same band already exists.
                    // If yes, abort, it will not be saved as a new one.
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

                // parse the release year into an int (to check if the proper format was given)
                var res_yr = int.TryParse(tbRelYear.Text, out int yr);

                // if the release year could not be parsed into an int,
                // or it could, but the year is earlier than 1950 (not many metal bands before that),
                // or later than next year, abort. (We allow to store albums we know will be released next year.)
                if (!res_yr || (res_yr && (yr < 1950 || yr > DateTime.Today.Year + 1))) {
                    MessageBox.Show("Provide a valid date - it should be between 1950 and " + DateTime.Today.Year + 1,
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // parse the rating into an int (to check if the propar format was written in the textbox)
                var res_rating = int.TryParse(tbRating.Text, out int rating);

                // if rating is not a number or is not between 1 and 10, abort
                if (!res_rating || (res_rating && (rating < 1 || rating > 10))) {
                    MessageBox.Show("Provide a valid rating - a whole number between 1 and 10",
                        "Invalid entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // get the band id based on the artist textbox
                var bandId = context.enMetalBands.Where(x => x.Band_name == tbArtist.Text).Select(x => x.Band_id).FirstOrDefault();

                if (bandId > 0) {
                    // of the band was found, save the properties into a new Album object, and add it to the Albums database table
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

        /// <summary>
        /// This event fires when album details are showed on the main window, and the "Delete" button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDeleteAlbum_Click(object sender, RoutedEventArgs e) {
            try {
                // get the band id in order to look up the exact album object to delete
                var bandId = context.enMetalBands.Where(x => x.Band_name == cbAlbumArtist.Text).Select(x => x.Band_id).FirstOrDefault();
                enAlbum album = (from x in context.enAlbums where x.Album_title == cbAlbumTitle.Text && x.Band_id == bandId select x).First();
                if (album is null) {
                    // no album found
                    MessageBox.Show("No such album in the database as " + cbAlbumTitle.Text,
                        "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                } else {
                    // confirm the deletion - if confirmed, remove the Album object from the Albums database table
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

        /// <summary>
        /// This event fires, when the add button is clicked - it is on the taskbar on the Add/Update musician datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAddMusician_Click(object sender, RoutedEventArgs e) {
            // a new window comes up where we can write the name and the instruments of the band member, and select bands they play in
            var wndMus = new AddOrUpdateMusician(context);
            var wndOk = wndMus.ShowDialog();
            if (wndOk == true) {
                var newMusician = new enMusician {
                    First_name = wndMus.NewFirstName,
                    Last_name = wndMus.NewLastName,
                    Instrument = wndMus.NewInstrument
                };

                // add the musician (we do it here, adding them to bands come later, as they might not be in any band for now)
                context.enMusicians.Add(newMusician);
                MessageBox.Show("Added musician: " + newMusician.FullName, "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // we initiate the MetalBand list many-to-many relation on the Musician object
                newMusician.MetalBands = new List<enMetalBand>();
                
                // add each selected band from the listbox to the list, and save. Separate messagebox upon each save
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

        /// <summary>
        /// This event fires, when musicians are listed for updating, and in one of the rows, the "Update" button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdateMusician_Click(object sender, RoutedEventArgs e) {
            // get the Musician object of the musician in the row where the button was pressed
            enMusician? musician = ((FrameworkElement)sender).DataContext as enMusician;
            if (musician is not null) {
                // pop-up window where the musician can be modified.
                // The window is not initiated with the musician object, so the current details can be loaded into the textboxes
                var wndMus = new AddOrUpdateMusician(context, musician);
                var wndOk = wndMus.ShowDialog();
                if (wndOk == true) {
                    // take the new data from the Musician update window
                    musician.First_name = wndMus.NewFirstName;
                    musician.Last_name = wndMus.NewLastName;
                    musician.Instrument = wndMus.NewInstrument;

                    // add any band selected in the listbox, but not yet added to the musician object
                    // (the musician joined (a) new band(s)
                    foreach (enMetalBand band in wndMus.SelBands) {
                        if (!musician.MetalBands.Contains(band)) {
                            musician.MetalBands.Add(band);
                        }
                    }

                    // remove any band currently added to the musician, but not selected on the listbox
                    // (the musician quit the band(s))
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

        /// <summary>
        /// This event fires, when musicians are listed for updating, and in one of the rows, the "Delete" button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btDeleteMusician_Click(object sender, RoutedEventArgs e) {
            // get the Musician object of the musician in the row where the button was pressed
            enMusician? musician = ((FrameworkElement)sender).DataContext as enMusician;
            if (musician is not null) {
                //confirm deletion of the musician - if confirmed remove the object from the Musician database table
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
