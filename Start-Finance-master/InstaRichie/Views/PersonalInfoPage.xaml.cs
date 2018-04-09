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
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn;
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

        }
        public void Results()
        {
            conn.CreateTable<PersonalInfo>();
            var query1 = conn.Table<PersonalInfo>();
            PersonalInfoListView.ItemsSource = query1.ToList();
        }

        private async void AddPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string CDay = DOBDate.Date.Day.ToString();
                string CMonth = DOBDate.Date.Month.ToString();
                string CYear = DOBDate.Date.Year.ToString();
                string FinalDate = " On " + CMonth + "/" + CDay + "/" + CYear;

                if (txtFirstName.Text == "" || txtLastName.Text == "" || txtEmail.Text == "" || txtPhone.Text == "" || txtGender.Text == "")
                {
                    MessageDialog Dialog = new MessageDialog("Something is missing");
                    await Dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<PersonalInfo>();
                    conn.Insert(new PersonalInfo
                    {
                        FirstName = txtFirstName.Text.ToString(),
                        LastName = txtLastName.Text.ToString(),
                        Gender = txtGender.Text.ToString(),
                        Email = txtEmail.Text.ToString(),
                        Phone = txtPhone.Text.ToString(),
                        DOB = FinalDate.ToString()
                    });
                    Results();
                }

            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the date or entered an invalid Date", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        private async void EditBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (PersonalInfoListView.SelectedIndex > -1)
            {
                btnUpdate.Visibility = Visibility.Visible;
                int PersonID = ((PersonalInfo)PersonalInfoListView.SelectedItem).PersonalID;
                string firstName = ((PersonalInfo)PersonalInfoListView.SelectedItem).FirstName;
                string lastName = ((PersonalInfo)PersonalInfoListView.SelectedItem).LastName;
                string gender = ((PersonalInfo)PersonalInfoListView.SelectedItem).Gender;
                string email = ((PersonalInfo)PersonalInfoListView.SelectedItem).Email;
                string phone = ((PersonalInfo)PersonalInfoListView.SelectedItem).Phone;
                string date = ((PersonalInfo)PersonalInfoListView.SelectedItem).DOB;

                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtGender.Text = "";
                txtPhone.Text = "";
                txtEmail.Text = "";
            }
            else
            {
                MessageDialog dialog = new MessageDialog("Not selected");
                await dialog.ShowAsync();
            }
        }

        private async void DeleteBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this info will delete all info", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.DefaultCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                try
                {
                    string PersonLabel = ((PersonalInfo)PersonalInfoListView.SelectedItem).FirstName;
                    var querydel = conn.Query<PersonalInfo>("DELETE FROM PERSONALINFO WHERE FirstName='" + PersonLabel + "'");
                    Results();

                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select item to delete", "OOps....!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }



        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int Person = ((PersonalInfo)PersonalInfoListView.SelectedItem).PersonalID;
            string Day = DOBDate.Date.Day.ToString();
            string Month = DOBDate.Date.Month.ToString();
            string Year = DOBDate.Date.Year.ToString();
            string FinalDate = "/" + Month + "/" + Day + "/" + Year;
            string firstName = txtFirstName.Text.ToString();
            string lastName = txtLastName.Text.ToString();
            string phone = txtPhone.Text.ToString();
            string email = txtEmail.Text.ToString();
            string gender = txtGender.Text.ToString();
            var query = conn.Query<PersonalInfo>("UPDATE PersonalInfo SET FirstName = '" + firstName + "',LastName = '" + lastName + "',Phone = '" + phone + "',Email = '" + email + "',Gender = '" + gender + "',DOB = '" + FinalDate + "'WHERE PersonalID = '" + Person + "'");
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtGender.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            btnUpdate.Visibility = Visibility.Collapsed;
            Results();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
    }
}
