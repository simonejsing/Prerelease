using System;
using System.Collections.Generic;
using System.Text;
using CraftingGame.Actions;

namespace CraftingGame.Items
{
    public abstract class ItemBase
    {
        public abstract string Name { get; }
        public virtual int Quantity => 1;
        protected int CooldownFrames { get; set; }
        public virtual bool OnCooldown => CooldownFrames > 0;

        public virtual void Update()
        {
            if (CooldownFrames > 0)
                CooldownFrames--;
        }

        public void Reset()
        {
            CooldownFrames = 0;
        }
    }
}
