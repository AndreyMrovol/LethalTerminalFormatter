using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
    public class Scan : TerminalFormatterNode
    {
        public Scan()
            : base("Scan", ["ScanInfo"]) { }

        public override bool IsNodeValid(TerminalNode node, Terminal terminal)
        {
            return true;
        }

        public override string GetNodeText(TerminalNode node, Terminal terminal)
        {
            var adjustedTable = new StringBuilder();
            bool isShip = StartOfRound.Instance.inShipPhase;

            if (StartOfRound.Instance.currentLevel.PlanetName.Contains("Gordion"))
            {
                isShip = true;
            }

            int items = 0;
            int value = 0;

            List<GrabbableObject> objectsToScan;

            if (isShip)
            {
                objectsToScan = Object
                    .FindObjectsOfType<GrabbableObject>()
                    .Where(item =>
                        item.itemProperties.isScrap && (item.isInElevator || item.isInShipRoom)
                    )
                    .ToList();
            }
            else
            {
                objectsToScan = Object
                    .FindObjectsOfType<GrabbableObject>()
                    .Where(item =>
                        item.itemProperties.isScrap && !item.isInShipRoom && !item.isInElevator
                    )
                    .ToList();
            }

            objectsToScan = objectsToScan.OrderBy(x => x.scrapValue).ToList();

            var table = new ConsoleTables.ConsoleTable("Name", "Price", "Two-handed?");

            foreach (var item in objectsToScan)
            {
                table.AddRow(
                    item.itemProperties.itemName.PadRight(Settings.itemNameWidth),
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
                    { 0, $"SCANNING {(isShip ? "SHIP" : "MOON")}" },
                    { 1, $"ITEMS: {items.ToString()}" },
                    { 2, $"VALUE: ${value.ToString()}" },
                };
            string moonsHeader = new Header().CreateNumberedHeader(headerName, 2, headerInfo);

            adjustedTable.Append(moonsHeader);

            if (ConfigManager.DetailedScanPage.Value || isShip)
            {
                adjustedTable.Append(table.ToStringCustomDecoration(header: true));
            }

            return adjustedTable.ToString();
        }
    }
}
