using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using MrovLib;
using TerminalFormatter.Compatibility;
using TerminalUtils;
using TerminalUtils.Definitions;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
  public class Moons : TerminalFormatterNode
  {
    public Moons()
      : base("Moons", ["MoonsCatalogue", "preview", "sort", "filter"])
    {
      this.HelpText = " Welcome to the exomoons catalogue! \n Use ROUTE to to set the autopilot route. \n Use INFO to learn about any moon.";
    }

    public override bool IsNodeValid(TerminalNode node)
    {
      return true;
    }

    public override string GetNodeText(TerminalNode node)
    {
      bool decor = ConfigManager.ShowDecorations.Value;

      List<SelectableLevel> levelsToDisplay = TerminalUtils.TerminalManager.GetCurrentLevels();

      List<PreviewInfoType<SelectableLevel>> currentPreviewTypes = TerminalManager.CurrentPreviewInfoType;

      var table = new ConsoleTables.ConsoleTable(TerminalUtils.TerminalManager.CurrentPreviewInfoType.Select(info => info.Name).ToArray());

      var tableInConsole = new ConsoleTables.ConsoleTable("Name", "Price", "Weather", "Difficulty");

      var adjustedTable = new StringBuilder();

      string headerName = "MOONS CATALOGUE";
      Dictionary<int, string> headerInfo =
        new()
        {
          {
            0,
            $"<size=50%>PREVIEW: {string.Join(", ", TerminalUtils.TerminalManager.CurrentPreviewInfoType.Where(info => !info.Name.Contains("Name")).Select(info => info.Name.ToUpper()))}</size>"
          },
          { 1, $"<size=50%>SORT: {TerminalUtils.TerminalManager.CurrentSortInfoType.Name.ToUpper()}</size>" },
          { 2, $"<size=50%>FILTER: {TerminalUtils.TerminalManager.CurrentFilterInfoType.Name.ToUpper()}</size>" },
        };
      string moonsHeader = new Header().CreateNumberedHeader(headerName, 2, headerInfo);

      int itemCount = 1;

      foreach (SelectableLevel level in levelsToDisplay)
      {
        if (LevelHelper.IsHidden(level))
        {
          continue;
        }

        string planetName = MrovLib.StringResolver.GetAlphanumericName(level);
        string numbersInPlanetName = Regex.Match(level.PlanetName, @"^\d+").Value;

        // if the number is shorter than 3, fill it with 0s from the left
        if (ConfigManager.ShowNumberedPlanetNames.Value)
        {
          planetName = $"{numbersInPlanetName.PadLeft(3, '0')} {planetName}";
        }

        if (decor)
        {
          planetName = $"* {planetName}";
        }

        string[] rowItems = currentPreviewTypes.Select(info => info.Value(level)).ToArray();
        table.AddRow(rowItems);

        if (itemCount % 3 == 0)
        {
          itemCount = 1;
          table.AddRow(currentPreviewTypes.Select(_ => "").ToArray());
        }
        else
        {
          itemCount++;
        }
      }

      string tableString = table.ToStringCustomDecoration();
      adjustedTable.Append(moonsHeader);

      if (ConfigManager.ShowHelpText.Value)
      {
        adjustedTable.Append(this.HelpText != null ? $"\n{this.HelpText}\n\n" : "");
      }

      // adjustedTable.Append(
      //   LethalLevelLoader.Settings.levelPreviewFilterType == FilterInfoType.Tag
      //     ? " If you enabled tag filtering by accident: \n Use the <color=#0be697>filter none</color> command to disable it.\n\n"
      //     : ""
      // );

      adjustedTable.Append($" The Company // Buying at {Mathf.RoundToInt(StartOfRound.Instance.companyBuyingRate * 100f)}% \n\n");

      adjustedTable.Append(tableString);

      string finalString = adjustedTable.ToString().TrimEnd();

      Plugin.debugLogger.LogDebug("All strings:\n" + tableInConsole.ToMinimalString());
      Plugin.debugLogger.LogDebug("Final string:\n" + finalString);

      return finalString;
    }
  }
}
