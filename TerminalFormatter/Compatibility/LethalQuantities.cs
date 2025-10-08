using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using LethalQuantities.Json;
using LethalQuantities.Objects;

namespace TerminalFormatter
{
  internal class LethalQuantitiesCompatibility : MrovLib.Compatibility.CompatibilityBase
  {
    public LethalQuantitiesCompatibility(string guid, string version = null)
      : base(guid, version)
    {
      if (this.IsModPresent)
      {
        Init();
      }
    }

    public static void Init() { }

    public static string GetLevelRiskLevel(SelectableLevel level)
    {
      // access internal Dictionary<Guid, LevelPreset> presets
      // in LethalQuantities.Plugin.INSTANCE

      Type pluginType = LethalQuantities.Plugin.INSTANCE.GetType();
      FieldInfo presetsField = pluginType.GetField("presets", BindingFlags.NonPublic | BindingFlags.Instance);

      // Plugin.logger.LogDebug("Got FieldInfo");

      Dictionary<Guid, LevelPreset> presets = (Dictionary<Guid, LevelPreset>)presetsField.GetValue(LethalQuantities.Plugin.INSTANCE);

      // Plugin.logger.LogDebug("Got presets");

      // get the LevelPreset for the level
      LevelPreset preset = presets.TryGetValue(level.getGuid(), out LevelPreset value) ? value : null;

      // Plugin.logger.LogDebug("Got preset");

      if (preset == null)
      {
        return null;
      }

      // return the risk level
      return preset.riskLevel.value;
    }
  }
}
