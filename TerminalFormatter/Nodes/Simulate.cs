using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using TerminalFormatter.Patches;

namespace TerminalFormatter.Nodes
{
  public class Simulate : TerminalFormatterNode
  {
    public Simulate()
      : base("Simulate", ["simulate"]) { }

    public override bool IsNodeValid(TerminalNode node, Terminal terminal)
    {
      return true;
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public override string GetNodeText(TerminalNode node, Terminal terminal)
    {
      var table = new ConsoleTables.ConsoleTable("Interior", "Weight", "Chance");

      List<SelectableLevel> levels = MrovLib.LevelHelper.Levels;

      SelectableLevel currentLevel = levels
        .Where(level =>
        {
          string sanitizedNodeEvent = node.terminalEvent.ToString().Sanitized().ToLower();
          string sanitizedPlanetName = MrovLib.SharedMethods.GetNumberlessPlanetName(level).ToLower().Sanitized().Replace("-", "");
          return sanitizedNodeEvent.Contains(sanitizedPlanetName);
        })
        .FirstOrDefault();

      Dictionary<int, string> headerInfo = new() { { 1, $"PLANET: {MrovLib.SharedMethods.GetNumberlessPlanetName(currentLevel)}" }, };
      var header = new Header().CreateNumberedHeader("SIMULATING ARRIVAL", 2, headerInfo);

      LethalLevelLoader.PatchedContent.ExtendedLevelDictionary.TryGetValue(currentLevel, out var currentExtendedLevel);

      List<LethalLevelLoader.ExtendedDungeonFlowWithRarity> currentPlanetDungeonFlows =
        LethalLevelLoader.DungeonManager.GetValidExtendedDungeonFlows(currentExtendedLevel, false);

      Dictionary<string, int> dungeonFlowRarities = [];

      int totalRarityPool = 0;
      currentPlanetDungeonFlows.Do(dungeonFlow =>
      {
        totalRarityPool += dungeonFlow.rarity;

        dungeonFlowRarities.Add(dungeonFlow.extendedDungeonFlow.DungeonName, dungeonFlow.rarity);
      });

      dungeonFlowRarities = dungeonFlowRarities.OrderBy(o => -(o.Value)).ToDictionary(k => k.Key, v => v.Value);

      foreach ((string dungeonName, int dungeonRarity) in dungeonFlowRarities)
      {
        table.AddRow(
          dungeonName.PadRight(Settings.planetNameWidth),
          dungeonRarity,
          $"{((float)dungeonRarity / (float)totalRarityPool * 100).ToString("F2")}%".PadLeft(4)
        );
      }

      table.AddRow("", "", "");
      table.AddRow("", "", "");
      table.AddRow("", totalRarityPool.ToString().PadRight(6), "100%".ToString().PadLeft(4));

      var adjustedTable = new StringBuilder();
      adjustedTable.Append(header);

      // don't simulate for fucking March
      if (currentExtendedLevel.NumberlessPlanetName == "March")
      {
        adjustedTable.AppendLine("\nData for March will always be innacurate\n");
      }

      adjustedTable.Append("\n");
      adjustedTable.Append(table.ToStringCustomDecoration(header: true));

      return adjustedTable.ToString();
    }
  }
}
