using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite;
using StartFinance.Models;
using Windows.UI.Popups;
using SQLite.Net;
using System.Globalization;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");


        public AppointmentPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            //  Results();
        }

        public void Results()
        {
            conn.CreateTable<Appointment>();
            var query1 = conn.Table<Appointment>();
            AppointmentListView.ItemsSource = query1.ToList();
        }

        private async void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string calenderDay = eventDateDatePicker.Date.Day.ToString();
                string calenderMonth = eventDateDatePicker.Date.Month.ToString();
                string calenderYear = eventDateDatePicker.Date.Year.ToString();
                string Date =calenderDay + "/" + calenderMonth + "/" + calenderYear;


                string Hours = startTimePicker.Time.Hours.ToString();
                string Minutes = startTimePicker.Time.Minutes.ToString();
                string StartTime = " " + Hours + ":" + Minutes;

                string Hours2 = endTimePicker.Time.Hours.ToString();
                string Minutes2 = endTimePicker.Time.Minutes.ToString();
                string EndTime = " " + Hours2 + ":" + Minutes2;



                if (EventNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No value entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {

                    conn.CreateTable<Appointment>();
                    conn.Insert(new Appointment
                    {
                        //  EventID = EventIdTexBox.Text.ToString(),
                        EventName = EventNameTextBox.Text.ToString(),
                        EventLocation = EventLocationTextBox.Text.ToString(),
                        EventDate = Date.ToString(),
                        StartTime = StartTime.ToString(),
                        EndTime = EndTime.ToString()

                    });
                    // Creating table
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the event Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("event Name Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int AccSelection = ((Appointment)AppointmentListView.SelectedItem).EventID;
                if (AccSelection == -1)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<Appointment>();
                    var query1 = conn.Table<Appointment>();
                    var query3 = conn.Query<Appointment>("DELETE FROM Appointment WHERE EventID ='" + AccSelection + "'");
                    AppointmentListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (AppointmentListView.SelectedIndex > -1)
            {
                btnUpdate.Visibility = Visibility.Visible;
                string EventName = ((Appointment)AppointmentListView.SelectedItem).EventName;
                string EventLocation = ((Appointment)AppointmentListView.SelectedItem).EventLocation;
                string eventDate = ((Appointment)AppointmentListView.SelectedItem).EventDate;
                string startTime = ((Appointment)AppointmentListView.SelectedItem).StartTime;
                string endTime = ((Appointment)AppointmentListView.SelectedItem).EndTime;

                
                EventNameTextBox.Text = "";
                EventLocationTextBox.Text = "";
            }
            else
            {
                MessageDialog dialog = new MessageDialog("No Selected item to Edit", "Oops..!");
                await dialog.ShowAsync();
            }
        }
        

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int AccSelection = ((Appointment)AppointmentListView.SelectedItem).EventID;
            string CalenderDay = eventDateDatePicker.Date.Day.ToString();
            string calenderMonth = eventDateDatePicker.Date.Month.ToString();
            string calenderYear = eventDateDatePicker.Date.Year.ToString();
            string Date = CalenderDay + "/" + calenderMonth + "/" + calenderYear;


            string Hours = startTimePicker.Time.Hours.ToString();
            string Minutes = startTimePicker.Time.Minutes.ToString();
            string StartTime = " " + Hours + ":" + Minutes;

            string Hours2 = endTimePicker.Time.Hours.ToString();
            string Minutes2 = endTimePicker.Time.Minutes.ToString();
            string EndTime = " " + Hours2 + ":" + Minutes2;

            string EventName = EventNameTextBox.Text.ToString();
            string EventLocation = EventLocationTextBox.Text.ToString();
            string EventDate = Date.ToString();
            string startTime = StartTime.ToString();
            string endTime = EndTime.ToString();

            var query = conn.Query<Appointment>("UPDATE Appointment SET EventName = '" + EventName + "', EventLocation = '" + EventLocation + "', EventDate = '" + Date + "', StartTime = '" + StartTime + "', EndTime = '" + EndTime + "' WHERE EventID ='" + AccSelection + "'");

            EventNameTextBox.Text = "";
            EventLocationTextBox.Text = "";

            btnUpdate.Visibility = Visibility.Collapsed;
            Results();
        }
    }


}

