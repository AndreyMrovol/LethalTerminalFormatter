using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using LethalLevelLoader;

namespace TerminalFormatter
{
  internal class LLLCompatibility(string guid, string version = null) : MrovLib.Compatibility.CompatibilityBase(guid, version)
  {
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public void Init()
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

      GetLLLSettings();
    }

    public static void GetLLLSettings()
    {
      // try casting the configManager entry value to PreviewInfoType enum
      // if it fails, set the value to the default enum value

      bool isPreviewValueVaild = Enum.TryParse(ConfigManager.LastUsedPreview.Value, out PreviewInfoType result);
      if (isPreviewValueVaild)
      {
        LethalLevelLoader.Settings.levelPreviewInfoType = result;
        // Plugin.logger.LogInfo($"LLL preview type set to {result}");
      }

      bool isFilterValueVaild = Enum.TryParse(ConfigManager.LastUsedFilter.Value, out FilterInfoType resultFilter);
      if (isFilterValueVaild)
      {
        LethalLevelLoader.Settings.levelPreviewFilterType = resultFilter;
        // Plugin.logger.LogInfo($"LLL filter type set to {resultFilter}");
      }

      bool isSortValueVaild = Enum.TryParse(ConfigManager.LastUsedSort.Value, out SortInfoType resultSort);
      if (isSortValueVaild)
      {
        LethalLevelLoader.Settings.levelPreviewSortType = resultSort;
        // Plugin.logger.LogInfo($"LLL sort type set to {resultSort}");
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

    public static string InvokeMoonOverrideInfoEvent(ExtendedLevel extendedLevel, PreviewInfoType infoType)
    {
      // Get the type of the TerminalManager class
      Type terminalManagerType = typeof(TerminalManager);

      // Get the event info for the onBeforePreviewInfoTextAdded event
      EventInfo eventInfo = terminalManagerType.GetEvent("onBeforePreviewInfoTextAdded", BindingFlags.Static | BindingFlags.Public);

      // Get the field that stores the delegate for the event (events are backed by delegate fields)
      FieldInfo fieldInfo = terminalManagerType.GetField("onBeforePreviewInfoTextAdded", BindingFlags.Static | BindingFlags.NonPublic);

      // Get the delegate from the field
      if (fieldInfo != null)
      {
        var delegateInstance = fieldInfo.GetValue(null) as Delegate;

        if (delegateInstance != null)
        {
          // Create an array of parameters for the delegate invocation
          object[] parameters = new object[] { extendedLevel, infoType };

          // Iterate through the invocation list and invoke each subscriber
          foreach (var handler in delegateInstance.GetInvocationList())
          {
            var result = handler.DynamicInvoke(parameters);
            return result as string;
          }
        }
      }

      return null;
    }

    public static bool SwapRouteNodeToLockedNode()
    {
      return false;
    }

    public static bool IsLevelLocked(SelectableLevel level)
    {
      return MrovLib.SharedMethods.IsMoonLockedLLL(level);
    }

    public static bool IsLevelHidden(SelectableLevel level)
    {
      return MrovLib.SharedMethods.IsMoonHiddenLLL(level);
    }
  }
}
