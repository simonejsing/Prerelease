using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMath
{
    public class Spline2
    {
        private readonly PointVector2[] _segments;

        public IEnumerable<PointVector2> Segments
        {
            get { return _segments; }
        }

        public Spline2(Vector2 origin, IEnumerable<Vector2> steps, bool returnSegment = false)
        {
            _segments = Trace(origin, steps, returnSegment).ToArray();
        }

        public static IEnumerable<PointVector2> Trace(Vector2 origin, IEnumerable<Vector2> steps, bool returnSegment = false)
        {
            if(!steps.Any() && returnSegment)
                throw new ArgumentException("Cannot trace spline with return segment when steps is empty");

            var start = new Vector2(origin);

            foreach (var step in steps)
            {
                yield return new PointVector2(start, step);
                start += step;
            }

            // Return a final segment that goes back to the origin
            if(returnSegment)
                yield return new PointVector2(start, origin - start);
        }
    }
}
