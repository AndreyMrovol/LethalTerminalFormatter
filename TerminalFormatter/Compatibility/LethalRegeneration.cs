using System;
using System.Collections.Generic;
using System.Reflection;
using AdvancedCompany.Config;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using LethalLevelLoader;
using LethalRegeneration;

namespace TerminalFormatter
{
    internal class LethalRegenCompatibility
    {
        public static void Init(bool start = true)
        {
            // try casting the configManager entry value to PreviewInfoType enum
            // if it fails, set the value to the default enum value

            Plugin.logger.LogInfo("Checking for LethalRegeneration");
            Plugin.logger.LogInfo(
                $"{LethalRegeneration.config.Configuration.Instance.HealingUpgradeEnabled}"
            );

            if (!LethalRegeneration.config.Configuration.Instance.HealingUpgradeEnabled)
            {
                return;
            }

            Plugin.logger.LogInfo("Patching LethalRegeneration");
        }

        public static int GetCost()
        {
            return LethalRegeneration.config.Configuration.Instance.HealingUpgradePrice;
        }

        public static bool IsUpgradeBought()
        {
            return LethalRegeneration.config.Configuration.Instance.HealingUpgradeUnlocked;
        }
    }
}
