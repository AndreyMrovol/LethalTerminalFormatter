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
    public class SimulateOldLLL : TerminalFormatterNode
    {
        public SimulateOldLLL()
            : base("Simulate", ["simulate"]) { }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable(
                "Interior", // Name
                "Weight", // Price
                "Chance" // Weather
            );

            List<ExtendedLevel> levels = LethalLevelLoader.PatchedContent.ExtendedLevels;

            // levels.Do(level =>
            // {
            //     Plugin.logger.LogWarning(level.NumberlessPlanetName);
            //     Plugin.logger.LogWarning(node.terminalEvent.ToString().ToLower().Sanitized());
            //     Plugin.logger.LogWarning(level.NumberlessPlanetName.ToLower().Sanitized());
            //     Plugin.logger.LogWarning(
            //         level.NumberlessPlanetName.ToLower().Sanitized().Replace("-", "")
            //     );
            //     Plugin.logger.LogDebug("---");
            // });

            ExtendedLevel currentLevel = levels
                .Where(level =>
                    node.terminalEvent.ToString()
                        .ToLower()
                        .Sanitized()
                        .Contains(level.NumberlessPlanetName.ToLower().Sanitized().Replace("-", ""))
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
                    dungeonFlow.extendedDungeonFlow.dungeonDisplayName.PadRight(
                        Settings.planetNameWidth
                    ),
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

            // don't simulate for fucking March
            if (currentLevel.NumberlessPlanetName == "March")
            {
                adjustedTable.AppendLine("\nData for March will always be innacurate\n");
            }

            adjustedTable.Append("\n");
            adjustedTable.Append(table.ToStringCustomDecoration(header: true));

            return adjustedTable.ToString();
        }
    }
}
