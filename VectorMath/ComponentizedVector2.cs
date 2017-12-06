namespace VectorMath
{
    public class ComponentizedVector2
    {
        private Vector2 _normal;
        private Vector2 _vector;

        public Vector2 NormalComponent { get; private set; }
        public Vector2 PerpendicularComponent { get; private set; }

        public Vector2 Normal
        {
            get
            {
                return _normal;
            }
            set
            {
                _normal = value.Normalize();
                UpdateComponents();
            }
        }

        public Vector2 Vector
        {
            get
            {
                return _vector;
            }
            set
            {
                _vector = value;
                UpdateComponents();
            }
        }

        private void UpdateComponents()
        {
            NormalComponent = Normal*Vector2.Dot(Normal, _vector);
            PerpendicularComponent = _vector - NormalComponent;
        }

        public ComponentizedVector2(Vector2 vector, Vector2 normal)
        {
            _normal = normal.Normalize();
            Vector = vector;
        }
    }
}
