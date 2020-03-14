using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assemble.me
{
    public class Path
    {
        private static string dir = "../../3dsModels/";
        // Chassis
        public static string SportsCar_Blue { get { return dir + "Chassis/Sport_Blue.3ds"; } }
        public static string SportsCar_Gray { get { return dir + "Chassis/Sport_Gray.3ds"; } }
        public static string SportsCar_Red { get { return dir + "Chassis/Sport_Red.3ds"; } }
        public static string Jeep_Red { get { return dir + "Chassis/Jeep_Red.3ds"; } }
        public static string Jeep_Blue { get { return dir + "Chassis/Jeep_Blue.3ds"; } }
        public static string Jeep_Gray { get { return dir + "Chassis/Jeep_Gray.3ds"; } }
        public static string Sedan_Red { get { return dir + "Chassis/Sedan_Red.3ds"; } }
        public static string Sedan_Blue { get { return dir + "Chassis/Sedan_Blue.3ds"; } }
        public static string Sedan_Gray { get { return dir + "Chassis/Sedan_Gray.3ds"; } }
        public static string Minivan_Red { get { return dir + "Chassis/Minivan_Red.3ds"; } }
        public static string Minivan_Blue { get { return dir + "Chassis/Minivan_Blue.3ds"; } }
        public static string Minivan_Gray { get { return dir + "Chassis/Minivan_Gray.3ds"; } }
        public static string Cabrio_Red { get { return dir + "Chassis/Cabrio_Red.3ds"; } }
        public static string Cabrio_Blue { get { return dir + "Chassis/Cabrio_Blue.3ds"; } }
        public static string Cabrio_Gray { get { return dir + "Chassis/Cabrio_Gray.3ds"; } }

        // Tires
        public static string Tires14 { get { return dir + "Tires/14.3ds"; } }
        public static string Tires16 { get { return dir + "Tires/16.3ds"; } }
        public static string Tires18 { get { return dir + "Tires/18.3ds"; } }

        // Interior
        public static string LeatherInterior { get { return dir + "Interior/Leather.3ds"; } }
        public static string TextileInterior { get { return dir + "Interior/Textile.3ds"; } }

        // Engine
        public static string DieselEngine { get { return dir + "Engine/Diesel.3ds"; } }
        public static string PetrolEngine { get { return dir + "Engine/Petrol.3ds"; } }
        public static string ElectricEngine { get { return dir + "Engine/Electric.3ds"; } }
    }
}
