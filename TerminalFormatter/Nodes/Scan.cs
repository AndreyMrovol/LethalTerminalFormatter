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
        public string Scan(TerminalNode node, Terminal terminal)
        {
            Plugin.logger.LogDebug("Patching ScanForItems");

            var adjustedTable = new StringBuilder();

            int items = 0;
            int value = 0;

            var objectsInLevel = UnityEngine
                .Object.FindObjectsOfType<GrabbableObject>()
                .Where(item =>
                    item.itemProperties.isScrap && !item.isInShipRoom && !item.isInElevator
                )
                .ToList()
                .OrderBy(item => item.scrapValue);

            var table = new ConsoleTables.ConsoleTable("Name", "Price", "Two-handed?");

            foreach (var item in objectsInLevel)
            {
                table.AddRow(
                    item.itemProperties.itemName.PadRight(itemNameWidth),
                    $"${item.scrapValue}",
                    item.itemProperties.twoHanded ? "●" : "○"
                );

                items++;
                value += item.scrapValue;
            }

            string headerName = "SCANNER";
            Dictionary<int, string> headerInfo =
                new()
                {
                    { 0, $"ITEMS: {items.ToString()}" },
                    { 1, $"VALUE: ${value.ToString()}" },
                };
            string moonsHeader = new Header().CreateNumberedHeader(headerName, 2, headerInfo);

            adjustedTable.Append(moonsHeader);
            adjustedTable.Append(RemoveTable(table.ToMarkDownString()));

            return adjustedTable.ToString();
        }
    }
}
