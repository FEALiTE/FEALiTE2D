using System;

namespace FEALiTE2D.Plotter
{
    /// <summary>
    /// A Class That Represents An Arbitrary Point in 2D Space 
    /// </summary>
    public class Point2D
    {
        /// <summary>
        /// X-Coordinate of the point.
        /// </summary>
        public double x;

        /// <summary>
        /// Y-Coordinate of the point.
        /// </summary>
        public double y;

        /// <inheritdoc/>
        public override string ToString() => $"x={x}, y={y}";

        /// <summary>
        /// Get Point location that lies on an element at a given distance from it's start node.
        /// </summary>
        /// <param name="element">an element</param>
        /// <param name="x">distance measured from start node of the <see cref="Elements.IElement"/></param>
        /// <returns>a point that lies on the element from a given distance from it's start node.</returns>
        public static Point2D PointLocationOnLine(Elements.IElement element, double x)
        {
            double x1 = element.Nodes[0].X;
            double y1 = element.Nodes[0].Y;
            double x2 = element.Nodes[1].X;
            double y2 = element.Nodes[1].Y;
            double l = element.Length;
            double t = x / l;

            return new Point2D() { x = (1 - t) * x1 + t * x2, y = (1 - t) * y1 + t * y2 };
        }

        /// <summary>
        /// Get point location which any line passes through this point in direction of an element will be perpendicular to the element.
        /// This will be useful to find point location when drawing SFD, BMD for element when they are inclined.
        /// </summary>
        /// <param name="element">an element</param>
        /// <param name="x">distance measured from start node of the <see cref="Elements.IElement"/></param>
        /// <param name="perDistance">the perpendicular distance on the element.</param>
        /// <returns></returns>
        public static Point2D PointPerpendicularToLine(Elements.IElement element, double x, double perDistance)
        {
            double x1 = element.Nodes[0].X;
            double y1 = element.Nodes[0].Y;
            double x2 = element.Nodes[1].X;
            double y2 = element.Nodes[1].Y;

            // find slope of the element
            double m1 = (y2 - y1) / (x2 - x1);

            // find required point a long the element.
            Point2D startPerPoint = PointLocationOnLine(element, x);
            double m2 = -1 / m1;
            double theta = Math.Atan(m2);
            return new Point2D() { x = startPerPoint.x + perDistance * Math.Cos(theta), y = startPerPoint.y + perDistance * Math.Sin(theta) };
        }
    }
}
