namespace CraftingGame.Physics
{
    public interface ICollidableObjectGrid
    {
        ICollidableObject[] Neighbors(Object obj);
    }
}