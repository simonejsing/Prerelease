using CraftingGame.Physics;

namespace CraftingGame.Items
{
    internal class NoopItem : StackableItemBase, IEquipableItem
    {
        public override string Name => "Noop";
        public override bool Consumable => false;
        public override bool Placable => false;

        public bool OnCooldown => false;

        public NoopItem()
        {
        }

        public void Attack()
        {
        }

        public void Update()
        {
        }

        public void Reset()
        {
        }

        public void Equip(PlayerObject player)
        {
        }
    }
}