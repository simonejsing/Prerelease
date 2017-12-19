namespace CraftingGame.Items
{
    internal class NoopItem : ItemBase
    {
        public override string Name => "Noop";
        public override bool Consumable => false;
        public override bool Placable => false;

        public NoopItem()
        {
        }
    }
}