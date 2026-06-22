using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrovLib.ContentType;
using UnityEngine;

namespace TerminalFormatter.Nodes
{
  public class Store : TerminalFormatterNode
  {
    public Store()
      : base("Store", ["0_StoreHub"])
    {
      this.HelpText = " Welcome to the Company store. \n Use words BUY and INFO on any item. \n Order items in bulk by typing a number.";
    }

    public override string GetNodeText(TerminalNode node)
    {
      var table = new ConsoleTables.ConsoleTable("Name", "Price", "Owned");

      GameObject ship = GameObject.Find("/Environment/HangarShip");
      var ItemsOnShip = ship.GetComponentsInChildren<GrabbableObject>().ToList();

      bool decor = ConfigManager.ShowDecorations.Value;

      string headerName = "COMPANY STORE";
      string storeHeader = new Header().CreateHeaderWithoutLines(headerName);
      StringBuilder stringBuilder = new StringBuilder().Append(storeHeader);

      if (ConfigManager.ShowHelpText.Value)
      {
        stringBuilder.Append(this.HelpText != null ? $"\n{this.HelpText}\n" : "");
      }

      PurchaseType[] desiredOrder =
      [
        PurchaseType.Item,
        PurchaseType.Vehicle,
        PurchaseType.Unlockable,
        PurchaseType.Decoration,
        PurchaseType.Suit
      ];

      Dictionary<PurchaseType, List<BuyableThing>> groupedThings = TerminalUtils
        .TerminalManager.GetCurrentStoreItems()
        .GroupBy(thing => thing.Type)
        .OrderBy(group =>
        {
          int idx = Array.IndexOf(desiredOrder, group.Key);
          return idx == -1 ? int.MaxValue : idx;
        })
        .ToDictionary(
          group => group.Key,
          group =>
            group
              .Where(item =>
              {
                switch (item.Type)
                {
                  case PurchaseType.Unlockable:
                    BuyableUnlockable unlockable = (BuyableUnlockable)item;
                    return !unlockable.IsUnlocked;
                  case PurchaseType.Decoration:
                    BuyableDecoration decoration = (BuyableDecoration)item;
                    return decoration.InRotation && !decoration.IsUnlocked;
                  case PurchaseType.Suit:
                    BuyableSuit suit = (BuyableSuit)item;
                    return suit.InRotation && !suit.IsUnlocked;
                  default:
                    return true;
                }
              })
              .ToList()
        );

      foreach (var group in groupedThings)
      {
        if (group.Value.Count == 0)
        {
          continue;
        }

        int itemCount = 1;

        table.AddRow("", "", "");
        table.AddRow($"[{group.Key.ToString().ToUpperInvariant()}S]", "", "");
        if (decor)
        {
          table.AddRow($"{new string('-', Settings.dividerLength)}", "", "");
        }

        for (int i = 0; i < group.Value.Count; i++)
        {
          var thing = group.Value[i];
          string name = thing.Name;
          string priceWithDiscount = $"${thing.Price}";
          string howManyOnShip = "";

          if (decor)
          {
            name = $"* {name}";
          }

          if (thing.Type == PurchaseType.Item)
          {
            BuyableItem item = (BuyableItem)thing;
            if (item.Discount != 0)
            {
              int discount = item.Discount;
              string discountPercent = item.Discount != 0 ? $" {(decor ? "(" : "")}-{discount}%{(decor ? ")" : "")}" : "";

              if (name.Length + discountPercent.Length > Settings.itemNameWidth)
              {
                name = name.Substring(0, Settings.itemNameWidth - 4 - discountPercent.Length) + "... " + discountPercent;
              }
              else
              {
                name = $"{name.PadRight(Settings.itemNameWidth - discountPercent.Length)}{discountPercent}".PadRight(Settings.itemNameWidth);
              }
            }

            howManyOnShip = ItemsOnShip.FindAll(x => x.itemProperties.itemName == item.Item.itemName).Count.ToString("D2");
          }

          table.AddRow(
            $"{name.PadRight(Settings.itemNameWidth)}",
            $"{priceWithDiscount}",
            $"{((howManyOnShip == "" || howManyOnShip == "00") ? "" : $"×{howManyOnShip}")}"
          );

          if (ConfigManager.DivideShopPage.Value != 0 && i != group.Value.Count - 1)
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
      }

      string tableString = table.ToStringCustomDecoration(header: true, divider: true).TrimEnd();
      stringBuilder.Append(tableString);
      return stringBuilder.ToString().TrimEnd();
    }
  }
}
