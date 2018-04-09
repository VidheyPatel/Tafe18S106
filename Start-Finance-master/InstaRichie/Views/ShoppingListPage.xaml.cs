using SQLite.Net;
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
using StartFinance.Models;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ShoppingListPage : Page
    {
        SQLiteConnection conn;
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public ShoppingListPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        public void Results()
        {
            conn.CreateTable<ShoppingList>();
            var query1 = conn.Table<ShoppingList>();
            ShoppingListPageView.ItemsSource = query1.ToList();
        }
        
        private async void AddList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string CDay = shoppingDatePicker.Date.Value.Day.ToString();
                string CMonth = shoppingDatePicker.Date.Value.Month.ToString();
                string CYear = shoppingDatePicker.Date.Value.Year.ToString();
                string FinalDate = "" + CMonth + "/" + CDay + "/" + CYear;

                if (nameTextBox.Text.ToString() == "" ||          
                    itemNameTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No value entered");
                    await dialog.ShowAsync();
                }
                else
                {
                    double PriceQ = Convert.ToDouble(priceTextBox.Text);
                    conn.CreateTable<ShoppingList>();
                    conn.Insert(new ShoppingList
                    {
                        ShoppingDate = FinalDate.ToString(),
                        ShopName = nameTextBox.Text.ToString(),
                        NameOfItem = itemNameTextBox.Text.ToString(),
                        PriceQuoted = PriceQ
                    });

                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid Amount", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("List Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }


            }
        }

        private async void DeleteList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ListSelection = ((ShoppingList)ShoppingListPageView.SelectedItem).ShoppingItemID;
                if (ListSelection == -1)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ShoppingList>();
                    var query1 = conn.Table<ShoppingList>();
                    var query3 = conn.Query<ShoppingList>("DELETE FROM ShoppingList WHERE ShoppingItemID ='" + ListSelection + "'");
                    ShoppingListPageView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }

        private async void EditList_Click(object sender, RoutedEventArgs e)
        {

            if (ShoppingListPageView.SelectedIndex > -1)
            {
                updateButton.Visibility = Visibility.Visible;
                int shoppingID = ((ShoppingList)ShoppingListPageView.SelectedItem).ShoppingItemID;
                string shopName = ((ShoppingList)ShoppingListPageView.SelectedItem).ShopName;
                string nameOfItem = ((ShoppingList)ShoppingListPageView.SelectedItem).NameOfItem;
                string shoppingDate = ((ShoppingList)ShoppingListPageView.SelectedItem).ShoppingDate;
                double priceQuoted = ((ShoppingList)ShoppingListPageView.SelectedItem).PriceQuoted;

                nameTextBox.Text = "";
                itemNameTextBox.Text = "";
                priceTextBox.Text = "";
            }
            else
            {
                MessageDialog dialog = new MessageDialog("No Selected item to Edit");
                await dialog.ShowAsync();
            }
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            int ListSelection = ((ShoppingList)ShoppingListPageView.SelectedItem).ShoppingItemID;
            string CDay = shoppingDatePicker.Date.Value.Day.ToString();
            string CMonth = shoppingDatePicker.Date.Value.Month.ToString();
            string CYear = shoppingDatePicker.Date.Value.Year.ToString();
            string FinalDate = "" + CMonth + "/" + CDay + "/" + CYear;


            string shopName = nameTextBox.Text.ToString();
            string shoppingDate = FinalDate.ToString();
            string nameOfItem = itemNameTextBox.Text.ToString();
            double PriceQ = Convert.ToDouble(priceTextBox.Text);

            var query = conn.Query<ShoppingList>("UPDATE ShoppingList SET ShopName = '" + shopName + "', ShoppingDate = '" + shoppingDate + "', NameOfItem ='" + nameOfItem + "', PriceQuoted = '" + PriceQ + "'  WHERE ShoppingItemID ='" + ListSelection + "'");
            

            nameTextBox.Text = "";
            itemNameTextBox.Text = "";
            priceTextBox.Text = "";

            updateButton.Visibility = Visibility.Collapsed;
            Results();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
    }
}

