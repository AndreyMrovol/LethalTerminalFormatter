namespace TerminalFormatter.Compatibility
{
  internal class LethalRegenCompatibility : MrovLib.CompatibilityHandler
  {
    internal static bool IsUpgradeInStore = false;

    public LethalRegenCompatibility(string guid, string version = null)
      : base(guid, version) { }

    public override void Init()
    {
      if (!this.IsModPresent)
      {
        return;
      }

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
