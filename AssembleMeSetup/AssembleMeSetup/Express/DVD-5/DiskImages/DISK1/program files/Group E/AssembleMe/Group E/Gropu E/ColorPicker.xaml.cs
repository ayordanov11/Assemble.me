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
using Assemble.me.Library.Parts.PackageChassis;

namespace Assemble.me
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        private string pickedColor = null;
        public ColorPicker()
        {
            InitializeComponent();
            Pickbtn.IsEnabled = false;
            MouseDown += (sender, args) => DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var box = ComboBoxColor.SelectedItem;
            if (box == Red)
                pickedColor = "Red";
            else if (box == Blue)
                pickedColor = "Blue";
            else if (box == Gray)
                pickedColor = "Gray";
            else
                pickedColor = null;
            this.Close();
        }

        public ChassisColors GetColor()
        {
            this.ShowDialog();
            switch (pickedColor)
            {
                case "Red":
                {
                    return ChassisColors.Red;
                }
                case "Blue":
                {
                    return ChassisColors.Blue;
                }
                default:
                {
                    return ChassisColors.Grey;
                }
            }
        }

        private void ComboBoxColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pickbtn.IsEnabled = true;
        }
    }
}
