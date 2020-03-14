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
using System.Windows.Shapes;

namespace Assemble.me
{
    /// <summary>
    /// Interaction logic for ModelNameDialog.xaml
    /// </summary>
    public partial class ModelNameDialog : Window
    {
        private CarModel carModel;
        private bool isOrder;
        private OrderPriority priority;

        public ModelNameDialog(CarModel model, bool isOrder)
        {
            InitializeComponent();
            this.carModel = model;
            this.isOrder = isOrder;
            priority = OrderPriority.Normal;
            if (!isOrder)
                HidePriorityControls();
            else
            {
                lblPrice.Content = Connector.GetModelPrice() + "€";
                lblTime.Content = Connector.GetModelProductionTime() + "day(s)";
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string name = tbName.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                DataHelper.Fail("Please provide a valid name.");
            }
            else
            {
                if (isOrder){
                    if (rbPriorityHigh.IsChecked == true)
                    {
                        priority = OrderPriority.High;
                    }
                    else if (rbPriorityNormal.IsChecked == true)
                    {
                        priority = OrderPriority.Normal;
                    }
                    Order(name,priority);
                }    
                else
                    SaveToCloud(name);
            }
        }

        /// <summary>
        /// Generates an order for the current customer.
        /// </summary>
        /// <param name="name">Name of th model</param>
        /// <param name="o">Priority of the order.</param>
        private void Order(string name, OrderPriority o)
        {
            Connector.Order(Connector.GetCurrentCustomer(), name, o);
            this.Close();
        }

        /// <summary>
        /// Saves the model to a cloud.
        /// </summary>
        /// <param name="name">Name of the model.</param>
        private void SaveToCloud(string name)
        {
            carModel.SetName(name);
            Connector.SaveModelToCloud(carModel);
            this.Close();
        }

        /// <summary>
        /// Hides the user controls for selecting a priority.
        /// </summary>
        private void HidePriorityControls()
        {
            rbPriorityHigh.Visibility = Visibility.Hidden;
            rbPriorityNormal.Visibility = Visibility.Hidden;
            labelPriority.Visibility = Visibility.Hidden;
            saveButton.Margin = new Thickness(110, 137, 110, 0);
            this.Width = 326.276;
            this.Height = 252.659;
        }

        private void rbPriorityNormal_Click(object sender, RoutedEventArgs e)
        {
            lblPrice.Content = Connector.GetModelPrice() + "€";
            lblTime.Content = Connector.GetModelProductionTime() + "day(s)";
        }

        private void rbPriorityHigh_Click(object sender, RoutedEventArgs e)
        {
            lblPrice.Content = Connector.GetModelPrice(OrderPriority.High) + "€";
            lblTime.Content = Connector.GetModelProductionTime(OrderPriority.High) + "day(s)";
        }
    }
}
