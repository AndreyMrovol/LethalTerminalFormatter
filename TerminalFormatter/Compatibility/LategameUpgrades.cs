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
            LGUAssembly = Chainloader
                .PluginInfos["com.malco.lethalcompany.moreshipupgrades"]
                .Instance.GetType()
                .Assembly;

            Plugin.isLGUPresent = true;
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
