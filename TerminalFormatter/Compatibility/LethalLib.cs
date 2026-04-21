namespace TerminalFormatter.Compatibility
{
  internal class LethalLibCompatibility : MrovLib.CompatibilityHandler
  {
    public LethalLibCompatibility(string guid, string version = null)
      : base(guid, version) { }

    public static bool IsLLItemDisabled(Item item)
    {
      // is LL item?
      LethalLib.Modules.Items.ShopItem shopItem = LethalLib.Modules.Items.shopItems.Find(x => x.item == item);

      if (shopItem == null)
      {
        return false;
      }

      return shopItem.wasRemoved;
    }

    public static bool IsLLUpgradeDisabled(UnlockableItem unlockable)
    {
      LethalLib.Modules.Unlockables.RegisteredUnlockable registeredUnlockable = LethalLib.Modules.Unlockables.registeredUnlockables.Find(x =>
        x.unlockable == unlockable
      );

      if (registeredUnlockable == null)
      {
        return false;
      }

      return registeredUnlockable.disabled;
    }
  }
}
