namespace Prerelease.Main.Physics
{
    public interface ICollidableObjectGrid
    {
        ICollidableObject[] Neighbors(Object obj);
    }
}