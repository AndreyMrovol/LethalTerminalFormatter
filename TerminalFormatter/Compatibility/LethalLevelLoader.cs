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
    }
}
