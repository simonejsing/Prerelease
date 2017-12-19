using CraftingGame.Physics;
using VectorMath;

namespace CraftingGame
{
    public class Camera
    {
        public enum CameraMode { Free, Follow }

        private readonly ViewportProjection view;
        private Object trackingObject;

        public CameraMode Mode { get; private set; }

        public Camera(ViewportProjection view)
        {
            this.Mode = CameraMode.Free;
            this.view = view;
        }

        public void Update()
        {
            if (this.Mode == CameraMode.Follow && trackingObject != null)
            {
                this.view.Center(trackingObject.Center);
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

        public void Track(Object obj)
        {
            trackingObject = obj;
        }
    }
}