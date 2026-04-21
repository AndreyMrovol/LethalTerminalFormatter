using System;
using System.Reflection;
using BepInEx.Bootstrap;

namespace TerminalFormatter.Compatibility
{
  internal class LategameUpgradesCompatibility : MrovLib.CompatibilityHandler
  {
    public LategameUpgradesCompatibility(string guid, string version = null)
      : base(guid, version) { }

    internal static int GetMoonPrice(int price)
    {
      // access internal class EfficientEngines in type MoreShipUpgrades.UpgradeComponents.TierUpgrades

      var typeName = "MoreShipUpgrades.UpgradeComponents.TierUpgrades.EfficientEngines";

      Type efficientEnginesType = Plugin.LGUCompat.GetModAssembly.GetType($"{typeName}");

      if (efficientEnginesType == null)
      {
        Plugin.debugLogger.LogWarning($"Could not find {typeName} type");
        return price;
      }

      // run public static int GetDiscountedMoonPrice(int defaultPrice) in the EfficientEngines class
      MethodInfo getDiscountedMoonPrice = efficientEnginesType.GetMethod("GetDiscountedMoonPrice", BindingFlags.Public | BindingFlags.Static);

      if (getDiscountedMoonPrice == null)
      {
        Plugin.debugLogger.LogWarning("Could not find GetDiscountedMoonPrice method in EfficientEngines");
        return price;
      }

      return (int)getDiscountedMoonPrice.Invoke(null, new object[] { price });
    }
  }
}
