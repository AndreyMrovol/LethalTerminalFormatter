using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using MoreShipUpgrades;
using MoreShipUpgrades.Managers;
using MoreShipUpgrades.Misc.TerminalNodes;
using UnityEngine;

namespace TerminalFormatter
{
    internal class LategameUpgradesCompatibility : MrovLib.Compatibility.CompatibilityBase
    {
        internal static Assembly LGUAssembly;

        public LategameUpgradesCompatibility(string guid, string version = null)
            : base(guid, version)
        {
            if (this.IsModPresent)
            {
                Init();
            }
        }

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

            LGUAssembly = Chainloader
                .PluginInfos["com.malco.lethalcompany.moreshipupgrades"]
                .Instance.GetType()
                .Assembly;

            Plugin.isLGUPresent = true;
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

                int itemNameWidth = Settings.itemNameWidth;
                string name = terminalNode.Name.PadRight(itemNameWidth);
                string discountPercent = salePercent > 0 ? $" -{salePercent}%" : "";

                string itemNameWithDiscount = name;
                string shortenedText = "... ";

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

        internal static int GetMoonPrice(int price)
        {
            // access internal class EfficientEngines in type MoreShipUpgrades.UpgradeComponents.TierUpgrades

            var typeName = "MoreShipUpgrades.UpgradeComponents.TierUpgrades.EfficientEngines";

            Type efficientEnginesType = Plugin.LGUCompat.GetModAssembly.GetType($"{typeName}");

            if (efficientEnginesType == null)
            {
                Plugin.logger.LogWarning($"Could not find {typeName} type");
                return price;
            }

            // run public static int GetDiscountedMoonPrice(int defaultPrice) in the EfficientEngines class
            MethodInfo getDiscountedMoonPrice = efficientEnginesType.GetMethod(
                "GetDiscountedMoonPrice",
                BindingFlags.Public | BindingFlags.Static
            );

            if (getDiscountedMoonPrice == null)
            {
                Plugin.logger.LogWarning(
                    "Could not find GetDiscountedMoonPrice method in EfficientEngines"
                );
                return price;
            }

            return (int)getDiscountedMoonPrice.Invoke(null, new object[] { price });
        }
    }
}
