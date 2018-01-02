using Contracts;
using System;
using VectorMath;

namespace CraftingGame.Controllers
{
    internal class FreeCameraController
    {
        public const int ScrollSpeed = 50;

        private readonly Func<Camera> cameraFunc;

        public FreeCameraController(Func<Camera> cameraFunc)
        {
            this.cameraFunc = cameraFunc;
        }

        public void Update(InputMask inputMask)
        {
            var input = inputMask.Input;
            var translation = new Vector2();
            if (input.Up)
                translation += new Vector2(0, ScrollSpeed);
            else if (input.Down)
                translation += new Vector2(0, -ScrollSpeed);
            if (input.Left)
                translation += new Vector2(-ScrollSpeed, 0);
            else if (input.Right)
                translation += new Vector2(ScrollSpeed, 0);

            if (translation != Vector2.Zero)
            {
                var camera = cameraFunc();
                camera?.Free();
                camera?.Translate(translation);
            }

            inputMask.Reset();
        }
    }
}
