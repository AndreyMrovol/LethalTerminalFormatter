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
        public string Simulate(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable(
                "Interior", // Name
                "Weight", // Price
                "Chance" // Weather
            );

            List<ExtendedLevel> levels = LethalLevelLoader.PatchedContent.ExtendedLevels;
            ExtendedLevel currentLevel = levels
                .Where(level =>
                    node.terminalEvent.ToString()
                        .ToLower()
                        .Sanitized()
                        .Contains(level.NumberlessPlanetName.ToLower().Sanitized())
                )
                .FirstOrDefault();

            Dictionary<int, string> headerInfo =
                new() { { 1, $"PLANET: {currentLevel.NumberlessPlanetName}" }, };
            var header = new Header().CreateNumberedHeader("SIMULATING ARRIVAL", 2, headerInfo);

            Plugin.logger.LogWarning("Current Level: " + currentLevel.NumberlessPlanetName);

            var currentPlanetDungeonFlows =
                (List<ExtendedDungeonFlowWithRarity>)
                    typeof(DungeonManager)
                        .GetMethod(
                            "GetValidExtendedDungeonFlows",
                            BindingFlags.NonPublic | BindingFlags.Static
                        )
                        .Invoke(null, [currentLevel, false]);

            currentPlanetDungeonFlows.OrderBy(o => -(o.rarity)).ToList();

            int totalRarityPool = 0;
            currentPlanetDungeonFlows.Do(dungeonFlow => totalRarityPool += dungeonFlow.rarity);

            foreach (var dungeonFlow in currentPlanetDungeonFlows)
            {
                Plugin.logger.LogDebug(
                    $"{dungeonFlow.extendedDungeonFlow.dungeonDisplayName} - {dungeonFlow.rarity} - {totalRarityPool}"
                );

                table.AddRow(
                    dungeonFlow.extendedDungeonFlow.dungeonDisplayName.PadRight(planetNameWidth),
                    dungeonFlow.rarity,
                    $"{((float)dungeonFlow.rarity / (float)totalRarityPool * 100).ToString("F2")}%".PadLeft(
                        4
                    )
                );
            }

            table.AddRow("", "", "");
            table.AddRow("", "", "");
            table.AddRow("", totalRarityPool.ToString().PadRight(6), "100%".ToString().PadLeft(4));

            var adjustedTable = new StringBuilder();
            adjustedTable.Append(header);
            // adjustedTable.Append("\n\n");
            // adjustedTable.Append("Chances of each interior being selected:");

            // don't simulate for fucking March
            if (currentLevel.NumberlessPlanetName == "March")
            {
                adjustedTable.AppendLine("\nData for March will always be innacurate\n");
            }

            adjustedTable.Append("\n\n");
            adjustedTable.Append(RemoveTable(table.ToMarkDownString(), false));

            return adjustedTable.ToString();
        }
    }
}
