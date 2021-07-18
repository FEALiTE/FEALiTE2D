using System;
using netDxf;

namespace FEALiTE2D.Plotting.Dxf
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Get Point location that lies on an element at a given distance from it's start node.
        /// </summary>
        /// <param name="element">an element</param>
        /// <param name="x">distance measured from start node of the <see cref="Elements.IElement"/></param>
        /// <returns>a point that lies on the element from a given distance from it's start node.</returns>
        public static Vector2 PointLocationOnLine(this Elements.IElement element, double x)
        {
            double x1 = element.Nodes[0].X;
            double y1 = element.Nodes[0].Y;
            double x2 = element.Nodes[1].X;
            double y2 = element.Nodes[1].Y;
            double l = element.Length;
            double t = x / l;

            return new Vector2() { X = (1 - t) * x1 + t * x2, Y = (1 - t) * y1 + t * y2 };
        }

        /// <summary>
        /// Get point location which any line passes through this point in direction of an element will be perpendicular to the element.
        /// This will be useful to find point location when drawing SFD, BMD for element when they are inclined.
        /// </summary>
        /// <param name="element">an element</param>
        /// <param name="x">distance measured from start node of the <see cref="Elements.IElement"/></param>
        /// <param name="perDistance">the perpendicular distance on the element.</param>
        public static Vector2 PointPerpendicularToLine(this Elements.IElement element, double x, double perDistance)
        {
            double x1 = element.Nodes[0].X;
            double y1 = element.Nodes[0].Y;
            double x2 = element.Nodes[1].X;
            double y2 = element.Nodes[1].Y;

            // find slope of the element
            double m1 = (y2 - y1) / (x2 - x1);

            // find required point a long the element.
            Vector2 startPerPoint = PointLocationOnLine(element, x);
            double m2 = -1 / m1;
            double theta = Math.Atan(m2);
            return new Vector2() { X = startPerPoint.X + perDistance * Math.Cos(theta), Y = startPerPoint.Y + perDistance * Math.Sin(theta) };
        }

        /// <summary>
        /// Get deformed point location along an element
        /// </summary>
        /// <param name="element">an element</param>
        /// <param name="x">distance measured from start node of the <see cref="Elements.IElement"/></param>
        /// <param name="dx">displacement in x-direction</param>
        /// <param name="dy">displacement in y-direction</param>
        public static Vector2 PointForDeflection(this Elements.IElement element, double x, double dx, double dy)
        {
            // find location of required point along the frame after adding dx
            return PointPerpendicularToLine(element, x + dx, -dy);
        }
    }
}
