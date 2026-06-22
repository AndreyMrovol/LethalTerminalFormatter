using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrovLib;
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

    public override string GetNodeText(TerminalNode node)
    {
      // TODO: change this to TerminalManager.GetCurrentLevels() after the update upstream
      List<SelectableLevel> levelsToDisplay = TerminalManager.GetCurrentLevels();
      List<PreviewInfoType<SelectableLevel>> currentPreviewTypes = TerminalManager.CurrentPreviewInfoType;

      if (ConfigManager.ShowNumberedPlanetNames.Value)
      {
        currentPreviewTypes = currentPreviewTypes
          .Select(infoType =>
          {
            if (infoType.Name == "Name")
            {
              return TerminalUtils.TerminalManager.PreviewInfoTypes["NumberedName"];
            }
            else
            {
              return infoType;
            }
          })
          .ToList();
      }

      var table = new ConsoleTables.ConsoleTable(
        TerminalUtils
          .TerminalManager.CurrentPreviewInfoType.Select(info =>
          {
            return "";
          })
          .ToArray()
      );
      var adjustedTable = new StringBuilder();

      string headerName = "MOONS CATALOGUE";
      Dictionary<int, string> headerInfo =
        new()
        {
          {
            0,
            $"PREVIEW: <size=70%>{string.Join(", ", TerminalUtils.TerminalManager.CurrentPreviewInfoType.Where(info => !info.Name.Contains("Name")).Select(info => info.Name.ToUpper()))}</size>"
          },
          { 1, $"SORT: {TerminalUtils.TerminalManager.CurrentSortInfoType.Name.ToUpper()}" },
          { 2, $"FILTER: {TerminalUtils.TerminalManager.CurrentFilterInfoType.Name.ToUpper()}" },
        };
      string moonsHeader = new Header().CreateNumberedHeader(headerName, 2, headerInfo);

      int itemCount = 1;

      foreach (SelectableLevel level in levelsToDisplay)
      {
        if (LevelHelper.IsHidden(level) && !ConfigManager.AlwaysDisplayHiddenMoons.Value)
        {
          continue;
        }

        string[] rowItems = currentPreviewTypes
          .Select(info =>
          {
            if (info.Name.Contains("Name"))
            {
              return $"* {info.Value(level)}";
            }
            else
            {
              return info.Value(level);
            }
          })
          .ToArray();
        table.AddRow(rowItems);

        if (itemCount % 3 == 0)
        {
          itemCount = 1;

          if (ConfigManager.ShowGroupDividerLines.Value)
          {
            var dividerRow = new string[currentPreviewTypes.Count];
            dividerRow[0] = "".PadRight(Settings.planetNameWidth, '-');

            table.AddRow(dividerRow);
          }
          else
          {
            table.AddRow(currentPreviewTypes.Select(_ => "").ToArray());
          }
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

      adjustedTable.Append($" The Company // Buying at {Mathf.RoundToInt(StartOfRound.Instance.companyBuyingRate * 100f)}% \n\n");

      adjustedTable.Append(tableString);

      string finalString = adjustedTable.ToString().TrimEnd();

      Plugin.debugLogger.LogDebug("Final string:\n" + finalString);

      return finalString;
    }
  }
}
