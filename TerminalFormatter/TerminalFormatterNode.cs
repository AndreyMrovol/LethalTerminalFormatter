using System.Collections.Generic;

namespace TerminalFormatter
{
  public abstract class TerminalFormatterNode : TerminalUtils.Definitions.TerminalNodeReplacement
  {
    public List<string> NodeNamesToMatch { get; internal set; } = [];

    // constructors
    public TerminalFormatterNode(string name, List<string> nodeNames)
      : base(name, null, ConfigManager.configFile.Bind("Nodes", name, true, $"Enable node {name}"))
    {
      this.NodeNamesToMatch = nodeNames;
      Settings.RegisteredNodes.Add(this);
      Plugin.debugLogger.LogInfo($"Registered node {name}");
    }

    public TerminalFormatterNode(string name, TerminalNode terminalNode)
      : base(name, terminalNode, ConfigManager.configFile.Bind("Nodes", name, true, $"Enable node {name}"))
    {
      Settings.RegisteredNodes.Add(this);
      Plugin.debugLogger.LogInfo($"Registered node {name}");
    }
  }
}
