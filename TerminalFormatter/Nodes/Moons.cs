using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using HarmonyLib;
using LethalLevelLoader;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
    public class Moons : TerminalFormatterNode
    {
        public Moons()
            : base("Moons", ["MoonsCatalogue", "preview", "sort", "filter"]) { }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching MoonsCatalogue");

            string currentTagFilter;

            if (
                node.name.Contains("preview")
                && Enum.TryParse(
                    typeof(PreviewInfoType),
                    TerminalManager.GetTerminalEventEnum(node.terminalEvent),
                    out object previewEnumValue
                )
            )
                LethalLevelLoader.Settings.levelPreviewInfoType = (PreviewInfoType)previewEnumValue;
            else if (
                node.name.Contains("sort")
                && Enum.TryParse(
                    typeof(SortInfoType),
                    TerminalManager.GetTerminalEventEnum(node.terminalEvent),
                    out object sortEnumValue
                )
            )
                LethalLevelLoader.Settings.levelPreviewSortType = (SortInfoType)sortEnumValue;
            else if (
                node.name.Contains("filter")
                && Enum.TryParse(
                    typeof(FilterInfoType),
                    TerminalManager.GetTerminalEventEnum(node.terminalEvent),
                    out object filterEnumValue
                )
            )
            {
                LethalLevelLoader.Settings.levelPreviewFilterType = (FilterInfoType)filterEnumValue;
                currentTagFilter = TerminalManager.GetTerminalEventString(node.terminalEvent);
            }

            // Call internal static RefreshExtendedLevelGroups through reflection
            MethodInfo refreshExtendedLevelGroups =
                typeof(LethalLevelLoader.TerminalManager).GetMethod(
                    "RefreshExtendedLevelGroups",
                    BindingFlags.NonPublic | BindingFlags.Static
                );
            refreshExtendedLevelGroups.Invoke(null, null);

            LethalLevelLoader.MoonsCataloguePage moonCatalogue =
                (LethalLevelLoader.MoonsCataloguePage)
                    MrovLib.API.SharedMethods.GetLLLMoonsCataloguePage();

            var table = new ConsoleTables.ConsoleTable(
                "", // Name
                "", // Price
                "" // Weather
            );

            var tableInConsole = new ConsoleTables.ConsoleTable(
                "Name",
                "Price",
                "Weather",
                "Difficulty"
            );

            var adjustedTable = new StringBuilder();

            string headerName = "MOONS CATALOGUE";
            Dictionary<int, string> headerInfo =
                new()
                {
                    {
                        0,
                        $"PREVIEW: {LethalLevelLoader.Settings.levelPreviewInfoType.ToString().ToUpper()}"
                    },
                    {
                        1,
                        $"SORT: {LethalLevelLoader.Settings.levelPreviewSortType.ToString().ToUpper()}"
                    },
                    {
                        2,
                        $"FILTER: {LethalLevelLoader.Settings.levelPreviewFilterType.ToString().ToUpper()}"
                    },
                };
            string moonsHeader = new Header().CreateNumberedHeader(headerName, 2, headerInfo);

            ConfigManager.LastUsedFilter.Value =
                LethalLevelLoader.Settings.levelPreviewFilterType.ToString();
            ConfigManager.LastUsedSort.Value =
                LethalLevelLoader.Settings.levelPreviewSortType.ToString();
            ConfigManager.LastUsedPreview.Value =
                LethalLevelLoader.Settings.levelPreviewInfoType.ToString();

            Plugin.logger.LogWarning(
                $"LLL preview type set to {LethalLevelLoader.Settings.levelPreviewInfoType}"
            );

            Plugin.logger.LogWarning(
                $"LLL filter type set to {LethalLevelLoader.Settings.levelPreviewFilterType}"
            );

            Plugin.logger.LogWarning(
                $"LLL sort type set to {LethalLevelLoader.Settings.levelPreviewSortType}"
            );

            Plugin.logger.LogDebug("MoonsCataloguePage: " + moonCatalogue);

            foreach (
                LethalLevelLoader.ExtendedLevelGroup extendedLevelGroup in moonCatalogue.ExtendedLevelGroups
            )
            {
                foreach (
                    LethalLevelLoader.ExtendedLevel extendedLevel in extendedLevelGroup.extendedLevelsList
                )
                {
                    if (MrovLib.API.SharedMethods.IsMoonHiddenLLL(extendedLevel.SelectableLevel))
                    {
                        continue;
                    }

                    string planetName = extendedLevel.NumberlessPlanetName;
                    Plugin.logger.LogDebug($"Planet: {planetName}");

                    bool showDifficulty =
                        (
                            ConfigManager.ShowDifficultyInAll.Value
                            && LethalLevelLoader.Settings.levelPreviewInfoType
                                == LethalLevelLoader.PreviewInfoType.All
                        )
                        || LethalLevelLoader.Settings.levelPreviewInfoType
                            == LethalLevelLoader.PreviewInfoType.Difficulty;

                    // make itemName length = itemNameWidth
                    // if showDifficulty, make difficulty length = 4 and display it on the right

                    int planetWidth = showDifficulty
                        ? Settings.planetNameWidth - 2
                        : Settings.planetNameWidth;

                    if (planetName.Length > Settings.planetNameWidth)
                    {
                        // replace last 3 characters with "..."
                        planetName = $"{planetName.Substring(0, planetWidth - 3)}...";
                    }
                    else
                    {
                        planetName = $"{planetName}".PadRight(planetWidth);
                    }

                    // if longer than 3, trim
                    var difficulty = showDifficulty
                        ? $" {SharedMethods.GetLevelRiskLevel(extendedLevel.SelectableLevel).PadRight(3)}"
                        : "";

                    bool showPrice =
                        LethalLevelLoader.Settings.levelPreviewInfoType
                            == LethalLevelLoader.PreviewInfoType.All
                        || LethalLevelLoader.Settings.levelPreviewInfoType
                            == LethalLevelLoader.PreviewInfoType.Price;
                    string price = showPrice
                        ? $"${SharedMethods.GetPrice(extendedLevel.RoutePrice)}"
                        : "";

                    bool showWeather =
                        LethalLevelLoader.Settings.levelPreviewInfoType
                            == LethalLevelLoader.PreviewInfoType.All
                        || LethalLevelLoader.Settings.levelPreviewInfoType
                            == LethalLevelLoader.PreviewInfoType.Weather;

                    // use reflection to call TerminalManager.GetWeatherConditions - must invoke the original method cause of weathertweaks
                    // it's internal static method
                    var weatherCondition = SharedMethods.GetWeather(extendedLevel.SelectableLevel);

                    string weather = showWeather
                        ? weatherCondition.PadRight(Settings.planetWeatherWidth - 2)
                        : "";

                    table.AddRow(
                        $"{planetName}{difficulty}",
                        $"{price}",
                        $"{weather}".PadLeft(Settings.planetWeatherWidth)
                    );

                    tableInConsole.AddRow(planetName, price, weather, difficulty);
                }

                table.AddRow("", "", "");
            }

            string tableString = table.ToStringCustomDecoration();
            adjustedTable.Append(moonsHeader);

            adjustedTable.Append(
                $" The Company // Buying at {Mathf.RoundToInt(StartOfRound.Instance.companyBuyingRate * 100f)}% \n\n"
            );

            adjustedTable.Append(tableString);

            string finalString = adjustedTable.ToString().TrimEnd();
            Plugin.logger.LogInfo("All strings:\n" + tableInConsole.ToMinimalString());

            return finalString;
        }
    }
}
