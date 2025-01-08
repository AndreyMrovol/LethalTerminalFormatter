namespace TerminalFormatter
{
  internal class LethalRegenCompatibility
  {
    internal static bool IsUpgradeInStore = false;

    public static void Init()
    {
      IsUpgradeInStore = LethalRegeneration.config.Configuration.Instance.HealingUpgradeEnabled;
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
