using CraftingGame.Physics;
using CraftingGame.State;
using System;
using System.Collections.Generic;
using System.Text;
using Terrain;

namespace CraftingGame.Items
{
    public abstract class TerrainModifyingItem : ItemBase, IEquipableItem
    {
        protected GameState state;

        public abstract string Name { get; }
        public PlayerObject Wielder { get; private set; }
        public TerrainTarget Target { get; }

        public TerrainModifyingItem(GameState state)
        {
            this.state = state;
            this.Target = new TerrainTarget(state, ValidTerrainTarget);
        }

        public void Equip(PlayerObject player)
        {
            Wielder = player;
            Target.Equip(player);
        }

        public override void Update()
        {
            base.Update();
            Target.Update();
        }

        protected abstract bool ValidTerrainTarget(TerrainType type);

        public abstract void Attack();
    }
}
