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
    public class MoonsOldLLL : TerminalFormatterNode
    {
        public MoonsOldLLL()
            : base("Moons", ["MoonsCatalogue", "preview", "sort", "filter"]) { }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching MoonsCatalogue - OldLLL");

            LethalLevelLoader.MoonsCataloguePage moonCatalogue =
                (LethalLevelLoader.MoonsCataloguePage)
                    MrovLib.API.SharedMethods.GetLLLMoonsCataloguePage();

            // MoonsCataloguePage moonCatalogue = Traverse
            //     .Create<TerminalManager>()
            //     .Field<MoonsCataloguePage>("currentMoonsCataloguePage")
            //     .Value;

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

            Plugin.logger.LogDebug("MoonsCataloguePage: " + moonCatalogue);

            foreach (
                LethalLevelLoader.ExtendedLevelGroup extendedLevelGroup in moonCatalogue.ExtendedLevelGroups
            )
            {
                foreach (
                    LethalLevelLoader.ExtendedLevel extendedLevel in extendedLevelGroup.extendedLevelsList
                )
                {
                    if (MrovLib.API.SharedMethods.IsMoonHiddenLLL(extendedLevel.selectableLevel))
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

                    // make itemName length = itemNameWidth
                    if (planetName.Length > Settings.planetNameWidth)
                    {
                        // replace last 3 characters with "..."
                        planetName = $"{planetName.Substring(0, Settings.planetNameWidth - 3)}...";
                    }
                    else
                    {
                        planetName = $"{planetName}".PadRight(Settings.planetNameWidth);
                    }

                    // if longer than 2, trim
                    var difficulty = showDifficulty
                        ? $" {extendedLevel.selectableLevel.riskLevel.ToString().PadRight(2)}"
                        : "";

                    // int LGUPrice;
                    // if(Plugin.isLGUPresent){
                    //     LGUPrice = extendedLevel.LGUPrice;
                    // } else {
                    //     LGUPrice = 0;
                    // }

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
                    var weatherCondition = SharedMethods.GetWeather(extendedLevel.selectableLevel);
                    // substring to Settings.planetWeatherWidth

                    if (
                        weatherCondition.Length > Settings.planetWeatherWidth - 2
                        || ConfigManager.UseShortenedWeathers.Value
                    )
                    {
                        // weatherCondition =
                        //     $"{weatherCondition.Substring(0, Settings.planetWeatherWidth - 2)}..";

                        Settings.WeathersShortened.Do(pair =>
                        {
                            weatherCondition = Regex.Replace(
                                weatherCondition,
                                pair.Key,
                                pair.Value
                            );
                        });
                    }

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
