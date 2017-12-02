namespace CraftingGame.Physics
{
    public class Weapon
    {
        public bool CanFire => Cooldown <= 0;
        public int Cooldown { get; set; }

        public void Update()
        {
            if(Cooldown > 0)
                Cooldown--;
        }
    }
}