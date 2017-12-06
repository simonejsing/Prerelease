namespace VectorMath
{
    public class Line2
    {
        public Vector2 Origin { get; set; }

        public Vector2 Normal { get; set; }

        public Line2(Vector2 origin, Vector2 normal)
        {
            Normal = normal.Normalize();
            Origin = origin;
        }

        /// <summary>
        /// Computes the intersection point between 'this' line and 'line'.
        /// If the lines are parallel either Infinity is returned unless the lines are overlapping, in which case NaN is returned
        /// </summary>
        /// <param name="line"></param>
        /// <returns>The length of where the intersection happened along 'this' line</returns>
        public float IntersectionFactor(Line2 line)
        {
            var components = new ComponentizedVector2(Normal.Hat(), line.Normal);
            var factor = Vector2.Dot(line.Normal, Origin - line.Origin) / Vector2.Dot(line.Normal, components.NormalComponent);
            return float.IsInfinity(factor) ? float.PositiveInfinity : factor;
        }

        /// <summary>
        /// Returns the vector that points from 'point' to the orthogonal intersection point on the line,
        /// such that point + result is the perpendicular projection of the point on the line
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 DistanceVector(Vector2 point)
        {
            return (Origin - point).ProjectOn(Normal);
        }

        /// <summary>
        /// Computes the projection of 'vector' onto this line
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public Vector2 Projection(Vector2 vector)
        {
            return vector.ProjectOn(Normal.Hat());
        }
    }
}
