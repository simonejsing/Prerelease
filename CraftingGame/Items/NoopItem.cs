namespace CraftingGame.Items
{
    internal class NoopItem : StackableItemBase
    {
        public override string Name => "Noop";
        public override bool Consumable => false;
        public override bool Placable => false;

        public NoopItem()
        {
        }
    }
}