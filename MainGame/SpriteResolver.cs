using System.Collections.Generic;
using Contracts;
using Prerelease.Main.Physics;

namespace Prerelease.Main
{
    public class SpriteResolver
    {
        private readonly IRenderScope scope;

        public SpriteResolver(IRenderScope scope)
        {
            this.scope = scope;
        }

        public void ResolveBindings(IEnumerable<Object> objects)
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

            ResolveBindings(level.AllObjects);
        }
    }
}