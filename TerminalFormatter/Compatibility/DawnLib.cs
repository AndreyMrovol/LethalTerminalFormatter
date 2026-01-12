using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Dawn;
using Dawn.Internal;
using HarmonyLib;

namespace TerminalFormatter
{
  internal class DawnLibCompatibility : MrovLib.Compatibility.CompatibilityBase
  {
    public DawnLibCompatibility(string guid, string version = null)
      : base(guid, version) { }

    public void Init()
    {
      Type dawnTerminalType = AccessTools.TypeByName("Dawn.MoonRegistrationHandler");
      Plugin.harmony.Patch(
        AccessTools.Method(dawnTerminalType, "DynamicMoonCatalogue"),
        transpiler: new HarmonyMethod(AccessTools.Method(typeof(DawnLibCompatibility), nameof(InsertMoonCatalogueSkip)))
      );
    }

    public (bool locked, bool hidden) GetLevelStatus(SelectableLevel level)
    {
      return level.GetDawnInfo().DawnPurchaseInfo.PurchasePredicate.CanPurchase() switch
      {
        TerminalPurchaseResult.HiddenPurchaseResult hiddenResult => (hiddenResult.IsFailure, true),
        TerminalPurchaseResult.FailedPurchaseResult => (true, false),
        TerminalPurchaseResult.SuccessPurchaseResult => (false, false),
        _ => (false, false)
      };
    }

    public static IEnumerable<CodeInstruction> InsertMoonCatalogueSkip(IEnumerable<CodeInstruction> instructions)
    {
      var matcher = new CodeMatcher(instructions);

      matcher
        .Start()
        .Insert(
          new CodeInstruction(OpCodes.Ldarg_0), // self (orig delegate)
          new CodeInstruction(OpCodes.Ldarg_1), // self (Terminal instance)
          new CodeInstruction(OpCodes.Ldarg_2), // modifieddisplaytext
          new CodeInstruction(OpCodes.Ldarg_3), // node
          new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(On.Terminal.orig_TextPostProcess), "Invoke")),
          new CodeInstruction(OpCodes.Ret)
        );

      return matcher.InstructionEnumeration();
    }
  }
}
