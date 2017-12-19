using CraftingGame.Items;
using VectorMath;

namespace CraftingGame.Physics
{
    public delegate void CollectEventHandler(object sender, ICollectableObject source, ICollectingObject target);

    public interface ICollectableObject
    {
        void OnCollect(ICollectingObject target);

        Rect2 BoundingBox { get; }
        ItemBase Item { get; }
        bool PickedUp { get; }
    }
}