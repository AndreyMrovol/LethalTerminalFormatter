using System;
using System.Collections.Generic;
using System.Reflection;
using AdvancedCompany.Config;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using LethalLevelLoader;

namespace TerminalFormatter
{
    internal class LLLCompatibility
    {
        public static void Init()
        {
            Plugin.harmony.Patch(
                AccessTools.Method(
                    typeof(LethalLevelLoader.Patches),
                    "TerminalLoadNewNode_Postfix"
                ),
                prefix: new HarmonyMethod(typeof(LLLCompatibility), nameof(LLLLoadNodePatch))
            );

            GetLLLSettings();
        }

        public static void GetLLLSettings()
        {
            // try casting the configManager entry value to PreviewInfoType enum
            // if it fails, set the value to the default enum value

            bool isPreviewValueVaild = Enum.TryParse(
                ConfigManager.LastUsedPreview.Value,
                out PreviewInfoType result
            );
            if (isPreviewValueVaild)
            {
                LethalLevelLoader.Settings.levelPreviewInfoType = result;
                // Plugin.logger.LogInfo($"LLL preview type set to {result}");
            }

            bool isFilterValueVaild = Enum.TryParse(
                ConfigManager.LastUsedFilter.Value,
                out FilterInfoType resultFilter
            );
            if (isFilterValueVaild)
            {
                LethalLevelLoader.Settings.levelPreviewFilterType = resultFilter;
                // Plugin.logger.LogInfo($"LLL filter type set to {resultFilter}");
            }

            bool isSortValueVaild = Enum.TryParse(
                ConfigManager.LastUsedSort.Value,
                out SortInfoType resultSort
            );
            if (isSortValueVaild)
            {
                LethalLevelLoader.Settings.levelPreviewSortType = resultSort;
                // Plugin.logger.LogInfo($"LLL sort type set to {resultSort}");
            }

            Variables.ISLLLActive = true;
        }

        public static bool LLLLoadNodePatch(Terminal __0, ref TerminalNode __1)
        {
            if (__1 == Variables.LastReplacedNode)
            {
                return false;
            }

            return true;
        }

        public static string InvokeMoonOverrideInfoEvent(
            ExtendedLevel extendedLevel,
            PreviewInfoType infoType
        )
        {
            // Get the type of the TerminalManager class
            Type terminalManagerType = typeof(TerminalManager);

            // Get the event info for the onBeforePreviewInfoTextAdded event
            EventInfo eventInfo = terminalManagerType.GetEvent(
                "onBeforePreviewInfoTextAdded",
                BindingFlags.Static | BindingFlags.Public
            );

            // Get the field that stores the delegate for the event (events are backed by delegate fields)
            FieldInfo fieldInfo = terminalManagerType.GetField(
                "onBeforePreviewInfoTextAdded",
                BindingFlags.Static | BindingFlags.NonPublic
            );

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
    }
}
