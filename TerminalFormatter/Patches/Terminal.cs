using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using LethalLevelLoader;
using UnityEngine;

namespace TerminalFormatter
{
    [HarmonyPatch(typeof(Terminal))]
    public class TerminalPatches
    {
        public static readonly int terminalWidth = 48;

        [HarmonyPostfix]
        [HarmonyPatch("TextPostProcess")]
        public static void TextPostProcessPrefix(
            string modifiedDisplayText,
            TerminalNode node,
            Terminal __instance
        )
        {
            Plugin.logger.LogMessage(node.name);

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
                newDisplayText = new Nodes().Moons(node, __instance);
            }

            if (newDisplayText != null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("\n\n");
                builder.Append(newDisplayText);
                builder.Append("\n-------------------- \n");

                Plugin.logger.LogMessage("New display text:\n" + newDisplayText);

                __instance.screenText.text = builder.ToString();
                __instance.currentText = builder.ToString();
                __instance.textAdded = 0;
            }

            return;
        }
    }
}
