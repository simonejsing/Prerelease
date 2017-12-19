using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using VectorMath;
using CraftingGame.Physics;

namespace CraftingGame.Items
{
    public sealed class ConsumableFlower : ItemBase
    {
        public override string Name => nameof(ConsumableFlower);
        public override bool Consumable => true;
        public override bool Placable => false;

        internal ConsumableFlower()
        {
        }

        public override void Consume(PlayerObject player)
        {
            player.HitPoints = 0;
        }
    }
}
