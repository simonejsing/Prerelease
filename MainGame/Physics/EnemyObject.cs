using Contracts;
using VectorMath;

namespace Prerelease.Main.Physics
{
    public class EnemyObject : MovableObject
    {
        public int HitPoints { get; set; }
        public bool Dead => HitPoints <= 0;

        public EnemyObject(ActionQueue actionQueue, Vector2 startingPosition, Vector2 size) : base(actionQueue, startingPosition, size)
        {
            HitPoints = 10;
        }
    }
}