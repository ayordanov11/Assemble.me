using Assemble.me.Library;
using Assemble.me.Library.Parts;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Assemble.me.Administrator
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class InventoryWindow : Window
    {
        Dictionary<CarPart, int> cart;
        ObservableCollection<PartQuantity> parts;
        int currentPartNr = 0;
        public InventoryWindow()
        {
            InitializeComponent();
            BindAllParts(); // by default all parts are shown
            cart = new Dictionary<CarPart, int>();
        }

        /// <summary>
        /// Pupulates the DataGrid with parts that have quantity higher than 0.
        /// </summary>
        private void BindAvailableParts()
        {
            try
            {
                parts = new ObservableCollection<PartQuantity>();
                foreach (PartQuantity cp in Inventory.GetAvailableParts())
                {
                    parts.Add(cp);
                }
                dataGrid.ItemsSource = parts;
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        /// <summary>
        /// Populates the DataGrid with parts that are out of stock.
        /// </summary>
        private void BindPartsOutOfStock()
        {
            try
            {
                parts = new ObservableCollection<PartQuantity>();
                foreach (PartQuantity cp in Inventory.GetUnavailableParts())
                {
                    parts.Add(cp);
                }
                dataGrid.ItemsSource = parts;
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        /// <summary>
        /// Bind all parts in the inventory.
        /// </summary>
        private void BindAllParts()
        {
            try
            {
                parts = new ObservableCollection<PartQuantity>();
                foreach (PartQuantity cp in Inventory.GetAllParts())
                {
                    parts.Add(cp);
                }
                dataGrid.ItemsSource = parts;
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        /// <summary>
        /// Updates the screen with the realtime list of parts.
        /// </summary>
        private void UpdateCart()
        {
            lbCart.Items.Clear();
            tbQuantity.Clear();
            foreach (var i in cart)
            {
                lbCart.Items.Add(i.Value + "x " + i.Key.Name);
            }
        }

        /// <summary>
        /// Validates the text box, allowing the user to only input digits.
        /// </summary>
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// Clears the values of all input fields on the screen.
        /// </summary>
        private void WipeFields()
        {
            lbCart.Items.Clear();
            tbQuantity.Clear();
            lblNr.Content = "";
        }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedIndex != -1)
            {
                currentPartNr = ((PartQuantity)dataGrid.SelectedItem).PartId;
                lblNr.Content = currentPartNr;
            }
        }

        private void btnAvailable_Click(object sender, RoutedEventArgs e)
        {
            BindAvailableParts();
        }

        private void btnOutOfStock_Click(object sender, RoutedEventArgs e)
        {
            BindPartsOutOfStock();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try {
                if (!string.IsNullOrWhiteSpace(tbQuantity.Text))
                {
                    cart.Add(ApplicationSettings.GetPartById(currentPartNr), Convert.ToInt32(tbQuantity.Text));
                    UpdateCart();
                }
                else
                {
                    MessageBox.Show("The value for quantity can not be empty.");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("No part selected.");
            }
                
               
        }

        private void mouse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(cart.Count != 0)
                {
                    foreach (var part in cart)
                    {
                        Inventory.PurchaseParts(part.Key, part.Value);
                    }
                    cart = new Dictionary<CarPart, int>();
                    this.WipeFields();
                    MessageBox.Show("You have successfully purchased the parts.");
                    BindAllParts();
                }
                MessageBox.Show("The cart is empty.");
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void btnAllParts_Click(object sender, RoutedEventArgs e)
        {
            BindAllParts();
        }
    }
}
