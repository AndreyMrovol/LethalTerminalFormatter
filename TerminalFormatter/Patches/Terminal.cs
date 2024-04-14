using System.Linq;
using System.Text;
using HarmonyLib;

namespace TerminalFormatter
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatches
    {
        // [HarmonyPrefix]
        // [HarmonyPatch(nameof(Terminal.ParsePlayerSentence))]
        // static void CustomParser(ref Terminal __instance, ref TerminalNode __result)
        // {
        //     string text = __instance.screenText.text.Substring(
        //         __instance.screenText.text.Length - __instance.textAdded
        //     );
        //     CommandParser.ParseCommand(text, ref __instance, ref __result);
        // }

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

            // Settings.RegisteredNodes.ForEach(terminalnode =>
            // {
            //     Plugin.logger.LogDebug($"Node: {terminalnode.name}");
            //     terminalnode.terminalNode.ForEach(terminalNode =>
            //     {
            //         Plugin.logger.LogDebug(
            //             $"{terminalNode}, contains: {node.name.Contains(terminalNode)} {terminalNode.Contains(node.name)}"
            //         );
            //     });
            // });

            // check if node.name contains any of TerminalFormatterNode.terminalNode strings
            TerminalFormatterNode currentNode = Settings
                .RegisteredNodes.Where(formatterNode =>
                    formatterNode.terminalNode.Any(y => node.name.Contains(y))
                )
                .FirstOrDefault();

            if (currentNode != null)
            {
                bool shouldRun = currentNode.IsNodeValid(node, __instance);
                if (!shouldRun)
                {
                    return;
                }

                Plugin.logger.LogWarning($"Found node: {currentNode.name}");

                newDisplayText = currentNode.GetNodeText(node, __instance);
            }
            else
            {
                return;
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

            Plugin.logger.LogDebug("First use: " + Settings.firstUse);

            if (Settings.firstUse && Variables.ISLLLActive)
            {
                LLLCompatibility.Init();
            }
        }
    }
}
