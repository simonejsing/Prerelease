using VectorMath;

namespace CraftingGame.Physics
{
    public delegate void ObjectCollisionEventHandler(object sender, ICollidableObject target, Collision collision);
    public delegate void GridCollisionEventHandler(object sender, ICollidableObject[] target, Collision collision);
    public delegate void HitEventHandler(object sender, IProjectile target);

    public interface ICollidableObject
    {
        event ObjectCollisionEventHandler ObjectCollision;
        event GridCollisionEventHandler GridCollision;
        event HitEventHandler Hit;

        void OnObjectCollision(ICollidableObject target, Collision collision);
        void OnGridCollision(ICollidableObject[] target, Collision collision);
        void OnHit(IProjectile target);

        Rect2 BoundingBox { get; }
        Vector2 Position { get; }
        Vector2 Size { get; }
        Vector2 Center { get; }
        bool Occupied { get; }
    }
}