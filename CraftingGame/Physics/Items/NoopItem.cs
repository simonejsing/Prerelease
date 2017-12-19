namespace CraftingGame.Physics.Items
{
    internal class NoopItem : Item
    {
        public override string Name => "Noop";
        public override bool Consumable => false;
        public override bool Placable => false;

        public NoopItem()
        {
        }
    }
}