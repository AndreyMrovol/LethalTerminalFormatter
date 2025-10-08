using GameNetcodeStuff;
using HarmonyLib;
using MrovLib.ContentType;
using StoreRotationConfig.Api;

namespace TerminalFormatter.Compatibility
{
  internal class StoreRotationConfigCompatibility : MrovLib.Compatibility.CompatibilityBase
  {
    public StoreRotationConfigCompatibility(string guid, string version = null)
      : base(guid, version)
    {
      if (IsModPresent)
      {
        Init();
      }
    }

    public static void Init()
    {
      UnpatchTerminalScroll();
    }

    public static int GetDiscountedPrice(BuyableThing buyable, out int discount)
    {
      return RotationSalesAPI.GetDiscountedPrice(buyable.Nodes.Node, out discount);
    }

    public static void UnpatchTerminalScroll()
    {
      // Prioritize TerminalFormatter's scrolling settings.
      Plugin.harmony.Unpatch(
        AccessTools.Method(typeof(PlayerControllerB), "ScrollMouse_performed"),
        HarmonyPatchType.Transpiler,
        "pacoito.StoreRotationConfig"
      );
    }
  }
}
