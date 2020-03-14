using Assemble.me.Library.PackageCustomer;
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
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateRegisterForm())
            {
                string fname = FName.Text;
                string lname = LName.Text;
                string email = Email.Text;
                string phone = Phone.Text;

                if (!Connector.VerifyEmail(email))
                {
                    DataHelper.Fail("Email already taken, please use a different one.");
                    
                }
                else
                {
                    Customer cust = new Customer(fname, lname, email, phone);
                    Connector.SetCurrentCustomer(cust);
                    Connector.SaveCustomer();

                    DataHelper.Success("You successfully registered! Your ID is " + Connector.GetCurrentCustomer().ID + ". Remember it so you can login later!");

                    new Window1().Show();
                    foreach (Window w in Application.Current.Windows)
                    {
                        if (w.Title == "Welcome to Assemble.me")
                            w.Close();
                    }
                }
                
            }
            else
                DataHelper.Fail("Some of the input fields are not valid!");
        }

        /// <summary>
        /// Checks if the email is a valid email.
        /// </summary>
        /// <param name="email">The email from the form.</param>
        /// <returns>True if valid and false if not.</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);

                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates the set of characters that can be used in the form.
        /// </summary>
        /// <returns>true if vallid and false if not.</returns>
        private bool ValidateRegisterForm()
        {
            if (FName.Text.ToString().All(char.IsLetter) && LName.Text.ToString().All(char.IsLetter)
                && IsValidEmail(Email.Text.ToString()) && Phone.Text.ToString().All(char.IsDigit))
                return true;
            else
                return false;
        }
    }
}
