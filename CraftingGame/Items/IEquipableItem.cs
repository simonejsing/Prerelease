using CraftingGame.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CraftingGame.Items
{
    public interface IEquipableItem
    {
        bool OnCooldown { get; }

        void Attack();
        void Equip(PlayerObject player);

        void Update();
        void Reset();
    }
}
