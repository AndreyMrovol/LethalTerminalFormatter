using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using HarmonyLib;
using LethalLevelLoader;
using UnityEngine;

namespace TerminalFormatter
{
    partial class Nodes
    {
        private static readonly int planetWeatherWidth = 18;
        private static readonly int planetNameWidth =
            TerminalPatches.terminalWidth + 2 - planetWeatherWidth - 9;

        static readonly ManualLogSource logger = Plugin.logger;

        public string Moons(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching MoonsCatalogue");

            MoonsCataloguePage moonCatalogue = Traverse
                .Create<TerminalManager>()
                .Field<MoonsCataloguePage>("currentMoonsCataloguePage")
                .Value;

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
                    { 0, $"PREVIEW: {Settings.levelPreviewInfoType.ToString().ToUpper()}" },
                    { 1, $"SORT: {Settings.levelPreviewSortType.ToString().ToUpper()}" },
                    { 2, $"FILTER: {Settings.levelPreviewFilterType.ToString().ToUpper()}" },
                };
            string moonsHeader = new Header().CreateNumberedHeader(headerName, 2, headerInfo);

            ConfigManager.LastUsedFilter.Value = Settings.levelPreviewFilterType.ToString();
            ConfigManager.LastUsedSort.Value = Settings.levelPreviewSortType.ToString();
            ConfigManager.LastUsedPreview.Value = Settings.levelPreviewInfoType.ToString();

            logger.LogDebug("MoonsCataloguePage: " + moonCatalogue);

            foreach (ExtendedLevelGroup extendedLevelGroup in moonCatalogue.ExtendedLevelGroups)
            {
                foreach (ExtendedLevel extendedLevel in extendedLevelGroup.extendedLevelsList)
                {
                    string planetName = extendedLevel.NumberlessPlanetName;
                    logger.LogDebug($"Planet: {planetName}");

                    // make itemName length = itemNameWidth
                    if (planetName.Length > planetNameWidth)
                    {
                        // replace last 3 characters with "..."
                        planetName = $"{planetName.Substring(0, planetNameWidth - 3)}...";
                    }
                    else
                    {
                        planetName = $"{planetName}".PadRight(planetNameWidth);
                    }

                    bool showDifficulty =
                        (
                            ConfigManager.ShowDifficultyInAll.Value
                            && Settings.levelPreviewInfoType == PreviewInfoType.All
                        )
                        || Settings.levelPreviewInfoType == PreviewInfoType.Difficulty;

                    // if longer than 2, trim
                    var difficulty = showDifficulty
                        ? $" {extendedLevel.selectableLevel.riskLevel.ToString().PadRight(2)}"
                        : "";

                    bool showPrice =
                        Settings.levelPreviewInfoType == PreviewInfoType.All
                        || Settings.levelPreviewInfoType == PreviewInfoType.Price;
                    string price = showPrice ? $"${extendedLevel.RoutePrice}" : "";

                    bool showWeather =
                        Settings.levelPreviewInfoType == PreviewInfoType.All
                        || Settings.levelPreviewInfoType == PreviewInfoType.Weather;

                    // use reflection to call TerminalManager.GetWeatherConditions - must invoke the original method cause of weathertweaks
                    // it's internal static method
                    var weatherCondition = typeof(TerminalManager)
                        .GetMethod(
                            "GetWeatherConditions",
                            BindingFlags.NonPublic | BindingFlags.Static
                        )
                        .Invoke(null, new object[] { extendedLevel.selectableLevel })
                        .ToString()
                        .Replace("(", "")
                        .Replace(")", "");
                    // substring to planetWeatherWidth

                    if (weatherCondition.Length > planetWeatherWidth - 2)
                    {
                        weatherCondition =
                            $"{weatherCondition.Substring(0, planetWeatherWidth - 2)}..";
                    }

                    string weather = showWeather
                        ? weatherCondition.PadRight(planetWeatherWidth - 2)
                        : "";

                    table.AddRow(
                        planetName,
                        $"{price}",
                        $"{difficulty}{weather}".PadLeft(planetWeatherWidth)
                    );

                    tableInConsole.AddRow(planetName, price, weather, difficulty);
                }

                table.AddRow("", "", "");
            }

            string tableString = RemoveTable(table.ToMarkDownString(), false);
            adjustedTable.Append(moonsHeader);

            adjustedTable.Append(
                $"\n The Company // Buying at {Mathf.RoundToInt(StartOfRound.Instance.companyBuyingRate * 100f)}%\n\n"
            );

            adjustedTable.Append(tableString);

            string finalString = adjustedTable.ToString().TrimEnd();
            Plugin.logger.LogInfo("All strings:\n" + tableInConsole.ToMinimalString());

            return finalString;
        }
    }
}
