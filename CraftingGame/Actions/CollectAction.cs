using CraftingGame.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingGame.Actions
{
    public class CollectAction
    {
        public void Invoke(object sender, ICollectableObject source, ICollectingObject target)
        {
            if (target.Inventory.Add(source.Item.Name))
            {
                source.Collected = true;
            }
        }
    }
}
