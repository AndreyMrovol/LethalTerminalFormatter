using Dawn;

namespace TerminalFormatter.Compatibility
{
  internal class DawnLibCompatibility : MrovLib.CompatibilityHandler
  {
    public DawnLibCompatibility(string guid, string version = null)
      : base(guid, version) { }

    public (bool locked, bool hidden) GetLevelStatus(SelectableLevel level)
    {
      return level.GetDawnInfo().DawnPurchaseInfo.PurchasePredicate.CanPurchase() switch
      {
        TerminalPurchaseResult.HiddenPurchaseResult hiddenResult => (hiddenResult.IsFailure, true),
        TerminalPurchaseResult.FailedPurchaseResult => (true, false),
        TerminalPurchaseResult.SuccessPurchaseResult => (false, false),
        _ => (false, false)
      };
    }
  }
}
