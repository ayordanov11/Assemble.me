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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Xceed.Wpf.Toolkit;

namespace Assemble.me
{
    public class DataHelper
    {
        /// <summary>
        /// Retrieves tha string representation of a part.
        /// </summary>
        /// <param name="part">Part for which the name has to be determined.</param>
        /// <returns>Name of the part as a string.</returns>
        public static string GetPartName(CarPart part)
        {
            // Chassis
            if (part is SportCar)
                return "Sport";
            else if (part is Jeep)
                return "Jeep";
            else if (part is Minivan)
                return "Minivan";
            else if (part is Sedan)
                return "Sedan";
            // Tires
            else if (part is Tires)
            {
                if (((Tires)part).Inches == Inches.Fourteen)
                    return "14 Tires";
                else if (((Tires)part).Inches == Inches.Sixteen)
                    return "16 Tires";
                else if (((Tires)part).Inches == Inches.Eighteen)
                    return "18 Tires";
                else
                    throw new InvalidOperationException("Invalid tires.");
            }
            // Rims
            else if (part is Rims)
            {
                if(((Rims)part).Tires.Inches == Inches.Fourteen)
                    return "14 Rims";
                else if (((Rims)part).Tires.Inches == Inches.Sixteen)
                    return "14 Rims";
                else if (((Rims)part).Tires.Inches == Inches.Eighteen)
                    return "14 Rims";
                else
                    throw new InvalidOperationException("Invalid rims.");
            }
            // Accumulator
            else if (part is Accumulator)
            {
                if(((Accumulator)part).Size == AccumulatorSize.Small)
                    return "Small";
                else if (((Accumulator) part).Size == AccumulatorSize.Average)
                    return "Average";
                else if (((Accumulator) part).Size == AccumulatorSize.MegaPower)
                    return "MegaPower";
                else 
                    throw new InvalidOperationException("Invalid accumulator.");
            }
            // Transmission
            else if (part is Transmission)
            {
                if (part is ManualTransmission)
                    return "Manual";
                else if (part is AutomaticTransmission)
                    return "Automatic";
                else
                    throw new InvalidOperationException("Invalid transmission.");
            }
            // Suspension
            else if (part is Suspension)
            {
                if (part is HydraulicSuspension)
                    return "Hydraulic";
                else if (part is GasSuspension)
                    return "Gas";
                else if (part is AirSuspension)
                    return "Air";
                else
                    throw new InvalidOperationException("Invalid suspension.");
            }
            // Interior
            else if (part is Interior)
            {
                if (part is LeatherInterior)
                    return "Leather";
                else if (part is TextileInterior)
                    return "Textile";
                else
                    throw new InvalidOperationException("Invalid interior.");
            }
            // Engine
            else if (part is Engine)
            {
                if (part is DieselEngine)
                    return "Diesel";
                else if (part is PetrolEngine)
                    return "Petrol";
                else if (part is ElectricEngine)
                    return "Electric";
                else
                    throw new InvalidOperationException("Invalid engine.");
            }
            //Extras
            else if (part is Extra)
            {
                if (part is _4x4)
                    return "4x4";
                else if (part is AC)
                    return "AC";
                else if (part is AudioSystem)
                    return "Audio System";
                else if (part is ButtonStarter)
                    return "Button Starter";
                else if (part is CruiseControl)
                    return "Cruise Control";
                else if (part is GPS)
                    return "GPS";
                else if (part is Parktronic)
                    return "Parktronic";
                else
                    throw new InvalidOperationException("Invalid extra.");
            }
            else
                throw new InvalidOperationException("Invalid part.");
        }

