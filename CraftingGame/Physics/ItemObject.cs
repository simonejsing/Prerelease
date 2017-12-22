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
        public bool Collected { get; set; }

        public event CollectEventHandler Collect;

        public ItemObject(ActionQueue actionQueue, StackableItemBase item) : base(actionQueue)
        {
            this.Item = item;
        }

        public void OnCollect(ICollectingObject target)
        {
            Collect?.Invoke(this, this, target);
        }
    }
}
