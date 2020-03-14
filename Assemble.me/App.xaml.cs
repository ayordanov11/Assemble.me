using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Dropbox.Api;

namespace Assemble.me
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            using (var dbx = new DropboxClient("khJDxv0h3M4AAAAAAAAHujvHGMs7MwehZEDaxyKHCdjHk2S9IcSR8jRsO3dEy1Oo"))
            {
                var full = await dbx.Users.GetCurrentAccountAsync();
                Console.WriteLine("{0} - {1}", full.Name.DisplayName, full.Email);
            }
        }
    }
}
