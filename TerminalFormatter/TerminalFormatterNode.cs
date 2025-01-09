using System.Collections.Generic;
using BepInEx.Configuration;

namespace TerminalFormatter
{
  public abstract class TerminalFormatterNode
  {
    public string name;
    public string AdditionalInfo = null;
    public List<string> terminalNode;
    public ConfigEntry<bool> Enabled;

    public virtual bool IsNodeValid(TerminalNode node, Terminal terminal)
    {
      return true;
    }

    public abstract string GetNodeText(TerminalNode node, Terminal terminal);

    // constructor
    public TerminalFormatterNode(string name, List<string> terminalNode)
    {
      this.name = name;
      this.terminalNode = terminalNode;
      this.Enabled = ConfigManager.configFile.Bind("Nodes", name, true, $"Enable node {name}");

      Settings.RegisteredNodes.Add(this);
      Plugin.logger.LogInfo($"Registered node {name}");
    }
  }
}
