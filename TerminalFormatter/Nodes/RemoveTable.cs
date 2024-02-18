using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TerminalFormatter
{
    partial class Nodes
    {
        private static string RemoveTable(string input)
        {
            // Plugin.logger.LogWarning("\n" + input);

            Regex tableOutline = new(@"(\|)|(\-{2,})", RegexOptions.Multiline);

            return tableOutline.Replace(input, "");
        }
    }
}
