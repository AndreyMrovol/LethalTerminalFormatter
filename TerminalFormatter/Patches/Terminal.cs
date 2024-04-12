using System.Linq;
using System.Text;
using HarmonyLib;

namespace TerminalFormatter
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatches
    {
        public static readonly int terminalWidth = 48;
        public static bool firstUse = true;

        [HarmonyPostfix]
        [HarmonyPatch("TextPostProcess")]
        public static void TextPostProcessPrefix(
            string modifiedDisplayText,
            TerminalNode node,
            Terminal __instance
        )
        {
            Plugin.logger.LogDebug(node.name);

            string newDisplayText = null;

            if (node.name == "0_StoreHub")
            {
                newDisplayText = new Nodes().Store(node, __instance);
            }

            if (
                node.name == "MoonsCatalogue"
                || node.name.Contains("preview")
                || node.name.Contains("filter")
                || node.name.Contains("sort")
            )
            {
                if (!Plugin.isLLLPresent)
                {
                    newDisplayText = new Nodes().MoonsNoLLL(node, __instance);
                    // return;
                }
                else
                {
                    newDisplayText = new Nodes().Moons(node, __instance);
                    firstUse = false;
                }
            }

            if (node.name == "ScanInfo")
            {
                newDisplayText = new Nodes().Scan(node, __instance);
            }

            if (Plugin.isLLLPresent)
            {
                if (node.name.ToLower().Contains("route") && node.buyRerouteToMoon == -2)
                {
                    newDisplayText = new Nodes().Route(node, __instance);
                }

                if (node.name.ToLower().Contains("simulate"))
                {
                    newDisplayText = new Nodes().Simulate(node, __instance);
                }
            }

            if (newDisplayText != null)
            {
                StringBuilder builder = new StringBuilder();

                if (__instance.displayingPersistentImage)
                {
                    builder.Append("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                }

                builder.Append("\n\n");
                builder.Append(newDisplayText);
                builder.Append("\n--------------------\n");

                Plugin.logger.LogMessage("New display text:\n" + newDisplayText);

                __instance.screenText.text = builder.ToString();
                __instance.currentText = builder.ToString();
                __instance.textAdded = 0;
            }

            return;
        }

        [HarmonyPostfix]
        // [HarmonyPriority(Priority.Last)]
        [HarmonyPatch("LoadNewNode")]
        public static void StartPostfix(Terminal __instance)
        {
            Variables.BuyableItemList = __instance.buyableItemsList.ToList();
            Variables.UnlockableItemList = StartOfRound
                .Instance.unlockablesList.unlockables.Where(x =>
                    x.unlockableType == 1 && x.alwaysInStock == true
                )
                .ToList();
            Variables.DecorationsList = __instance.ShipDecorSelection;

            if (Variables.IsACActive)
            {
                ACCompatibility.Refresh();
            }

            Plugin.logger.LogDebug("First use: " + firstUse);

            if (firstUse && Variables.ISLLLActive)
            {
                LLLCompatibility.Init();
            }
        }
    }
}
