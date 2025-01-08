using System;
using System.Collections.Generic;
using System.Reflection;
using AdvancedCompany.Config;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;

namespace TerminalFormatter
{
  internal class ACCompatibility
  {
    internal static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("TerminalFormatter AC");

    internal static FieldInfo ServerConfiguration;
    internal static Dictionary<string, bool> Items = [];

    public static bool populated = false;

    public static void Init(string assemblyName)
    {
      // We need to access AdvancedCompany.Config.ServerConfiguration.Instance for all future shenanigans
      // this is hacky, amateurish and very bad practice, but it works
      // oh well

      string nspace = "AdvancedCompany.Config";
      string className = "ServerConfiguration";

      // Get the assembly that contains the class
      var assembly = Chainloader.PluginInfos[assemblyName].Instance.GetType().Assembly;

      // Get the Type object for the class
      var type = assembly.GetType($"{nspace}.{className}");

      if (type != null)
      {
        logger.LogInfo($"Type {type} found");

        var instance = type.GetField("Instance", BindingFlags.Public | BindingFlags.Static);

        if (instance != null)
        {
          logger.LogInfo($"Property {instance} found");
          Variables.IsACActive = true;
          ServerConfiguration = instance;

          // Refresh();
        }
        else
        {
          logger.LogError($"Property {instance} not found");
        }
      }
      else
      {
        logger.LogDebug($"Type {nspace}.{className} not found");
      }
    }

    internal static void Refresh()
    {
      if (!Variables.IsACActive)
        return;

      try
      {
        Items.Clear();
        var value = ServerConfiguration.GetValue(null);
        if (value != null)
        {
          logger.LogInfo($"Value {value} found");

          FieldInfo fieldInfo = value.GetType().GetField("Items", BindingFlags.Public | BindingFlags.Instance);
          if (fieldInfo != null)
          {
            // logger.LogInfo($"Property {fieldInfo} found");
            LobbyConfiguration.ItemsConfig items = (LobbyConfiguration.ItemsConfig)fieldInfo.GetValue(value);

            Dictionary<string, LobbyConfiguration.ItemConfig> ItemsItems = items.Items;

            // Plugin.logger.LogDebug($"Items: {ItemsItems}, count: {ItemsItems.Count}");

            ItemsItems.Do(x =>
            {
              Items.Add(x.Key, x.Value.Active);
            });
            // rest of your code
          }
          else
          {
            logger.LogError($"Property {fieldInfo} not found");
          }
        }
        else
        {
          logger.LogError($"Property {ServerConfiguration} value not found");
        }
      }
      catch (Exception e)
      {
        logger.LogError(e);
      }
    }
  }
}
