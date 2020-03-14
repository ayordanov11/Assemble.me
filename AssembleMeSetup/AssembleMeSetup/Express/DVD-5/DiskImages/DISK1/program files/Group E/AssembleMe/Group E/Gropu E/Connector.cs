using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Assemble.me.Library;
using Assemble.me.Library.PackageCustomer;
using Assemble.me.Library.PackageOrder;
using Assemble.me.Library.Parts;
using Assemble.me.Library.Parts.PackageAccumulator;
using Assemble.me.Library.Parts.PackageChassis;
using Assemble.me.Library.Parts.PackageEngine;
using Assemble.me.Library.Parts.PackageExtra;
using Assemble.me.Library.Parts.PackageInterrior;
using Assemble.me.Library.Parts.PackageRims;
using Assemble.me.Library.Parts.PackageSuspension;
using Assemble.me.Library.Parts.PackageTires;
using Assemble.me.Library.Parts.PackageTransmission;
using HelixToolkit.Wpf;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.Toolkit;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;
using System.IO;
using System.Runtime.InteropServices;
using Dropbox.Api;

namespace Assemble.me
{
    public class Connector
    {
        private static CustomerClient client = new CustomerClient();
        private static Window1 mainWindow;
        private static DropboxClient dropBoxClient = new DropboxClient("khJDxv0h3M4AAAAAAAAHujvHGMs7MwehZEDaxyKHCdjHk2S9IcSR8jRsO3dEy1Oo");

        public Connector()
        {
        }

        /// <summary>
        /// Adds a part to the model.
        /// </summary>
        /// <param name="part"></param>
        public static void AddPart(CarPart part)
        {
            client.AddPartToModel(part);
        }

        /// <summary>
        /// Verifies if the email is not already taken.
        /// </summary>
        /// <param name="email">The email that must be checked.</param>
        /// <returns></returns>
        public static bool VerifyEmail(string email)
        {
            return client.CheckIfEmailIsTaken(email);
        }

        /// <summary>
        /// Removes a part from the current model.
        /// </summary>
        /// <param name="part">The car part that must be removed.</param>
        public static void RemovePart(CarPart part)
        {
            client.RemovePartFromModel(part);
        }

        /// <summary>
        /// Gets the current model.
        /// </summary>
        /// <returns></returns>
        public static CarModel GetCurrentModel()
        {
            return client.GetCurrentModel();
        }

        /// <summary>
        /// Places an order for the current model.
        /// </summary>
        /// <param name="customer">The customer to which the order belongs.</param>
        /// <param name="modelName">The name of the model.</param>
        /// <param name="priority">The priority of the model.  Default is Normal.</param>
        public static void Order(Customer customer, string modelName, OrderPriority p=OrderPriority.Normal)
        {
            try
            {
                client.GetCurrentModel().SetName(modelName);
                client.OrderModel(customer, client.GetCurrentModel(),p);
                DataHelper.Success("Order was placed successfully!");
            }
            catch (Exception exc)
            {
                DataHelper.Fail(exc.Message);
            }

        }

        /// <summary>
        /// The current model is changed with a new empty one.
        /// </summary>
        public static void NewModel()
        {
            client.NewModel();
        }

        /// <summary>
        /// Calculates the total price for the model.
        /// </summary>
        /// <returns>The calculated total price in string format.</returns>
        public static string GetModelPrice(OrderPriority priority = OrderPriority.Normal)
        {
            if (priority == OrderPriority.High)
                return (client.GetCurrentModel().GetPrice() * 1.25m).ToString();
            return client.GetCurrentModel().GetPrice().ToString();
        }

        /// <summary>
        /// Calculates the total estimated production time for the model.
        /// </summary>
        /// <returns>The total estimated production time in string format.</returns>
        public static string GetModelProductionTime(OrderPriority priority = OrderPriority.Normal)
        {
            if (priority == OrderPriority.High)
                return (client.GetCurrentModel().GetProductionTime() * 0.75).ToString();
            return client.GetCurrentModel().GetProductionTime().ToString();
        }

