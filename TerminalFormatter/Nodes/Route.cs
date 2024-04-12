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
        public string Route(TerminalNode node, Terminal terminal)
        {
            var table = new ConsoleTables.ConsoleTable(
                "Planet", // Name
                "Price", // Price
                "Weather" // Weather
            );

            var header = new Header().CreateHeaderWithoutLines("CONFIRM ROUTE");
            var adjustedTable = new StringBuilder();

            List<ExtendedLevel> levels = LethalLevelLoader.PatchedContent.ExtendedLevels;
            ExtendedLevel currentLevel = levels
                .Where(level => level.routeNode == node)
                .FirstOrDefault();

            string currentWeather = typeof(TerminalManager)
                .GetMethod("GetWeatherConditions", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, [currentLevel.selectableLevel])
                .ToString()
                .Replace("(", "")
                .Replace(")", "");

            if (
                currentWeather.Length > TerminalPatches.terminalWidth - 5
                || ConfigManager.UseShortenedWeathers.Value
            )
            {
                WeathersShortened.Do(pair =>
                {
                    currentWeather = Regex.Replace(currentWeather, pair.Key, pair.Value);
                });
            }

            adjustedTable.Append(header);
            adjustedTable.Append("\n\n");
            adjustedTable.Append("Please CONFIRM or DENY routing the autopilot:");
            adjustedTable.Append("\n\n");
            // adjustedTable.Append(table.ToStringCustomDecoration());

            adjustedTable.AppendLine($" PLANET: {currentLevel.NumberlessPlanetName}");
            adjustedTable.AppendLine(
                $" PRICE: ${currentLevel.RoutePrice} (${terminal.groupCredits - currentLevel.RoutePrice} after routing)"
            );
            adjustedTable.AppendLine(
                $" WEATHER: {(currentWeather == "" ? "Clear" : currentWeather)}"
            );

            return adjustedTable.ToString();
        }
    }
}
