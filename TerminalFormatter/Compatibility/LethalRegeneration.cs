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
        public static void Init()
        {
            if (!LethalRegeneration.config.Configuration.Instance.HealingUpgradeEnabled)
            {
                return;
            }
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