        /// <summary>
        /// Sets the current customer that uses the application.
        /// </summary>
        /// <param name="customer">The customer object to which the model is related.</param>
        public static void SetCurrentCustomer(Customer customer)
        {
            client.SetCurrentCustomer(customer);
        }

        /// <summary>
        /// Gets the current customer using the application.
        /// </summary>
        /// <returns></returns>
        public static Customer GetCurrentCustomer()
        {
            return client.GetCurrentCustomer();
        }

        /// <summary>
        /// Saves the customer to the database.
        /// </summary>
        public static void SaveCustomer()
        {
            client.GetCurrentCustomer().SaveToDB();
        }

        /// <summary>
        /// Boolean that checks the credentials of a customer. If the email and id belong to one registered customer. It returns true.
        /// </summary>
        /// <param name="email">The email of the customer.</param>
        /// <param name="id">The id of that specific customer.</param>
        /// <returns>True if combination is right,fase if not.</returns>
        public static bool LogIn(string email, int id)
        {
            Customer cust = client.GetCustomer(id);
            if (cust != null)
            {
                if (cust.Email == email)
                {
                    client.SetCurrentCustomer(cust);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// Saves the ids of the parts of a <see cref="CarModel"/> to Dropbox in an .assemble file.
        /// </summary>
        /// <param name="model">The model which should be saved.</param>
        public static async void SaveModelToCloud(CarModel model)
        {
            StreamWriter writer = null;
            try
            {
                DropBoxGui dropbox = new DropBoxGui(dropBoxClient, mainWindow);
                Directory.CreateDirectory("../../3dsModelsTemp");
                string localPath = "../../3dsModelsTemp/" + model.Name + ".assemble";
                writer = new StreamWriter(localPath);

                foreach (CarPart part in model.GetAllParts())
                {
                    if (part is Chassis)
                        writer.WriteLine(part.GetId() + "," + ((Chassis)part).Color);
                    else
                        writer.WriteLine(part.GetId());
                }
                writer.Close();

                // Upload to Dropbox
                string remotePath = @"/" + GetCurrentCustomer().ID + "/" + model.Name + ".assemble";
                await dropbox.CreateDirectory(@"/" + GetCurrentCustomer().ID);
                await dropbox.Upload(localPath, remotePath);
            }
            catch (IOException exc)
            {
                DataHelper.Fail(exc.Message);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Retrieves the file from the specified <paramref name="path"/> in Dropbox and converts it
        /// to a <see cref="CarModel"/>.
        /// </summary>
        /// <param name="path">the path ot the file in Dropbox.</param>
        public static void LoadModelFromCloud(string path, Window1 mainWindow)
        {
            StreamReader reader = null;
            bool readerShoudBeClosed = false;
            try
            {
                reader = new StreamReader(path);
                String line = reader.ReadLine();
                String chasColor="";
                mainWindow.ClearScreen();

                while (line != null)
                {
                    if (line.Contains(","))
                    {
                        string[] chas = line.Split(',');
                        line = chas[0];
                        chasColor = chas[1];
                    }
                    CarPart part = ApplicationSettings.GetPartById(Convert.ToInt32(line));
                    if (part is Chassis)
                        ((Chassis)part).SetChassisColor(DataHelper.GetChassisColor(chasColor));
                    AddPart(part);
                    line = reader.ReadLine();
                }

                List<CarPart> parts = GetCurrentModel().GetAllParts();

                foreach (CarPart p in parts)
                {
                    // The reader should be closed only if the list is looped through
                    if (parts.IndexOf(p) == parts.Count - 1)
                        readerShoudBeClosed = true;

                    if (ShouldBeLoaded(p))
                    {
                        mainWindow.LoadDownloadedModel(DataHelper.GetPartCategory(p), DataHelper.GetPartName(p),DataHelper.GetChassisColor(chasColor));
                    }
                }
                mainWindow.UpdateRemovePartsMenu();
                mainWindow.Populate(true);
            }
            catch (IOException exc)
            {
                DataHelper.Fail(exc.Message);
            }
            finally
            {
                if (reader != null && readerShoudBeClosed)
                    reader.Close();
            }
        }

        /// <summary>
        /// Loads a visual model on the screen.
        /// </summary>
        /// <param name="partName">The name of the part that must be drawn</param>
        /// <param name="part">A cartpart object that represents the drawn part.</param>
        /// <param name="color">The color of the chassis.</param>
        /// <returns>A model object that should be drawn.</returns>
        public static Model3DGroup LoadModel(string partName, out CarPart part, ChassisColors color)
        {
            int angle1 = 0, t1X = 0, t1Y = 0, t1Z = 0, t2X = 0, t2Y = 0, t2Z = 0;
            int angle2 = 0, t3X = 0, t3Y = 0, t3Z = 0, t4X = 0, t4Y = 0, t4Z = 0;
            var tires = new Model3DGroup();
            switch (partName)
            {
                case "18 Tires":
                    {
                        part = new Tires(Inches.Eighteen);
                        angle1 = 180;
                        t1X = -43; // predna dqsna
                        t1Y = 15;
                        t1Z = 0;
                        t2X = -43; // zadna dqsna
                        t2Y = -75;
                        t2Z = 0;
                        angle2 = 0;
                        t3X = 105; // predna lqva
                        t3Y = -13;
                        t3Z = 0;
                        t4X = 105; // zadna lqva
                        t4Y = 80;
                        t4Z = 0;
                        tires.Children.Add(GetModel(Path.Tires18));
                        tires.Children.Add(GetModel(Path.Tires18));
                        tires.Children.Add(GetModel(Path.Tires18));
                        tires.Children.Add(GetModel(Path.Tires18));
                        return LoadingTires(tires, angle1, angle2, t1X, t1Y, t1Z, t2X, t2Y, t2Z, t3X, t3Y, t3Z, t4X,
                            t4Y, t4Z);
                    }
                case "16 Tires":
                    {
                        part = new Tires(Inches.Sixteen);
                        angle1 = 270;
                        t1X = 17;
                        t1Y = 40;
                        t1Z = 1;
                        t2X = -77;
                        t2Y = 40;
                        t2Z = 2;
                        angle2 = 90;
                        t3X = -17;
                        t3Y = -110;
                        t3Z = 1;
                        t4X = 77;
                        t4Y = -110;
                        t4Z = 2;
                        tires.Children.Add(GetModel(Path.Tires16));
                        tires.Children.Add(GetModel(Path.Tires16));
                        tires.Children.Add(GetModel(Path.Tires16));
                        tires.Children.Add(GetModel(Path.Tires16));
                        return LoadingTires(tires, angle1, angle2, t1X, t1Y, t1Z, t2X, t2Y, t2Z, t3X, t3Y, t3Z, t4X, t4Y,
                            t4Z);
                    }
                case "14 Tires":
                    {
                        part = new Tires(Inches.Fourteen);
                        angle1 = 170;
                        t1X = -13; //pedna lqva
                        t1Y = -56;
                        t1Z = 2;
                        t2X = 3; // zadna lqva
                        t2Y = -148;
                        t2Z = 2;
                        angle2 = 350;
                        t3X = 140; // predna dqsna
                        t3Y = -63;
                        t3Z = 2;
                        t4X = 124; // zadna dqsna
                        t4Y = 28;
                        t4Z = 2;
                        tires.Children.Add(GetModel(Path.Tires14));
                        tires.Children.Add(GetModel(Path.Tires14));
                        tires.Children.Add(GetModel(Path.Tires14));
                        tires.Children.Add(GetModel(Path.Tires14));
                        return LoadingTires(tires, angle1, angle2, t1X, t1Y, t1Z, t2X, t2Y, t2Z, t3X, t3Y, t3Z, t4X, t4Y,
                            t4Z);
                    }
                case "Jeep":
                    {
                        part = new Jeep();
                        Model3DGroup jeep = new Model3DGroup();
                        Transform3DGroup jp = new Transform3DGroup();
                        if (color == ChassisColors.Red)
                            jeep.Children.Add(GetModel(Path.Jeep_Red));
                        if (color == ChassisColors.Blue)
                            jeep.Children.Add(GetModel(Path.Jeep_Blue));
                        if (color == ChassisColors.Grey)
                            jeep.Children.Add(GetModel(Path.Jeep_Gray));
                        ((Chassis)part).SetChassisColor(color);
                        jp.Children.Add(new TranslateTransform3D(75, 35, 4));
                        jeep.Children[0].Transform = jp;
                        return jeep;
                    }
                case "Sport":
                    {
                        part = new SportCar();
                        Model3DGroup sport = new Model3DGroup();
                        if (color == ChassisColors.Red)
                            sport.Children.Add(GetModel(Path.SportsCar_Red));
                        if (color == ChassisColors.Blue)
                            sport.Children.Add(GetModel(Path.SportsCar_Blue));
                        if (color == ChassisColors.Grey)
                            sport.Children.Add(GetModel(Path.SportsCar_Gray));
                        ((Chassis)part).SetChassisColor(color);
                        return sport;
                    }
                case "Sedan":
                    {
                        part = new Sedan();
                        Model3DGroup sedan = new Model3DGroup();
                        Transform3DGroup sd = new Transform3DGroup();

                        if (color == ChassisColors.Red)
                            sedan.Children.Add(GetModel(Path.Sedan_Red));
                        if (color == ChassisColors.Blue)
                            sedan.Children.Add(GetModel(Path.Sedan_Blue));
                        if (color == ChassisColors.Grey)
                            sedan.Children.Add(GetModel(Path.Sedan_Gray));

                        ((Chassis)part).SetChassisColor(color);
                        sd.Children.Add(new TranslateTransform3D(75, 32, 0));
                        sedan.Children[0].Transform = sd;
                        return sedan;

                    }
                case "Minivan":
                    {
                        part = new Minivan();
                        Model3DGroup minivan = new Model3DGroup();
                        Transform3DGroup mv = new Transform3DGroup();

                        if (color == ChassisColors.Red)
                            minivan.Children.Add(GetModel(Path.Minivan_Red));
                        if (color == ChassisColors.Blue)
                            minivan.Children.Add(GetModel(Path.Minivan_Blue));
                        if (color == ChassisColors.Grey)
                            minivan.Children.Add(GetModel(Path.Minivan_Gray));

                        ((Chassis)part).SetChassisColor(color);
                        mv.Children.Add(new TranslateTransform3D(74, 29, -3));
                        minivan.Children[0].Transform = mv;
                        return minivan;
                    }
                case "Cabrio":
                    {
                        part = new Cabrio();
                        Model3DGroup cabrio = new Model3DGroup();
                        Transform3DGroup mv = new Transform3DGroup();

                        if (color == ChassisColors.Red)
                            cabrio.Children.Add(GetModel(Path.Cabrio_Red));
                        if (color == ChassisColors.Blue)
                            cabrio.Children.Add(GetModel(Path.Cabrio_Blue));
                        if (color == ChassisColors.Grey)
                            cabrio.Children.Add(GetModel(Path.Cabrio_Gray));
                        mv.Children.Add(new TranslateTransform3D(77, 32, 1));
                        cabrio.Children[0].Transform = mv;
                        return cabrio;
                    }
                case "Leather":
                    {
                        part = new LeatherInterior();
                        Model3DGroup seats = new Model3DGroup();
                        Transform3DGroup st = new Transform3DGroup();
                        seats.Children.Add(GetModel(Path.LeatherInterior));
                        st.Children.Add(new TranslateTransform3D(-5, 43, 7));
                        seats.Children[0].Transform = st;
                        return seats;
                    }
                case "Textile":
                    {
                        part = new TextileInterior();
                        Model3DGroup seats = new Model3DGroup();
                        Transform3DGroup st = new Transform3DGroup();
                        var rotateTransform3D = new RotateTransform3D();
                        var axisAngleRotation3D = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 170);
                        rotateTransform3D.Rotation = axisAngleRotation3D;
                        seats.Children.Add(GetModel(Path.TextileInterior));
                        st.Children.Add(new TranslateTransform3D(-165, -90, 4));
                        st.Children.Add(rotateTransform3D);
                        seats.Children[0].Transform = st;
                        return seats;
                    }
                case "Diesel":
                    {
                        part = new DieselEngine();
                        Model3DGroup engine = new Model3DGroup();
                        Transform3DGroup en = new Transform3DGroup();
                        engine.Children.Add(GetModel(Path.DieselEngine));
                        en.Children.Add(new TranslateTransform3D(80, -15, 0));
                        engine.Children[0].Transform = en;
                        return engine;
                    }
                case "Petrol":
                    {
                        part = new PetrolEngine();
                        Model3DGroup engine = new Model3DGroup();
                        Transform3DGroup en = new Transform3DGroup();
                        engine.Children.Add(GetModel(Path.PetrolEngine));
                        en.Children.Add(new TranslateTransform3D(75, -15, 15));
                        engine.Children[0].Transform = en;
                        return engine;
                    }
                case "Electric":
                    {
                        part = new ElectricEngine();
                        Model3DGroup engine = new Model3DGroup();
                        Transform3DGroup en = new Transform3DGroup();
                        engine.Children.Add(GetModel(Path.ElectricEngine));
                        en.Children.Add(new TranslateTransform3D(65, -8,0));
                        engine.Children[0].Transform = en;
                        return engine;
                    }
                case "Air":
                    {
                        part = new AirSuspension();
                        return new Model3DGroup();
                    }
                case "Gas":
                    {
                        part = new GasSuspension();
                        return new Model3DGroup();
                    }
                case "Hydraulic":
                    {
                        part = new HydraulicSuspension();
                        return new Model3DGroup();
                    }
                case "Manual":
                    {
                        part = new ManualTransmission();
                        return new Model3DGroup();
                    }
                case "Automatic":
                    {
                        part = new AutomaticTransmission();
                        return new Model3DGroup();
                    }
                case "Small":
                    {
                        part = new Accumulator(AccumulatorSize.Small);
                        return new Model3DGroup();
                    }
                case "Average":
                    {
                        part = new Accumulator(AccumulatorSize.Average);
                        return new Model3DGroup();
                    }
                case "MegaPower":
                    {
                        part = new Accumulator(AccumulatorSize.MegaPower);
                        return new Model3DGroup();
                    }
                case "4x4":
                    {
                        part = new _4x4();
                        return new Model3DGroup();
                    }
                case "AC":
                    {
                        part = new AC();
                        return new Model3DGroup();
                    }
                case "Audio System":
                    {
                        part = new AudioSystem();
                        return new Model3DGroup();
                    }
                case "Button Starter":
                    {
                        part = new ButtonStarter();
                        return new Model3DGroup();
                    }
                case "Cruise Control":
                    {
                        part = new CruiseControl();
                        return new Model3DGroup();
                    }
                case "GPS":
                    {
                        part = new GPS();
                        return new Model3DGroup();
                    }
                case "Parktronic":
                    {
                        part = new Parktronic();
                        return new Model3DGroup();
                    }
                case "14 Rims":
                    {
                        part = new Rims(new Tires(Inches.Fourteen));
                        return new Model3DGroup();
                    }
                case "16 Rims":
                    {
                        part = new Rims(new Tires(Inches.Sixteen));
                        return new Model3DGroup();
                    }
                case "18 Rims":
                    {
                        part = new Rims(new Tires(Inches.Eighteen));
                        return new Model3DGroup();
                    }
                default:
                    {
                        part = null;
                        return null;
                    }
            }
        }

        /// <summary>
        /// Draws tires on the screen.
        /// </summary>
        /// <param name="tires">The car model object.</param>
        /// <returns>A model object that should be drawn.</returns>
        private static Model3DGroup LoadingTires(Model3DGroup tires, int angle1, int angle2, int t1X, int t1Y, int t1Z, int t2X, int t2Y, int t2Z, int t3X, int t3Y, int t3Z, int t4X, int t4Y, int t4Z)
        {
            for (int i = 0; i < tires.Children.Count; i++)
            {
                Transform3DGroup tr = new Transform3DGroup();
                var rotateTransform3D = new RotateTransform3D();
                if (i < 2)
                {
                    var axisAngleRotation3D = new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle1);
                    rotateTransform3D.Rotation = axisAngleRotation3D;
                    if (i == 0)
                        tr.Children.Add(new TranslateTransform3D(t1X, t1Y, t1Z));
                    if (i == 1)
                        tr.Children.Add(new TranslateTransform3D(t2X, t2Y, t2Z));
                }
                else if (i >= 2)
                {
                    var axisAngleRotation3D = new AxisAngleRotation3D(new Vector3D(0, 0, 1), angle2);
                    rotateTransform3D.Rotation = axisAngleRotation3D;
                    if (i == 2)
                        tr.Children.Add(new TranslateTransform3D(t3X, t3Y, t3Z));
                    if (i == 3)
                        tr.Children.Add(new TranslateTransform3D(t4X, t4Y, t4Z));
                }
                tr.Children.Add(rotateTransform3D);
                tires.Children[i].Transform = tr;
            }
            return tires;
        }

        /// <summary>
        /// Gets an imported from a file visual object.
        /// </summary>
        /// <param name="path">The path to the visual object.</param>
        /// <returns>Avisual object if found.</returns>
        public static Model3D GetModel(string path)
        {
            Model3D device = null;

            try
            {
                ModelImporter import = new ModelImporter();
                device = import.Load(path);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return device;
        }

        /// <summary>
        /// Handles all window responsiveness and resizes all elements to fit in properly. 
        /// </summary>
        /// <param name="top">Top position.</param>
        /// <param name="bot">Bottom position.</param>
        /// <param name="left">Left position.</param>
        /// <param name="right">Right position</param>
        /// <param name="windowHeigh">The height of the window.</param>
        /// <param name="windowWidth">The width of the window.</param>
        /// <returns></returns>
        public static Thickness Resizer(double top, double bot, double left,
             double right, double windowHeigh, double windowWidth)
        {
            double topPos = windowHeigh * (top / 100);
            double botPos = windowHeigh * (bot / 100);
            double leftPos = windowWidth * (left / 100);
            double rightPos = windowWidth * (right / 100);
            return new Thickness(leftPos, topPos, rightPos, botPos);
        }

        /// <summary>
        /// Assignes a mainwindow to the application.
        /// </summary>
        /// <param name="window"></param>
        public static void SetMainWindow(Window1 window)
        {
            mainWindow = window;
        }

        /// <summary>
        /// Disables user controls while performing an operation that requires it.
        /// </summary>
        public static void DisableButtons()
        {
            mainWindow.Partslb.IsEnabled = false;
            mainWindow.modelslb.IsEnabled = false;
            mainWindow.Selected_PartsBtn.IsEnabled = false;
            mainWindow.Clear_Model.IsEnabled = false;
        }

        /// <summary>
        /// Enables user interactions with user controls that were previously disabled.
        /// </summary>
        public static void EnableButtons()
        {
            mainWindow.Partslb.IsEnabled = true;
            mainWindow.modelslb.IsEnabled = true;
            mainWindow.Selected_PartsBtn.IsEnabled = true;
            mainWindow.Clear_Model.IsEnabled = true;
        }

        #region Helpers
        private static bool ShouldBeLoaded(CarPart part)
        {
            if (part is Chassis || part is Engine || part is Tires || part is Interior 
                || part is Extra || part is Accumulator || part is Rims || part is Transmission 
                || part is Suspension)
                return true;
            else
                return false;
        }
        #endregion
    }
}
