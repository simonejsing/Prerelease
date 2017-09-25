using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prerelease.Main.Physics
{
    public static class Constants
    {
        public const float GRAVITY = 0.2f;
        public const float MAX_VERTICAL_VELOCITY = 8.0f;
        public const float MAX_HORIZONTAL_VELOCITY = 3.0f;

        public const float JUMP_SPEED = -5.5f;

        public const float GROUND_ACCELERATION = 0.5f;
        public const float AIR_ACCELERATION = 0.1f;

        public const float PROJECTILE_VELOCITY = 5.0f;
        public const float ENEMY_VELOCITY = 3.0f;
    }
}
