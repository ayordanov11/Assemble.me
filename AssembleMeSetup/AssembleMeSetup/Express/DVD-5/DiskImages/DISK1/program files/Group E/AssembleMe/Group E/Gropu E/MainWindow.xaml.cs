using Assemble.me.Library;
using Assemble.me.Library.PackageOrder;
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
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Assemble.me.Administrator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();


        // method executed on each timer tick
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            admin.Randomization.GenerateRandomOrder();
            try
            {
                if (this.listBoxOrders.Items.Count > 0)
                {
                    this.listBoxOrders.SelectedIndex = 0;
                    if (listBoxOrders.SelectedItem != null)
                    {
                        admin.ProcessOrder(((Order)listBoxOrders.SelectedItem).ID);
                    }
                }
                    
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
            }

            this.LoadOrders();
        }

        Administration admin;

        public MainWindow()
        {
            InitializeComponent();
            admin = new Administration();

            // timer that generates random orders per tick
            dispatcherTimer.Tick += dispatcherTimer_Tick;

            // rate 10 econds
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);

            //  uncomment if the eandom orders should start by default
            //  dispatcherTimer.Start();

        }

        private void btnViewAllOrders_Click(object sender, RoutedEventArgs e)
        {
            this.LoadOrders();
        }

        /// <summary>
        /// Displays all pending orders on screen.
        /// </summary>
        private void LoadOrders()
        {
            label.Content = "Orders";
            label.Margin = new Thickness(203, 2, 10, 0);
            if (btnEnableRandomOrders.IsEnabled == true)
            {
                btnChangePriority.IsEnabled = true;
                btnProcessOrder.IsEnabled = true;
            }
            // Hide fields for order's processed date
            OrderGroupBox.Height = 146;
            OrderGroupBox.Margin = new Thickness(23, 65, 0, 0);
            lblOrderNr.Margin = new Thickness(139, 92, 0, 0);
            lblOrderProcessedDate.Visibility = Visibility.Hidden;
            lblProcessed.Visibility = Visibility.Hidden;

            listBoxOrders.Items.Clear();

            List<Order> temp = admin.GetOrders();

            // sorts the list of orders by proiority
            temp.Sort((Order x, Order y) => y.Priority.CompareTo(x.Priority));
            DateTime mostRecent = new DateTime(1,1,1);
            int index = 0;
            foreach (Order o in temp)
            {
                listBoxOrders.Items.Add(o);
                if (mostRecent < o.CreatedAt) 
                { 
                    mostRecent = o.CreatedAt;
                    index = listBoxOrders.Items.IndexOf(o);
                }
            }
            listBoxOrders.SelectedIndex = index;

        }

        /// <summary>
        /// Displays all processed orders on screen.
        /// </summary>
        private void LoadProcessedOrders()
        {
            label.Content = "Processed Orders";
            label.Margin = new Thickness(162, 2, 51, 0);
            // Disable changing priority because processed orders' priority can't be changed
            btnChangePriority.IsEnabled = false;
            btnProcessOrder.IsEnabled = false;
            // Show fields for order's processed date
            lblOrderProcessedDate.Visibility = Visibility.Visible;
            lblProcessed.Visibility = Visibility.Visible;
            OrderGroupBox.Height = 170;
            OrderGroupBox.Margin = new Thickness(23, 41, 0, 0);
            lblOrderNr.Margin = new Thickness(139, 68, 0, 0);

            listBoxOrders.Items.Clear();
            foreach (Order o in admin.GetProcessedOrders())
            {
                listBoxOrders.Items.Add(o);
            }
        }

        /// <summary>
        /// Disables random order generation.
        /// </summary>
        private void DisableRandomOrders()
        {
            dispatcherTimer.Stop();
            btnEnableRandomOrders.IsEnabled = true;
            btnDisableRandomOrders.IsEnabled = false;
            btnProcessOrder.IsEnabled = true;
            btnChangePriority.IsEnabled = true;
        }

        #region Events
        private void btnProcessOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxOrders.SelectedItem != null)
                {
                    admin.ProcessOrder(((Order)listBoxOrders.SelectedItem).ID);
                    LoadOrders();
                }
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
            }

        }

        private void btnChangePriority_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (listBoxOrders.SelectedIndex != -1)
                {
                    Order t = ((Order)listBoxOrders.SelectedItem);
                    int index = listBoxOrders.SelectedIndex;

                    if (t.Priority == OrderPriority.High)
                    {
                        t.ChangeOrderPriority(OrderPriority.Normal);
                    }
                    else
                    {
                        t.ChangeOrderPriority(OrderPriority.High);
                    }
                    LoadOrders();
                    listBoxOrders.SelectedIndex = index;
                }
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void btnViewProcessedOrders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnDisableRandomOrders.IsEnabled == true) {
                    DisableRandomOrders();
                    LoadProcessedOrders();
                    MessageBox.Show("Order processing has been paused.");
                }
                else { LoadProcessedOrders(); }
                
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void listBoxOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (listBoxOrders.SelectedIndex != -1)
                {
                    Order selectedOrder = (Order)listBoxOrders.SelectedItem;

                    // Display order details
                    lblOrderNr.Content = selectedOrder.ID;
                    lblOrderCreatedDate.Content = selectedOrder.CreatedAt;
                    lblOrderExpectedDate.Content = selectedOrder.ExpectedDate;
                    lblOrderPriority.Content = selectedOrder.Priority;
                    lblOrderProcessedDate.Content = selectedOrder.ProcessedAt;

                    // Display customer details
                    lblCustName.Content = selectedOrder.Customer.GetName();
                    lblCustEmail.Content = selectedOrder.Customer.Email;
                    lblCustPhone.Content = selectedOrder.Customer.Phone;

                    // Display model details
                    lblModelName.Content = selectedOrder.Model.Name;
                    lblModelPrice.Content = selectedOrder.Model.GetPrice() + "€";
                }
            }
            catch (MySqlException exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void btnInventory_Click(object sender, RoutedEventArgs e)
        {
            InventoryWindow inventory = new InventoryWindow();
            inventory.Show();
        }

        private void btnEnableRandomOrders_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Start();
            LoadOrders();
            btnEnableRandomOrders.IsEnabled = false;
            btnDisableRandomOrders.IsEnabled = true;
            btnProcessOrder.IsEnabled = false;
            btnChangePriority.IsEnabled = false;
        }

        private void btnDisableRandomOrders_Click(object sender, RoutedEventArgs e)
        {
            DisableRandomOrders();
        }

        private void btnTimeRatioSettings_Click(object sender, RoutedEventArgs e)
        {
            setRatioGrid.Visibility = Visibility.Visible;
        }

        private void btnTimeRatioSet_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, Convert.ToInt32(sliderTimeRatio.Value));
            setRatioGrid.Visibility = Visibility.Hidden;
        }

        private void btnClearUnprocessedOrders_Click(object sender, RoutedEventArgs e)
        {
            admin.ClearDatabaseUnprocessedOrders();
            LoadOrders();
        }

        private void btnClearProcessed_Click(object sender, RoutedEventArgs e)
        {
            admin.ClearDatabaseProcessedOrders();
            LoadProcessedOrders();
        }
        #endregion
    }
}
