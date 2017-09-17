namespace Prerelease.Main.Physics
{
    public class Weapon
    {
        public bool CanFire => Cooldown <= 0;
        public int Cooldown { get; set; }

        public void Update(float timestep)
        {
            if(Cooldown > 0)
                Cooldown--;
        }
    }
}