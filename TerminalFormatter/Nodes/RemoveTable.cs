using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TerminalFormatter
{
    partial class Nodes
    {
        internal static string RemoveTable(string input, bool removeEmptyLines = true)
        {
            string output = input;
            Plugin.logger.LogWarning("input:\n" + input);

            Regex tableOutline = new(@"(\|)|(\-{2,})", RegexOptions.Multiline);
            output = tableOutline.Replace(input, "");

            if (removeEmptyLines)
            {
                Regex emptyLine = new(@"^\s*\n", RegexOptions.Multiline);
                output = emptyLine.Replace(output, "");
            }

            return output;
        }
    }
}
