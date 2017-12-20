using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using VectorMath;
using CraftingGame.Items;

namespace CraftingGame.Physics
{
    public class ItemObject : StaticObject, ICollectableObject
    {
        public StackableItemBase Item { get; }
        public bool PickedUp { get; set; }

        public event CollectEventHandler Collect;

        public ItemObject(ActionQueue actionQueue, Plane startingPlane, IReadonlyVector startingPosition, IReadonlyVector size, StackableItemBase item) : base(actionQueue, startingPlane, startingPosition, size)
        {
            this.Item = item;
        }

        public void OnCollect(ICollectingObject target)
        {
            Collect?.Invoke(this, this, target);
        }
    }
}