        /// <summary>
        /// Retrieves a string representation of the category of the specified <paramref name="part"/>.
        /// </summary>
        /// <param name="part">The part for which the category has to be determined.</param>
        /// <returns>The category of the part as a string.</returns>
        public static string GetPartCategory(CarPart part)
        {
            if (part is Chassis)
                return "Chassis";
            else if (part is Engine)
                return "Engine";
            else if (part is Suspension)
                return "Suspension";
            else if (part is Extra)
                return "Extras";
            else if (part is Transmission)
                return "Transmission";
            else if (part is Tires)
                return "Tires";
            else if (part is Rims)
                return "Rims";
            else if (part is Interior)
                return "Interior";
            else if (part is Accumulator)
                return "Accumulator";
            else
                throw new InvalidOperationException("Part category is not supproted.");
        }

        /// <summary>
        /// Generates a popup success message.
        /// </summary>
        /// <param name="message">The message to be displayed to the user.</param>
        public static void Success(string message)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(message);
        }

        /// <summary>
        /// Generates a popup fail message.
        /// </summary>
        /// <param name="message">The message to be displayed to the user.</param>
        public static void Fail(string message)
        {
            Xceed.Wpf.Toolkit.MessageBox.Show(message);
        }

        /// <summary>
        /// Generates a dialog form for an action.
        /// </summary>
        /// <param name="action">The message to the user.</param>
        /// <returns>Result of the user interaction.</returns>
        public static  MessageBoxResult Dialog(string action)
        {
            string caption = "Action decider";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;
            return Xceed.Wpf.Toolkit.MessageBox.Show(action, caption, button, icon);
        }

        /// <summary>
        /// Maps human-readable strings to car parts.
        /// </summary>
        /// <param name="partName">The name of the part with words.</param>
        /// <returns>The meaning of the string as a car part.</returns>
        public static CarPart GetPartByString(string partName)
        {
            if (partName == "Jeep")
                return new Jeep();
            else if (partName == "Sport")
                return new SportCar();
            else if (partName == "Sedan")
                return new Sedan();
            else if (partName == "Minivan")
                return new Minivan();
            else if (partName == "14 Tires")
                return new Tires(Inches.Fourteen);
            else if (partName == "16 Tires")
                return new Tires(Inches.Sixteen);
            else if (partName == "18 Tires")
                return new Tires(Inches.Eighteen);
            else if (partName == "14 Rims")
                return new Rims(new Tires(Inches.Fourteen));
            else if (partName == "16 Rims")
                return new Rims(new Tires(Inches.Sixteen));
            else if (partName == "18 Rims")
                return new Rims(new Tires(Inches.Eighteen));
            else if (partName == "Leather")
                return new LeatherInterior();
            else if (partName == "Textile")
                return new TextileInterior();
            else if (partName == "Diesel")
                return new DieselEngine();
            else if (partName == "Petrol")
                return new PetrolEngine();
            else if (partName == "Air")
                return new AirSuspension();
            else if (partName == "Gas")
                return new GasSuspension();
            else if (partName == "Hydraulic")
                return new HydraulicSuspension();
            else if (partName == "Manual")
                return new ManualTransmission();
            else if (partName == "Automatic")
                return new AutomaticTransmission();
            else if (partName == "Small")
                return new Accumulator(AccumulatorSize.Small);
            else if (partName == "Average")
                return new Accumulator(AccumulatorSize.Average);
            else if (partName == "MegaPower")
                return new Accumulator(AccumulatorSize.MegaPower);
            else if (partName == "4x4")
                return new _4x4();
            else if (partName == "AC")
                return new AC();
            else if (partName == "Audio System")
                return new AudioSystem();
            else if (partName == "Button Starter")
                return new ButtonStarter();
            else if (partName == "Cruise Control")
                return new CruiseControl();
            else if (partName == "GPS")
                return new GPS();
            else if (partName == "Parktronic")
                return new Parktronic();
            else
                return null;
        }

