using UnityEngine;

namespace TerminalFormatter
{
  public class Variables
  {
    public static Terminal Terminal => TerminalUtils.TerminalManager.Terminal;

    internal static TerminalNode CurrentNode;

    public static TerminalNode LastReplacedNode = null;

    private static GameObject _shipCache;
    public static GameObject ShipCache
    {
      get
      {
        if (_shipCache == null)
        {
          _shipCache = GameObject.Find("/Environment/HangarShip");
        }

        return _shipCache;
      }
      set { _shipCache = value; }
    }
  }
}
