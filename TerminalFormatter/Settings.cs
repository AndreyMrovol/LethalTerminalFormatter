using System.Collections.Generic;
using BepInEx.Logging;

namespace TerminalFormatter
{
  internal class Settings
  {
    internal static List<TerminalFormatterNode> RegisteredNodes = [];

    public static readonly int terminalWidth = 48;
    public static bool firstUse = true;

    internal static readonly int planetWeatherWidth = 18;
    internal static readonly int planetNameWidth = terminalWidth + 2 - planetWeatherWidth - 9;

    internal static readonly int itemNameWidth = terminalWidth - 9 - 10;

    internal static readonly int dividerLength = 17;
  }
}
