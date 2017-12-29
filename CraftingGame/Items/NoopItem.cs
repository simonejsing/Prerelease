using CraftingGame.Physics;

namespace CraftingGame.Items
{
    internal class NoopItem : StackableItemBase, IEquipableItem
    {
        public override string Name => "Noop";
        public override bool Consumable => false;
        public override bool Placable => false;

        public override bool OnCooldown => false;

        public NoopItem()
        {
        }

        public void Attack()
        {
        }

        public override void Update()
        {
        }

        public void Equip(PlayerObject player)
        {
        }
    }
}