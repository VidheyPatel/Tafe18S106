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
    public sealed partial class ContactListPage : Page
    {
        SQLiteConnection conn;
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public ContactListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

        }
        public void Results()
        {
            conn.CreateTable<ContactList>();
            var query1 = conn.Table<ContactList>();
            ContactListView.ItemsSource = query1.ToList();
        }

        private async void AddPersonalInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               

                if (txtFirstName.Text == "" || txtLastName.Text == ""  || txtPhone.Text == "" || txtCompanyName.Text == "")
                {
                    MessageDialog Dialog = new MessageDialog("Something is missing");
                    await Dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ContactList>();
                    conn.Insert(new ContactList
                    {
                        FirstName = txtFirstName.Text.ToString(),
                        LastName = txtLastName.Text.ToString(),
                        CompanyName = txtCompanyName.Text.ToString(),
                        Phone = txtPhone.Text.ToString()
                    });
                    Results();
                }

            }
            catch (Exception ex)
            {
                
                if (ex is SQLiteException)
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
            if (ContactListView.SelectedIndex > -1)
            {
                btnUpdate.Visibility = Visibility.Visible;
                int ContactID = ((ContactList)ContactListView.SelectedItem).ContactID;
                string firstName = ((ContactList)ContactListView.SelectedItem).FirstName;
                string lastName = ((ContactList)ContactListView.SelectedItem).LastName;
                string companyName = ((ContactList)ContactListView.SelectedItem).CompanyName;
                string phone = ((ContactList)ContactListView.SelectedItem).Phone;
                

                txtFirstName.Text = "";
                txtLastName.Text = "";
                txtCompanyName.Text = "";
                txtPhone.Text = "";
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
                    string ContactLabel = ((ContactList)ContactListView.SelectedItem).FirstName;
                    var querydel = conn.Query<ContactList>("DELETE FROM CONTACTLIST WHERE FirstName='" + ContactLabel + "'");
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
            int Contact = ((ContactList)ContactListView.SelectedItem).ContactID;
            string firstName = txtFirstName.Text.ToString();
            string lastName = txtLastName.Text.ToString();
            string phone = txtPhone.Text.ToString();
            string companyName = txtCompanyName.Text.ToString();
            var query = conn.Query<ContactList>("UPDATE ContactList SET FirstName = '" + firstName + "',LastName = '" + lastName + "',Phone = '" + phone + "',CompanyName = '" + companyName + "'WHERE ContactID = '" + Contact + "'");
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtCompanyName.Text = "";
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
