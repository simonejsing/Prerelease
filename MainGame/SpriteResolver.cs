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

        public void ResolveBindings(LevelState level)
        {
            level.Blocks.SpriteBinding = scope.ResolveSprite(level.Blocks.SpriteBinding);

            foreach (var obj in level.AllObjects)
            {
                if (!obj.SpriteBinding.Resolved)
                {
                    obj.SpriteBinding = scope.ResolveSprite(obj.SpriteBinding);
                }
            }
        }
    }
}