using System;
using System.Collections.Generic;
using System.Text;
using CraftingGame.Actions;

namespace CraftingGame.Items
{
    public class ItemBase
    {
        protected int CooldownFrames { get; set; }
        public bool OnCooldown => CooldownFrames > 0;

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
