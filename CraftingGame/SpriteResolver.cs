using System;
using System.Collections.Generic;
using Contracts;
using CraftingGame.Physics;
using Object = CraftingGame.Physics.Object;
using System.Linq;

namespace CraftingGame
{
    public class SpriteResolver
    {
        private readonly IRenderScope scope;

        public SpriteResolver(IRenderScope scope)
        {
            if(scope == null)
                throw new ArgumentNullException(nameof(scope));

            this.scope = scope;
        }

        public void ResolveBindings(params Object[] objects)
        {
            foreach (var obj in objects)
            {
                if (!obj.SpriteBinding.Resolved)
                {
                    obj.SpriteBinding = scope.ResolveSprite(obj.SpriteBinding);
                }
            }
        }

        public void ResolveBindings(LevelState level)
        {
            level.Blocks.SpriteBinding = scope.ResolveSprite(level.Blocks.SpriteBinding);

            ResolveBindings(level.AllObjects.ToArray());
        }
    }
}