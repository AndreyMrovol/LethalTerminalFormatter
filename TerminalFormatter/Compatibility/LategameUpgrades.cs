using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using AdvancedCompany.Config;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using LethalLevelLoader;
using MoreShipUpgrades;
using MoreShipUpgrades.Managers;
using MoreShipUpgrades.Misc.TerminalNodes;
using UnityEngine;

namespace TerminalFormatter
{
    internal class LategameUpgradesCompatibility
    {
        public static void Init()
        {
            Harmony harmony = new Harmony("TerminalFormatter LGU");

            MethodInfo original = AccessTools.Method(
                typeof(MoreShipUpgrades.Managers.UpgradeBus),
                "ConstructNode"
            );

            MethodInfo postfix = AccessTools.Method(
                typeof(LategameUpgradesCompatibility),
                "ConstructNodePostfix"
            );

            harmony.Patch(original, postfix: new HarmonyMethod(postfix));
        }

        internal static List<CustomTerminalNode> RefreshNodes(
            MoreShipUpgrades.Managers.UpgradeBus upgradeBusInstance
        )
        {
            // we need to get the nodes list from MoreShipUpgrades using reflection cause it's internal

            var field = typeof(MoreShipUpgrades.Managers.UpgradeBus).GetField(
                "terminalNodes",
                BindingFlags.NonPublic | BindingFlags.Instance
            );

            if (field == null)
            {
                Plugin.logger.LogError("Could not find terminalNodes field in MoreShipUpgrades");
                return null;
            }

            return (List<CustomTerminalNode>)field.GetValue(upgradeBusInstance);
        }

        internal static void ConstructNodePostfix(
            ref TerminalNode __result,
            ref UpgradeBus __instance
        )
        {
            Plugin.logger.LogWarning("ConstructNodePostfix called");

            TerminalNode modStoreInterface = ScriptableObject.CreateInstance<TerminalNode>();
            modStoreInterface.clearPreviousText = true;

            var table = new ConsoleTables.ConsoleTable("Name", "Price", "Level");
            var adjustedTable = new StringBuilder();

            string headerName = "LATEGAME UPGRADES STORE";
            string storeHeader = new Header().CreateHeaderWithoutLines(headerName, 4);
            adjustedTable.Append(storeHeader);

            List<CustomTerminalNode> nodes = RefreshNodes(__instance);

            foreach (CustomTerminalNode terminalNode in nodes)
            {
                int salePercent = (int)(
                    terminalNode.salePerc < 1f ? (1 - terminalNode.salePerc) * 100 : 0
                );

                int itemNameWidth = Nodes.itemNameWidth;
                string name = terminalNode.Name.PadRight(itemNameWidth);
                string discountPercent = salePercent > 0 ? $" -{salePercent}%" : "";

                string itemNameWithDiscount = name;
                string shortenedText = " ... ";

                if (name.Length + discountPercent.Length > itemNameWidth)
                {
                    itemNameWithDiscount =
                        name.Substring(
                            0,
                            itemNameWidth - 4 - discountPercent.Length - shortenedText.Length
                        )
                        + shortenedText
                        + discountPercent;
                }
                else
                {
                    itemNameWithDiscount =
                        $"{name.PadRight(itemNameWidth - discountPercent.Length)}{discountPercent}".PadRight(
                            itemNameWidth
                        );
                }

                // string upgradeDotsDisplay = "";
                // int upgradeDotsCount = terminalNode.MaxUpgrade;
                // int upgradeDotsCountCurrent = terminalNode.CurrentUpgrade;




                // after trying to get this shit working for 3 hours, i give up
                // i still don't understand how it would even work lol

                // for (int i = 0; i < upgradeDotsCount; i++)
                // {
                //     // make the code work
                //     // [Warning:TerminalFormatter] Name: Bigger Lungs, Upgrades: 3/3, Diff: 0, Dots: ○●●●
                //     // so this doesn't happen
                //     // we want to create as many dots as MaxUpgrade
                //     // if MaxUpgrade is 0, make one dot
                //     // fill as many dots as CurrentUpgrade


                //     if (i == terminalNode.MaxUpgrade && i == 0)
                //     {
                //         upgradeDotsDisplay += "○";
                //     }

                //     if (i < terminalNode.CurrentUpgrade)
                //     {
                //         upgradeDotsDisplay += "●";
                //     }
                //     else
                //     {
                //         upgradeDotsDisplay += "○";
                //     }
                // }

                // // for every upgrade, add a filled dot
                // for (int i = 1; i <= terminalNode.CurrentUpgrade; i++)
                // {
                //     upgradeDotsDisplay += "●";
                // }

                // for (int i = 0; i <= terminalNode.MaxUpgrade - terminalNode.CurrentUpgrade; i++)
                // {
                //     upgradeDotsDisplay += "○";
                // }



                int currentLevel = terminalNode.Unlocked ? terminalNode.CurrentUpgrade + 1 : 0;
                int remainingLevels = terminalNode.Unlocked ? 0 : 1;
                remainingLevels +=
                    terminalNode.MaxUpgrade != 0
                        ? terminalNode.MaxUpgrade - terminalNode.CurrentUpgrade
                        : 0;
                string upgradeDotsDisplay =
                    new string('●', currentLevel) + new string('○', remainingLevels);

                // Plugin.logger.LogWarning(
                //     $"Name: {terminalNode.Name}, Upgrades: {terminalNode.CurrentUpgrade}/{terminalNode.MaxUpgrade}, Diff: {terminalNode.MaxUpgrade - terminalNode.CurrentUpgrade}, Dots: {upgradeDotsDisplay}"
                // );

                if (!terminalNode.Unlocked)
                {
                    table.AddRow(
                        itemNameWithDiscount,
                        $"${(int)(terminalNode.UnlockPrice * terminalNode.salePerc)}",
                        upgradeDotsDisplay
                    );
                }
                else if (terminalNode.MaxUpgrade == 0)
                {
                    table.AddRow(itemNameWithDiscount, $"", "●");
                }
                else if (terminalNode.MaxUpgrade > terminalNode.CurrentUpgrade)
                {
                    // string upgradeDotsDisplay = "";
                    // for (int i = 0; i <= terminalNode.CurrentUpgrade; i++)
                    // {
                    //     upgradeDotsDisplay += "●";
                    // }

                    // for (int i = terminalNode.CurrentUpgrade; i <= terminalNode.MaxUpgrade; i++)
                    // {
                    //     upgradeDotsDisplay += "○";
                    // }

                    table.AddRow(
                        itemNameWithDiscount,
                        $"${(int)(terminalNode.Prices[terminalNode.CurrentUpgrade] * terminalNode.salePerc)}",
                        upgradeDotsDisplay
                    );
                }
                else if (terminalNode.MaxUpgrade == terminalNode.CurrentUpgrade)
                {
                    table.AddRow(itemNameWithDiscount, "", upgradeDotsDisplay);
                }
            }

            if (nodes.Count == 0)
            {
                table.AddRow("No upgrades available", "", "");
            }

            adjustedTable.Append(table.ToStringCustomDecoration(header: true));
            modStoreInterface.displayText = adjustedTable.ToString();

            __result = modStoreInterface;
        }
    }
}
