using System;
using System.Collections.Generic;

namespace TerminalFormatter
{
    public abstract class TerminalFormatterNode
    {
        public string name;
        public List<string> terminalNode;

        public abstract bool IsNodeValid(TerminalNode node, Terminal terminal);

        public abstract string GetNodeText(TerminalNode node, Terminal terminal);

        // constructor
        public TerminalFormatterNode(string name, List<string> terminalNode)
        {
            this.name = name;
            this.terminalNode = terminalNode;

            Settings.RegisteredNodes.Add(this);
            Plugin.logger.LogWarning($"Registered node {name}");
        }
    }
}
