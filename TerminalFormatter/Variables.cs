using System.Collections.Generic;

namespace TerminalFormatter
{
    public class Variables
    {
        public static List<Item> BuyableItemList = [];
        public static List<UnlockableItem> UnlockableItemList = [];
        public static List<TerminalNode> DecorationsList = [];

        public static List<TerminalNode> Nodes = [];

        public static List<BuyableThing> Buyables = [];
        public static List<Route> Routes = [];
        public static List<BuyableCar> Vehicles = [];

        public static Terminal Terminal;

        public static bool IsACActive = false;
        public static bool ISLLLActive = false;

        public static Dictionary<string, int> upgrades =
            new()
            {
                { "Teleporter", 375 },
                { "Signal translator", 255 },
                { "Loud horn", 100 },
                { "Inverse Teleporter", 425 },
            };

        // public static List<
    }
}
