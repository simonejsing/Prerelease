using System;
using System.Linq;
using Contracts;
using CraftingGame.State;
using Object = CraftingGame.Physics.Object;

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