using System.Runtime.CompilerServices;
using HarmonyLib;

namespace TerminalFormatter.Compatibility
{
  internal class LLLCompatibility : MrovLib.CompatibilityHandler
  {
    public LLLCompatibility(string guid, string version = null)
      : base(guid, version) { }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public override void Init()
    {
      if (!this.IsModPresent)
      {
        return;
      }

      Plugin.harmony.Patch(
        AccessTools.Method(typeof(LethalLevelLoader.Patches), "TerminalLoadNewNode_Postfix"),
        prefix: new HarmonyMethod(typeof(LLLCompatibility), nameof(LLLLoadNodePatch))
      );

      if (Plugin.DawnLibCompat.IsModPresent)
      {
        Plugin.harmony.Patch(
          AccessTools.Method(typeof(LethalLevelLoader.TerminalManager), "SwapRouteNodeToLockedNode"),
          prefix: new HarmonyMethod(typeof(LLLCompatibility), nameof(SwapRouteNodeToLockedNode))
        );
      }
    }

    public static bool LLLLoadNodePatch(Terminal __0, ref TerminalNode __1)
    {
      if (__1 == Variables.LastReplacedNode)
      {
        return false;
      }

      return true;
    }

    public static bool SwapRouteNodeToLockedNode()
    {
      return false;
    }
  }
}
