using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace TerminalFormatter
{
    public abstract class TerminalFormatterNode
    {
        public string name;
        public List<string> terminalNode;
        public ConfigEntry<bool> Enabled;

        public abstract bool IsNodeValid(TerminalNode node, Terminal terminal);

        public abstract string GetNodeText(TerminalNode node, Terminal terminal);

        // constructor
        public TerminalFormatterNode(string name, List<string> terminalNode)
        {
            this.name = name;
            this.terminalNode = terminalNode;
            this.Enabled = ConfigManager.configFile.Bind(
                "Nodes",
                name,
                true,
                $"Enable node {name}"
            );

            Settings.RegisteredNodes.Add(this);
            Plugin.logger.LogWarning($"Registered node {name}");
        }
    }
}
