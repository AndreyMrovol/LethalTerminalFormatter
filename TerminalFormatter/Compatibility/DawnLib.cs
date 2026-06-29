using Dawn;

namespace TerminalFormatter.Compatibility
{
  internal class DawnLibCompatibility : MrovLib.CompatibilityHandler
  {
    public DawnLibCompatibility(string guid, string version = null)
      : base(guid, version) { }

    public (bool locked, bool hidden) GetLevelStatus(SelectableLevel level)
    {
      return TerminalUtils.Plugin.DawnCompatibility.GetLevelStatus(level);
    }
  }
}
