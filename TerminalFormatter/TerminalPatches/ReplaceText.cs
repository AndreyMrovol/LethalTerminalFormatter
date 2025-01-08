using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;

namespace TerminalFormatter
{
  [HarmonyPatch(typeof(Terminal))]
  public partial class TerminalPatches
  {
    [HarmonyPrefix]
    [HarmonyPatch("LoadNewNode")]
    [HarmonyPriority(Priority.Last)]
    [HarmonyAfter("imabatby.lethallevelloader")]
    public static bool LoadNewTerminalFormatterNode(Terminal __instance, TerminalNode node)
    {
      __instance.modifyingText = true;
      // __instance.RunTerminalEvents(node);
      __instance.screenText.interactable = true;
      string newDisplayText = null;

      Variables.LastReplacedNode = null;

      // check if node.name contains any of TerminalFormatterNode.terminalNode strings
      List<TerminalFormatterNode> possibleNodes = Settings
        .RegisteredNodes.Where(formatterNode => formatterNode.terminalNode.Any(y => node.name.Contains(y)))
        .ToList();

      if (possibleNodes != null)
      {
        Plugin.debugLogger.LogDebug($"Possible nodes count: {possibleNodes.Count}");
      }
      else
      {
        return true;
      }

      for (int i = 0; i < possibleNodes.Count; i++)
      {
        TerminalFormatterNode currentNode = possibleNodes[i];

        if (currentNode != null)
        {
          Plugin.debugLogger.LogDebug($"Checking if node {currentNode.name} is valid...");
          bool shouldRun = currentNode.IsNodeValid(node, __instance);
          if (!shouldRun)
          {
            Plugin.debugLogger.LogDebug($"Node {currentNode.name} is not valid");
            continue;
          }

          if (!currentNode.Enabled.Value)
          {
            Plugin.debugLogger.LogDebug($"Node {currentNode.name} is not enabled");
            continue;
          }

          Plugin.debugLogger.LogDebug($"Using node {currentNode.name}");
          newDisplayText = currentNode.GetNodeText(node, __instance);
          break;
        }
        else
        {
          continue;
        }
      }

      if (newDisplayText != null)
      {
        StringBuilder builder = new();

        if (__instance.displayingPersistentImage)
        {
          builder.Append("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        }

        builder.Append("\n\n");
        builder.Append(newDisplayText);
        builder.Append($"\n{new string('-', Settings.dividerLength)}\n");

        Plugin.debugLogger.LogMessage("New display text:\n" + newDisplayText);

        if (node.playSyncedClip != -1)
        {
          __instance.PlayTerminalAudioServerRpc(node.playSyncedClip);
        }
        else if (node.playClip != null)
        {
          __instance.terminalAudio.PlayOneShot(node.playClip);
        }

        __instance.LoadTerminalImage(node);
        __instance.currentNode = node;

        __instance.screenText.text = builder.ToString();
        __instance.currentText = builder.ToString();

        __instance.textAdded = 0;

        Settings.firstUse = false;
        Variables.LastReplacedNode = node;

        return false;
      }

      return true;
    }
  }
}