        /// <summary>
        /// Gets a chassis color from an enum by processing its string representation.
        /// </summary>
        /// <param name="color">Color name as a string.</param>
        /// <returns>The color as an object of ChassisColor.</returns>
        public static ChassisColors GetChassisColor(string color)
        {
            if (color == "Red")
                return ChassisColors.Red;
            else if (color == "Blue")
                return ChassisColors.Blue;
            else if (color == "Grey")
                return ChassisColors.Grey;
            else
                return ChassisColors.Grey;
        }

        /// <summary>
        /// Gets the index of the already inserted to the model part.
        /// </summary>
        /// <param name="selected">A part selected i na menu.</param>
        /// <param name="orderParts">List of parts </param>
        /// <returns>Number representing the index of the part.</returns>
        public static int GetPartIndex(MenuItem selected, List<CarPart> orderParts)
        {
            foreach (CarPart p in orderParts)
            {
                string cat = "";
                if (p is Chassis)
                    cat = "Chassis";
                else if (p is Accumulator)
                    cat = "Accumulator";
                else if (p is Suspension)
                    cat = "Suspension";
                else if (p is Engine)
                    cat = "Engine";
                else if (p is Interior)
                    cat = "Interior";
                else if (p is _4x4)
                    cat = "4x4";
                else if (p is CruiseControl)
                    cat = "Cruise Control";
                else if (p is Transmission)
                    cat = "Transmission";
                else if (p is Suspension)
                    cat = "Suspension";
                else if (p is AudioSystem)
                    cat = "Audio System";
                else if (p is ButtonStarter)
                    cat = "Button Starter";
                else if (p is CruiseControl)
                    cat = "Cruise Control";
                else if (p is Tires)
                    cat = "Tires";
                else if (p is Rims)
                    cat = "Rims";
                else if (p is Parktronic)
                    cat = "Parktronic";
                else if (p is GPS)
                    cat = "GPS";
                else if (p is AC)
                    cat = "AC";
                else
                    cat = p.GetType().Name.ToString();

                if (selected.Header.ToString() == cat)
                    return orderParts.IndexOf(p);
            }
            return 0;
        }

        /// <summary>
        /// Checks if there is an extra attached to the model.
        /// </summary>
        /// <returns></returns>
        public static bool ExtraButtonExists(ItemCollection menuItems)
        {
            foreach (MenuItem item in menuItems)
            {
                if (ReferenceEquals(item.Header, "Extras"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the part names by type of the part.
        /// </summary>
        /// <param name="type">The type of the part.</param>
        /// <returns></returns>
        public static List<string> GetPartNamesByType(string type)
        {
            List<string> temp = new List<string>();
            if (type == "Chassis")
            {
                temp.Add("Jeep");
                temp.Add("Sport");
                temp.Add("Sedan");
                temp.Add("Minivan");
                temp.Add("Cabrio");
            }
            else if (type == "Tires")
            {
                temp.Add("14 Tires");
                temp.Add("16 Tires");
                temp.Add("18 Tires");
            }
            else if (type == "Rims")
            {
                temp.Add("14 Rims");
                temp.Add("16 Rims");
                temp.Add("18 Rims");
            }
            else if (type == "Interior")
            {
                temp.Add("Leather");
                temp.Add("Textile");
            }
            else if (type == "Engine")
            {
                temp.Add("Diesel");
                temp.Add("Petrol");
                temp.Add("Electric");
            }
            else if (type == "Suspension")
            {
                temp.Add("Air");
                temp.Add("Gas");
                temp.Add("Hydraulic");
            }
            else if (type == "Transmission")
            {
                temp.Add("Manual");
                temp.Add("Automatic");
            }
            else if (type == "Extras")
            {
                temp.Add("4x4");
                temp.Add("AC");
                temp.Add("Audio System");
                temp.Add("Button Starter");
                temp.Add("Cruise Control");
                temp.Add("GPS");
                temp.Add("Parktronic");
            }
            else if (type == "Accumulator")
            {
                temp.Add("Small");
                temp.Add("Average");
                temp.Add("MegaPower");
            }

            return temp;
        }
    }
}
