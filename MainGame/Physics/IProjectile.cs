namespace Prerelease.Main.Physics
{
    public interface IProjectile
    {
        Object Shooter { get; }
        bool Expired { get; }
    }
}