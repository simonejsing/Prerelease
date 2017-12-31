using CraftingGame.Physics;
using VectorMath;

namespace CraftingGame
{
    public class Camera
    {
        public enum CameraMode { Free, Follow }

        private readonly ViewportProjection view;
        private Object[] trackingObjects = new Object[0];

        public CameraMode Mode { get; private set; }

        public Camera(ViewportProjection view)
        {
            this.Mode = CameraMode.Free;
            this.view = view;
        }

        public void Update()
        {
            if (this.Mode == CameraMode.Follow && trackingObjects.Length > 0)
            {
                var center = Vector2.Zero;
                foreach(var obj in trackingObjects)
                {
                    center += obj.Center;
                }
                this.view.Center(center / trackingObjects.Length);
            }
        }

        public void Free()
        {
            this.Mode = CameraMode.Free;
        }

        public void Translate(IReadonlyVector translation)
        {
            view.Translate(translation);
        }

        public void Follow()
        {
            this.Mode = CameraMode.Follow;
        }

        public void Track(params Object[] objs)
        {
            trackingObjects = objs;
        }
    }
}