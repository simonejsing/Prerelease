using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using VectorMath;

namespace CraftingGame.Physics.Items
{
    public sealed class ConsumableFlower : Item
    {
        public override string Name => nameof(ConsumableFlower);
        public override bool Consumable => true;
        public override bool Placable => false;

        public ConsumableFlower()
        {
        }

        static ConsumableFlower()
        {
            ItemFactory.Register(nameof(ConsumableFlower), () => new ConsumableFlower());
        }
    }
}
