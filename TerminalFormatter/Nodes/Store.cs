using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrovLib;
using MrovLib.ContentType;
using TerminalFormatter.Compatibility;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
  public class Store : TerminalFormatterNode
  {
    public Store()
      : base("Store", ["0_StoreHub"])
    {
      this.AdditionalInfo = " Welcome to the Company store. \n Use words BUY and INFO on any item. \n Order items in bulk by typing a number.";
    }

    public override bool IsNodeValid(TerminalNode node, Terminal terminal)
    {
      return true;
    }

    public override string GetNodeText(TerminalNode node, Terminal terminal)
    {
      var table = new ConsoleTables.ConsoleTable("Name", "Price", "Owned");
      var adjustedTable = new StringBuilder();

      var ACServerConfiguration = Variables.IsACActive ? ACCompatibility.ServerConfiguration.GetValue(null) : null;

      GameObject ship = GameObject.Find("/Environment/HangarShip");
      var ItemsOnShip = ship.GetComponentsInChildren<GrabbableObject>().ToList();

      bool decor = ConfigManager.ShowDecorations.Value;

      string headerName = "COMPANY STORE";
      string storeHeader = new Header().CreateHeaderWithoutLines(headerName);

      adjustedTable.Append(storeHeader);

      if (ConfigManager.ShowHelpText.Value)
      {
        adjustedTable.Append(this.AdditionalInfo != null ? $"\n{this.AdditionalInfo}\n\n" : "");
      }

      #region Items
      table.AddRow("[ITEMS]", "", "");
      if (decor)
      {
        table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
      }

      List<BuyableItem> sortedBuyableItemList = ContentManager.Items.OrderBy(x => x.Item.itemName).ToList();

      int itemCount = 1;
      // every 3 items make a space

      // [buyableItemsList]
      foreach (BuyableItem buyable in sortedBuyableItemList)
      {
        Item item = buyable.Item;

        var index = buyable.Nodes.Node.buyItemIndex;
        var itemName = item.itemName;
        int howManyOnShip = ItemsOnShip.FindAll(x => x.itemProperties.itemName == item.itemName).Count;
        int discount = buyable.Discount;

        if (index == -1)
        {
          continue;
        }

        if (decor)
        {
          itemName = $"* {itemName}";
        }

        if (Plugin.isLLibPresent)
        {
          if (LethalLibCompatibility.IsLLItemDisabled(item))
          {
            continue;
          }
        }

        if (ACCompatibility.Items.ContainsKey(itemName))
        {
          if (!(bool)ACCompatibility.Items[itemName])
          {
            Plugin.logger.LogDebug($"Item {itemName} is disabled");
            continue;
          }
        }

        string discountPercent = buyable.Discount != 0 ? $" {(decor ? "(" : "")}-{discount}%{(decor ? ")" : "")}" : "";

        // what i want to do:
        // itemName [some spaces] ... [discountPercent]
        // so the discountPercent is padded to the right

        // make itemName length = itemNameWidth
        if (itemName.Length + discountPercent.Length > Settings.itemNameWidth)
        {
          itemName = itemName.Substring(0, Settings.itemNameWidth - 4 - discountPercent.Length) + "... " + discountPercent;
        }
        else
        {
          itemName = $"{itemName.PadRight(Settings.itemNameWidth - discountPercent.Length)}{discountPercent}".PadRight(Settings.itemNameWidth);
        }

        table.AddRow(
          itemName,
          $"${(int)(item.creditsWorth * buyable.DiscountPercentage)}",
          $"{(howManyOnShip == 0 ? "" : $"×{howManyOnShip.ToString("D2")}")}"
        // $"{(terminal.itemSalesPercentages[index] != 100 ? 100 - terminal.itemSalesPercentages[index] : "")}"
        );

        if (ConfigManager.DivideShopPage.Value != 0)
        {
          if (itemCount % ConfigManager.DivideShopPage.Value == 0)
          {
            itemCount = 1;
            if (ConfigManager.ShowGroupDividerLines.Value)
            {
              table.AddRow("".PadRight(Settings.itemNameWidth, '-'), "".PadRight(5, '-'), "".PadRight(5, '-'));
            }
            else
            {
              table.AddRow("", "", "");
            }
          }
          else
          {
            itemCount++;
          }
        }
      }
      #endregion

      #region Upgrades
      List<BuyableUnlockable> unlockablesList = ContentManager.Unlockables.OrderBy(x => x.Unlockable.unlockableName).ToList();
      if (unlockablesList.Count > 0)
      {
        table.AddRow("", "", "");
        table.AddRow("[UPGRADES]", "", "");
        itemCount = 1;
        if (decor)
        {
          table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
        }
      }

      foreach (var buyable in unlockablesList)
      {
        UnlockableItem unlockable = buyable.Unlockable;
        bool isUnlocked = unlockable.hasBeenUnlockedByPlayer || unlockable.alreadyUnlocked;
        TerminalNode unlockableNode = buyable.Nodes.Node;

        string unlockableName = unlockable.unlockableName;

        if (decor)
        {
          unlockableName = $"* {unlockableName}";
        }

        if (Plugin.isLLibPresent)
        {
          if (LethalLibCompatibility.IsLLUpgradeDisabled(unlockable))
          {
            continue;
          }
        }

        if (isUnlocked)
        {
          continue;
        }

        table.AddRow(unlockableName.PadRight(Settings.itemNameWidth), $"${buyable.Price}", "");

        if (ConfigManager.DivideShopPage.Value != 0)
        {
          if (itemCount % ConfigManager.DivideShopPage.Value == 0)
          {
            itemCount = 1;
            if (ConfigManager.ShowGroupDividerLines.Value)
            {
              table.AddRow("".PadRight(Settings.itemNameWidth, '-'), "".PadRight(5, '-'), "".PadRight(5, '-'));
            }
            else
            {
              table.AddRow("", "", "");
            }
          }
          else
          {
            itemCount++;
          }
        }
      }

      #endregion

      #region Regeneration

      if (Plugin.isLRegenPresent)
      {
        if (!LethalRegenCompatibility.IsUpgradeBought() && LethalRegenCompatibility.IsUpgradeInStore)
        {
          table.AddRow("", "", "");
          table.AddRow("[REGENERATION]", "", "");
          if (decor)
          {
            table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
          }

          table.AddRow("Natural Regeneration", $"${LethalRegenCompatibility.GetCost()}", "");
        }
      }

      #endregion

      #region Vehicles

      table.AddRow("", "", "");
      table.AddRow("[VEHICLES]", "", "");
      itemCount = 1;
      if (decor)
      {
        table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
      }

      List<BuyableCar> sortedBuyableVehicleList = ContentManager.Vehicles.OrderBy(x => x.Name).ToList();

      foreach (var buyable in sortedBuyableVehicleList)
      {
        string vehicleName = buyable.Name;

        if (decor)
        {
          vehicleName = $"* {vehicleName}";
        }

        table.AddRow(vehicleName, $"${buyable.Price}", "");

        if (ConfigManager.DivideShopPage.Value != 0)
        {
          if (itemCount % ConfigManager.DivideShopPage.Value == 0)
          {
            itemCount = 1;
            if (ConfigManager.ShowGroupDividerLines.Value)
            {
              table.AddRow("".PadRight(Settings.itemNameWidth, '-'), "".PadRight(5, '-'), "".PadRight(5, '-'));
            }
            else
            {
              table.AddRow("", "", "");
            }
          }
          else
          {
            itemCount++;
          }
        }
      }

      #endregion

      #region Decorations
      List<BuyableDecoration> DecorSelection = ContentManager.Decorations.OrderBy(x => x.Name).Where(x => x.InRotation).ToList();
      // [unlockablesSelectionList]

      if (DecorSelection.Count > 0)
      {
        table.AddRow("", "", "");
        table.AddRow("[DECORATIONS]", "", "");
        if (decor)
        {
          table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
        }
      }

      itemCount = 1;

      foreach (var buyable in DecorSelection)
      {
        UnlockableItem unlockable = buyable.Decoration;
        // UnlockableItem unlockable = StartOfRound.Instance.unlockablesList.unlockables[
        //     decoration.shipUnlockableID
        // ];

        string decorationName = buyable.Name;

        if (decor)
        {
          decorationName = $"* {decorationName}";
        }

        if (Plugin.isLLibPresent)
        {
          if (LethalLibCompatibility.IsLLUpgradeDisabled(unlockable))
          {
            continue;
          }
        }

        if (unlockable.hasBeenUnlockedByPlayer || unlockable.alreadyUnlocked)
        {
          continue;
        }

        // StoreRotationConfig compatibility.
        if (Plugin.isSRCPresent)
        {
          int price = StoreRotationConfigCompatibility.GetDiscountedPrice(buyable, out int discount);

          if (discount > 0)
          {
            string discountPercent = $" {(decor ? '(' : "")}-{discount}%{(decor ? ')' : "")}";

            if (decorationName.Length + discountPercent.Length > Settings.itemNameWidth)
            {
              decorationName = decorationName[..(Settings.itemNameWidth - 4 - discountPercent.Length)] + "..." + discountPercent;
            }
            else
            {
              decorationName = $"{decorationName.PadRight(Settings.itemNameWidth - discountPercent.Length)}{discountPercent}".PadRight(
                Settings.itemNameWidth
              );
            }
          }

          table.AddRow(decorationName, $"${price}", "");
        }
        else
        {
          table.AddRow(decorationName, $"${buyable.Price}", "");
        }
        // ...

        if (ConfigManager.DivideShopPage.Value != 0)
        {
          if (itemCount % ConfigManager.DivideShopPage.Value == 0)
          {
            itemCount = 1;
            if (ConfigManager.ShowGroupDividerLines.Value)
            {
              table.AddRow("".PadRight(Settings.itemNameWidth, '-'), "".PadRight(5, '-'), "".PadRight(5, '-'));
            }
            else
            {
              table.AddRow("", "", "");
            }
          }
          else
          {
            itemCount++;
          }
        }
      }

      #endregion

      #region Suits
      List<BuyableSuit> SuitSelection = ContentManager.Suits.OrderBy(x => x.Name).Where(x => x.InRotation && !x.IsUnlocked).ToList();

      if (SuitSelection.Count > 0)
      {
        table.AddRow("", "", "");
        table.AddRow("[SUITS]", "", "");
        itemCount = 1;
        if (decor)
        {
          table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
        }
      }

      foreach (var buyable in SuitSelection)
      {
        UnlockableItem unlockable = buyable.Suit;
        string suitName = buyable.Name;

        if (decor)
        {
          suitName = $"* {suitName}";
        }

        if (buyable.IsUnlocked)
        {
          continue;
        }

        // StoreRotationConfig compatibility.
        if (Plugin.isSRCPresent)
        {
          int price = StoreRotationConfigCompatibility.GetDiscountedPrice(buyable, out int discount);

          if (discount > 0)
          {
            string discountPercent = $" {(decor ? '(' : "")}-{discount}%{(decor ? ')' : "")}";

            if (suitName.Length + discountPercent.Length > Settings.itemNameWidth)
            {
              suitName = suitName[..(Settings.itemNameWidth - 4 - discountPercent.Length)] + "..." + discountPercent;
            }
            else
            {
              suitName = $"{suitName.PadRight(Settings.itemNameWidth - discountPercent.Length)}{discountPercent}".PadRight(
                Settings.itemNameWidth
              );
            }
          }

          table.AddRow(suitName, $"${price}", "");
        }
        else
        {
          table.AddRow(suitName, $"${buyable.Price}", "");
        }
        // ...

        if (ConfigManager.DivideShopPage.Value != 0)
        {
          if (itemCount % ConfigManager.DivideShopPage.Value == 0)
          {
            itemCount = 1;
            if (ConfigManager.ShowGroupDividerLines.Value)
            {
              table.AddRow("".PadRight(Settings.itemNameWidth, '-'), "".PadRight(5, '-'), "".PadRight(5, '-'));
            }
            else
            {
              table.AddRow("", "", "");
            }
          }
          else
          {
            itemCount++;
          }
        }
      }

      #endregion

      table.AddRow("", "", "");

      //

      string tableString = table.ToStringCustomDecoration(header: true, divider: true);

      // Regex replaceHorizontal = new(@"^\|-+\|-+\|-+\|\n");
      // Regex middleLineReplace = new(@"(?:\ |\-)\|(?:\ |\-)");
      // Regex pipeReplace = new(@"\|");

      // replaceHorizontal.Replace(tableString, "");

      // string modifiedTableString = middleLineReplace.Replace(tableString, "   ");
      // modifiedTableString = pipeReplace.Replace(modifiedTableString, "").Replace("-", " ");

      adjustedTable.Append(tableString);

      string finalString = adjustedTable.ToString().TrimEnd();
      return finalString;
    }
  }
}
