using CraftingGame.Physics.Items;
using VectorMath;

namespace CraftingGame.Physics
{
    public delegate void CollectEventHandler(object sender, ICollectableObject source, ICollectingObject target);

    public interface ICollectableObject
    {
        void OnCollect(ICollectingObject target);

        Rect2 BoundingBox { get; }
        Item Item { get; }
        bool PickedUp { get; }
    }
}