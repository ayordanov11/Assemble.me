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

namespace Assemble.me
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Validates the set of characters that can be used in the form.
        /// </summary>
        /// <returns>True if form is valid or false if it is not.</returns>
        private bool ValidateLoginForm()
        {
            if (!string.IsNullOrWhiteSpace(loginEmail.Text.ToString()) && loginID.Text.ToString().All(char.IsDigit) &&
                !string.IsNullOrWhiteSpace(loginID.Text.ToString()))
                return true;
            else
                return false;
        }

        private void goToRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("RegisterPage.xaml", UriKind.Relative));
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateLoginForm())
            {
                if (Connector.LogIn(loginEmail.Text, Convert.ToInt32(loginID.Text)))
                {
                    new Window1().Show();

                    foreach (Window w in Application.Current.Windows)
                    {
                        if (w.Title == "Welcome to Assemble.me")
                            w.Close();
                    }
                }
                else
                {
                    DataHelper.Fail("Wrong credentials!");
                }
            }
            else
            {
                DataHelper.Fail("Invalid input fields.");
            }
        }
    }
}
