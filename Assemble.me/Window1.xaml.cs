using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Dropbox.Api;
using Assemble.me.Library;
using Assemble.me.Library.Parts;
using Assemble.me.Library.Parts.PackageChassis;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Assemble.me
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private readonly ModelVisual3D device;
        public readonly DropboxClient dbx;
        private DropBoxGui gui;
        private Model3DGroup parts;
        private List<CarPart> orderParts;
        private CarPart part;
        private bool isPreviouslySaved = false;

        public Window1()
        {
            InitializeComponent();
            device = new ModelVisual3D();
            viewport3D.Children.Add(device);
            dbx = new DropboxClient("khJDxv0h3M4AAAAAAAAHujvHGMs7MwehZEDaxyKHCdjHk2S9IcSR8jRsO3dEy1Oo");
            gui = new DropBoxGui(dbx, this);
            parts = new Model3DGroup();
            orderParts = new List<CarPart>();
            Connector.SetMainWindow(this);
            AddToBtn.Visibility = Visibility.Hidden;
            Cancel_Part.Visibility = Visibility.Hidden;
            OrderBtn.IsEnabled = false;
            Populate(true);

            Partslb.Opacity = 0.5;
            modelslb.Opacity = 0.5;
        }

        #region Events

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearScreen();
            Populate(true);
        }

        // Handles the scroll event.
        private void viewport3D_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //zoom out
            if (viewport3D.Camera.LookDirection.Length >= 800 && e.Delta < 0)
            {
                e.Handled = true;
            }

            //zoom in
            if (viewport3D.Camera.LookDirection.Length <= 15 && e.Delta > 0)
            {
                e.Handled = true;
            }
        }

        // Dynamically updates the lists of items
        private void Partslb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Partslb.SelectedIndex != -1)
            {
                string selectedItemName = Partslb.SelectedItem.ToString();
                modelslb.Items.Clear();

                foreach (string item in DataHelper.GetPartNamesByType(selectedItemName))
                {
                    modelslb.Items.Add(item);
                }

                if (selectedItemName == "Extras")
                {
                    if (MenuBtn.Items != null)
                        foreach (MenuItem v in MenuBtn.Items)
                        {
                            if (v.Header.ToString() == "Extras")
                                foreach (MenuItem extra in v.Items)
                                {
                                    if (modelslb.Items.Contains(extra.Header.ToString()))
                                        modelslb.Items.Remove(extra.Header.ToString());
                                }
                        }
                }

                if (selectedItemName == "Tires")
                {
                    RestrictTiresOrRims("Tires", "Rims");
                }
                if (selectedItemName == "Rims")
                {
                    RestrictTiresOrRims("Rims", "Tires");
                }
            }
        }

        private void modelslb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modelslb.SelectedIndex != -1)
            {
                string modelListBoxSelectedItem = modelslb.SelectedItem.ToString();
                string partsListBoxSelectedItem = Partslb.SelectedItem.ToString();

                ChassisColors color = ChassisColors.Grey;
                if (partsListBoxSelectedItem == "Chassis")
                {
                    ColorPicker cp = new ColorPicker();
                    color = cp.GetColor();
                }

                var model = Connector.LoadModel(modelListBoxSelectedItem, out part, color);

                if (model != null)
                {
                    parts.Children.Add(model);
                    BottomGroupBoxLoader(modelListBoxSelectedItem, part);
                    device.Content = parts;

                    Connector.DisableButtons();

                    viewport3D.CameraController.ResetCamera();
                    viewport3D.RotateGesture = new MouseGesture(MouseAction.LeftClick);
                }
                else
                    Xceed.Wpf.Toolkit.MessageBox.Show("Something went wrong part not loaded =( ");
            }
        }

        // GUI resize handeler
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            myGrid.Width = e.NewSize.Width;
            myGrid.Height = e.NewSize.Height;
            GroupBoxBot.Width = e.NewSize.Width - (e.NewSize.Width * 0.05);
            GroupBoxBot.Height = e.NewSize.Height * 0.15;
            Description_Photo.Width = e.NewSize.Width * 0.1;
            Description_Photo.Height = e.NewSize.Height * 0.1;
            Description_panelText.Width = GroupBoxBot.Width - (e.NewSize.Width * 0.005);
            Description_Text.Width = Description_panelText.Width - (Description_panelText.Width * 0.3);

            Selected_PartsBtn.Margin = Connector.Resizer(1, 0, 40, 40, e.NewSize.Height, e.NewSize.Width);
            OrderBtn.Margin = Connector.Resizer(1, 0, 0, 1, e.NewSize.Height, e.NewSize.Width);
            ModelDetailsBtn.Margin = Connector.Resizer(1, 0, 0, 9, e.NewSize.Height, e.NewSize.Width);
            Description_panelText.Margin = Connector.Resizer(2, 0, 8, 0, e.NewSize.Height, e.NewSize.Width);
            Description_Text.Margin = Connector.Resizer(1, 0, 2, 0, e.NewSize.Height, e.NewSize.Width);
            Descriprion_Header.Margin = Connector.Resizer(0, 0, 10, 0, e.NewSize.Height, e.NewSize.Width);
            GroupBoxBot.Margin = Connector.Resizer(60, 0, 0, 0, e.NewSize.Height, e.NewSize.Width);
            ZoomGrid.Margin = Connector.Resizer(4, 0, 25, 0, e.NewSize.Height, e.NewSize.Width);
            ZoomOut.Margin = Connector.Resizer(10, 0, 0, 0, e.NewSize.Height, e.NewSize.Width);
            Partslb.Margin = Connector.Resizer(10, 0, 1, 0, e.NewSize.Height, e.NewSize.Width);
            modelslb.Margin = Connector.Resizer(10, 0, 0, 1, e.NewSize.Height, e.NewSize.Width);
            BtnGrid.Margin = Connector.Resizer(70, 0, 25, 10, e.NewSize.Height, e.NewSize.Width);
            RotateLeft.Margin = Connector.Resizer(0, 0, 0, 40, e.NewSize.Height, e.NewSize.Width);
            RotateRight.Margin = Connector.Resizer(0, 0, 40, 0, e.NewSize.Height, e.NewSize.Width);

            double xChange = 1, yChange = 1;

            if (e.PreviousSize.Width != 0)
            {
                xChange = (e.NewSize.Width / e.PreviousSize.Width);
            }

            if (e.PreviousSize.Height != 0)
                yChange = (e.NewSize.Height / e.PreviousSize.Height);

            foreach (FrameworkElement fe in myGrid.Children)
            {
                if (fe is Grid == false)
                {
                    fe.Height = fe.ActualHeight * yChange;
                    fe.Width = fe.ActualWidth * xChange;

                    Canvas.SetTop(fe, Canvas.GetTop(fe) * yChange);
                    Canvas.SetLeft(fe, Canvas.GetLeft(fe) * xChange);

                }
            }
        }

        // Resets the camera view to the default one.
        private void Reset_Camera_Click(object sender, RoutedEventArgs e)
        {
            viewport3D.CameraController.ResetCamera();
        }

        private void RotateLeft_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewport3D.CameraController.AddRotateForce(5, 0);
        }

        private void RotateRight_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewport3D.CameraController.AddRotateForce(-5, 0);
        }

        private void ZoomIn_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (viewport3D.Camera.LookDirection.Length >= 15)
                viewport3D.CameraController.AddZoomForce(-0.2);
        }

        private void ZoomOut_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (viewport3D.Camera.LookDirection.Length <= 800)
                viewport3D.CameraController.AddZoomForce(0.2);
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            // Ask user if he wants to save previous model
            switch (DataHelper.Dialog("Do you want to save the current changes before opening a new project?"))
            {
                case MessageBoxResult.Yes:
                    if (orderParts.Count > 0)
                    {
                        if (!isPreviouslySaved)
                        {
                            new ModelNameDialog(Connector.GetCurrentModel(), false).Show();
                            isPreviouslySaved = true;
                        }
                        else
                            MessageBox.Show("Save aborted ! \nThis model has already been saved, you cannot save the same model !");
                    }
                    else
                        MessageBox.Show("You cannot save an empty model!");
                    ClearScreen();
                    Populate(true);
                    Connector.NewModel();
                    break;
                case MessageBoxResult.No:
                    ClearScreen();
                    Populate(true);
                    Connector.NewModel();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            switch (DataHelper.Dialog("Do you want to save the current changes before opening a project?"))
            {
                case MessageBoxResult.Yes:
                    if (orderParts.Count > 0)
                    {
                        if (!isPreviouslySaved)
                        {
                            new ModelNameDialog(Connector.GetCurrentModel(), false).ShowDialog();
                            isPreviouslySaved = true;
                        }
                        else
                            MessageBox.Show(
                                "Save aborted !\n This model has already been saved, you cannot save the same model !");
                    }
                    else
                        MessageBox.Show("You cannot save empty model !");
                    gui.ShowDialog();
                    break;
                case MessageBoxResult.No:
                    gui.ShowDialog();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (orderParts.Count > 0)
            {
                if (!isPreviouslySaved)
                {
                    new ModelNameDialog(Connector.GetCurrentModel(), false).Show();
                    isPreviouslySaved = true;
                }
                else
                    MessageBox.Show(
                        "Save aborted ! \nThis model has already been saved, you cannot save the same model !");
            }
            else
                MessageBox.Show("You cannot save an empty model!");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            switch (DataHelper.Dialog("Do you want to save the current changes before exiting?"))
            {
                case MessageBoxResult.Yes:
                    if (orderParts.Count > 0)
                    {
                        if (!isPreviouslySaved)
                        {
                            new ModelNameDialog(Connector.GetCurrentModel(), false).Show();
                            isPreviouslySaved = true;
                        }
                        else
                            MessageBox.Show(
                                "Save aborted ! \nThis model has already been saved, you cannot save the same model !");
                    }
                    else
                        MessageBox.Show("You cannot save empty model");
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        private void ModelDetailsBtn_Click(object sender, RoutedEventArgs e)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(this, "Price: " + Connector.GetModelPrice() + " €\nProduction Time: " +
                                              Connector.GetModelProductionTime() + " day(s)", "Model Details");
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Selected_PartsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;
        }

        private void AddToBtn_Click(object sender, RoutedEventArgs e)
        {
            if (modelslb.SelectedIndex != -1)
            {
                string selectedPartName = Partslb.SelectedItem.ToString();
                Connector.AddPart(part);
                MenuItem selected_one = new MenuItem() { Header = selectedPartName };

                if (selectedPartName == "Extras")
                {
                    if (DataHelper.ExtraButtonExists(MenuBtn.Items))
                    {
                        foreach (MenuItem item in MenuBtn.Items)
                        {
                            if (ReferenceEquals(item.Header, "Extras"))
                            {
                                selected_one = item;
                                MenuItem extra = new MenuItem() { Header = modelslb.SelectedItem.ToString() };
                                extra.Click += new RoutedEventHandler(this.Selected_Item_Remove);
                                selected_one.Items.Add(extra);
                            }
                        }
                    }
                    else
                    {
                        MenuItem extra = new MenuItem() { Header = modelslb.SelectedItem.ToString() };
                        extra.Click += new RoutedEventHandler(this.Selected_Item_Remove);
                        selected_one.Items.Add(extra);
                        MenuBtn.Items.Add(selected_one);
                    }
                }
                else
                {
                    selected_one.Click += new RoutedEventHandler(this.Selected_Item_Remove);
                    MenuBtn.Items.Add(selected_one);
                }
                BottomGroupBoxClear(true);
                Connector.EnableButtons();
                int counter = 0;
                foreach (MenuItem m in MenuBtn.Items)
                {
                    if (!(m.Parent is MenuItem) && !ReferenceEquals(m.Header, "Extras"))
                        counter++;
                    if (counter == 8)
                        OrderBtn.IsEnabled = true;
                    else
                        OrderBtn.IsEnabled = false;
                }
                isPreviouslySaved = false;
            }
        }

        private void Selected_Item_Remove(object sender, RoutedEventArgs eventArgs)
        {
            MenuItem selectedMenuItem = (MenuItem)eventArgs.OriginalSource;

            if (selectedMenuItem.Parent is MenuItem)
            {
                MenuItem parent = ((MenuItem)selectedMenuItem.Parent);
                parent.Items.Remove(selectedMenuItem);

                if (parent.Items.Count == 0)
                {
                    MenuBtn.Items.Remove(parent);
                }
            }
            else
            {
                MenuBtn.Items.Remove(selectedMenuItem);
            }

            int partIndex = DataHelper.GetPartIndex(selectedMenuItem, orderParts);
            Connector.RemovePart(orderParts.ElementAt(partIndex));
            parts.Children.RemoveAt(partIndex);
            orderParts.RemoveAt(partIndex);
            device.Content = parts;

            Populate(true);
            int counter = 0;
            foreach (MenuItem mi in MenuBtn.Items)
            {
                if (!(mi.Parent is MenuItem) && !ReferenceEquals(mi.Header, "Extras"))
                    counter++;
                if (counter == 8)
                    OrderBtn.IsEnabled = true;
                else
                    OrderBtn.IsEnabled = false;
            }
            isPreviouslySaved = false;
            GC.Collect();
        }

        private void Cancel_Part_OnClick(object sender, RoutedEventArgs e)
        {
            Connector.EnableButtons();
            parts.Children.RemoveAt(parts.Children.Count - 1);
            device.Content = parts;
            BottomGroupBoxClear(false);
            orderParts.RemoveAt(orderParts.Count - 1);
        }

        private void OrderBtn_OnClick(object sender, RoutedEventArgs e)
        {
            new ModelNameDialog(Connector.GetCurrentModel(), true).ShowDialog();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Refreshes the items in the different listboxes. 
        /// </summary>
        /// <param name="flag">Boolean used to deselect a part if not needed in the model.</param>
        public void Populate(bool flag)
        {
            if (!flag)
                modelslb.SelectedIndex = -1;
            else
            {
                Partslb.Items.Clear();
                modelslb.Items.Clear();

                foreach (string item in ApplicationSettings.mainItems)
                {
                    Partslb.Items.Add(item);
                }
            }

            if (MenuBtn.Items != null)
                foreach (MenuItem v in MenuBtn.Items)
                {
                    if (v.Header.ToString() != "Extras")
                        if (Partslb.Items.Contains(v.Header.ToString()))
                            Partslb.Items.Remove(v.Header.ToString());
                }
        }

        /// <summary>
        /// Clears the information for the selected part in the form.
        /// </summary>
        /// <param name="flag">Used as a delegate for the Populate method.</param>
        private void BottomGroupBoxClear(bool flag)
        {
            Description_Photo.Source = null;
            Descriprion_Header.Text = null;
            Description_Text.Text = null;
            AddToBtn.Visibility = Visibility.Hidden;
            Cancel_Part.Visibility = Visibility.Hidden;
            Populate(flag);
        }

        /// <summary>
        /// Checks if already tires/rims are added it makes so that you cannot choose
        /// bigger rims/tires than the size of tires/rims that you have 
        /// </summary>
        /// <param name="toBeRestricted">Category of which to restrict tires/rims</param>
        /// <param name="alreadyExisting">Category of which are already added tires/rims</param>
        private void RestrictTiresOrRims(string toBeRestricted, string alreadyExisting)
        {
            if (MenuBtn.Items != null)
            {
                foreach (MenuItem m in MenuBtn.Items)
                {
                    if (m.Header.ToString() == alreadyExisting)
                    {
                        CarPart existing = orderParts.ElementAt(DataHelper.GetPartIndex(m, orderParts));
                        if (existing.Name == "Fourteen inch " + alreadyExisting)
                        {
                            modelslb.Items.Remove("16 " + toBeRestricted);
                            modelslb.Items.Remove("18 " + toBeRestricted);
                        }
                        else if (existing.Name == "Sixteen inch " + alreadyExisting)
                        {
                            modelslb.Items.Remove("14 " + toBeRestricted);
                            modelslb.Items.Remove("18 " + toBeRestricted);
                        }
                        else if (existing.Name == "Eighteen inch " + alreadyExisting)
                        {
                            modelslb.Items.Remove("14 " + toBeRestricted);
                            modelslb.Items.Remove("16 " + toBeRestricted);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads description, header and icon for a selected part.
        /// </summary>
        /// <param name="partName"> The name of the part that must be described.</param>
        /// <param name="carPart"> A CarPart object used for obtaining the description.</param>
        private void BottomGroupBoxLoader(string partName, Assemble.me.Library.Parts.CarPart carPart)
        {
            Descriprion_Header.Text = carPart.Name + " Price : " + carPart.Price + " Time: " + carPart.ProductionTime + " Days";
            Description_Text.Text = carPart.Description;
            orderParts.Add(carPart);
            Description_Photo.Source = new BitmapImage(new Uri("icos/" + partName + ".png", UriKind.Relative));
            AddToBtn.Visibility = Visibility.Visible;
            Cancel_Part.Visibility = Visibility.Visible;
            viewport3D.CameraController.ResetCamera();
        }


        /// <summary>
        /// Method that loads a downloaded model.
        /// </summary>
        /// <param name="partCategory">Represents the type of the part.</param>
        /// <param name="partName"> represents the name of the part.</param>
        public void LoadDownloadedModel(string partCategory, string partName, ChassisColors color)
        {
            var model = Connector.LoadModel(partName, out part, color);
            parts.Children.Add(model);
            device.Content = parts;
            viewport3D.CameraController.ResetCamera();
            viewport3D.RotateGesture = new MouseGesture(MouseAction.LeftClick);
            isPreviouslySaved = false;
        }

        /// <summary>
        /// Updates the MenuItems containing the already selected parts for the model.
        /// </summary>
        public void UpdateRemovePartsMenu()
        {
            orderParts = new List<CarPart>();

            foreach (CarPart p in Connector.GetCurrentModel().GetAllParts())
            {
                string category = DataHelper.GetPartCategory(p);
                if (category == "Extras")
                {
                    if (!DataHelper.ExtraButtonExists(MenuBtn.Items))
                    {
                        MenuItem extra = new MenuItem() { Header = category };
                        MenuBtn.Items.Add(extra);
                    }
                    foreach (MenuItem item in MenuBtn.Items)
                    {
                        if (ReferenceEquals(item.Header, "Extras"))
                        {
                            MenuItem extra = new MenuItem() { Header = p.Name };
                            extra.Click += new RoutedEventHandler(this.Selected_Item_Remove);
                            item.Items.Add(extra);
                        }
                    }
                }
                else
                {
                    MenuItem part = new MenuItem() { Header = category };
                    MenuBtn.Items.Add(part);
                    part.Click += new RoutedEventHandler(this.Selected_Item_Remove);
                }
                orderParts.Add(p);
            }
        }

        /// <summary>
        /// Resets the screen to its initial state and empties the car model.
        /// </summary>
        public void ClearScreen()
        {
            device.Content = null;
            parts.Children.Clear();
            MenuBtn.Items.Clear();
            modelslb.Items.Clear();
            Connector.NewModel();
            orderParts.Clear();
            OrderBtn.IsEnabled = false;
        }
        #endregion

    }
}
