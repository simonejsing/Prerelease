using VectorMath;

namespace CraftingGame.Physics
{
    public delegate void CollectEventHandler(object sender, ICollectingObject target);

    public interface ICollectableObject
    {
        void OnCollect(ICollectingObject target);

        Rect2 BoundingBox { get; }
        bool PickedUp { get; }
    }
}