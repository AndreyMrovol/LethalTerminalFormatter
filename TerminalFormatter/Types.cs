using System.Collections.Generic;

namespace TerminalFormatter
{
    public enum PurchaseType
    {
        Item,
        Unlockable,
        Decoration,
        Moon
    }

    public class RelatedNodes
    {
        public TerminalNode Node;
        public TerminalNode NodeConfirm;
        // public TerminalNode NodeInfo;
    }

    public class BuyableThing
    {
        public string Name;
        public int Price;
        public PurchaseType Type;

        public RelatedNodes Nodes;

        public BuyableThing(Terminal terminal, RelatedNodes nodes)
        {
            Plugin.logger.LogWarning($"BuyableThing constructor: {terminal}, {nodes}");

            Nodes = nodes;
        }
    }

    public class BuyableItem : BuyableThing
    {
        public Item Item;

        public BuyableItem(Terminal terminal, RelatedNodes nodes)
            : base(terminal, nodes)
        {
            Type = PurchaseType.Item;

            Item = terminal.buyableItemsList[nodes.Node.buyItemIndex];
            Price = Item.creditsWorth;
            Name = Item.itemName;
        }
    }

    public class BuyableUnlockable : BuyableThing
    {
        public UnlockableItem Unlockable;

        public BuyableUnlockable(Terminal terminal, RelatedNodes nodes)
            : base(terminal, nodes)
        {
            Type = PurchaseType.Unlockable;

            Unlockable = StartOfRound.Instance.unlockablesList.unlockables[
                Nodes.Node.shipUnlockableID
            ];
            Price =
                Nodes.Node != null
                    ? Nodes.Node.buyItemIndex
                    : Variables.upgrades[Unlockable.unlockableName];
            Name = Unlockable.unlockableName;
        }
    }

    public class BuyableDecoration : BuyableThing
    {
        public UnlockableItem Decoration;

        public BuyableDecoration(Terminal terminal, RelatedNodes nodes)
            : base(terminal, nodes)
        {
            Type = PurchaseType.Decoration;

            UnlockableItem decor = StartOfRound.Instance.unlockablesList.unlockables[
                Nodes.Node.shipUnlockableID
            ];

            Decoration = decor;
            Price = Decoration.shopSelectionNode.itemCost;
            Name = Decoration.unlockableName;
        }
    }
}
