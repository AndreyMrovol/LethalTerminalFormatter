namespace TerminalFormatter
{
  public class Variables
  {
    public static Terminal Terminal => TerminalUtils.TerminalManager.Terminal;

    internal static TerminalNode CurrentNode;

    public static TerminalNode LastReplacedNode = null;
  }
}
