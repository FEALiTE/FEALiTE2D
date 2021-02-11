using FEALiTE2D.Materials;
using static System.Math;

namespace FEALiTE2D.CrossSections
{
    /// <summary>
    /// Represents a Solid Rectangular Cross-Section.
    /// </summary>
    public class RectangularSection : IFrame2DSection
    {
        /// <summary>
        /// Creates new instance of a <see cref="RectangularSection"/>.
        /// </summary>
        /// <param name="b">Width of the <see cref="RectangularSection"/>.</param>
        /// <param name="t">Height of the <see cref="RectangularSection"/>.</param>
        /// <param name="material">Material of the rectangle section.</param>
        public RectangularSection(double b, double t, IMaterial material) : base()
        {
            this.b = b;
            this.t = t;
            this.Material = material;

            // calculate section properties and set them here 
            // to avoid recalculation in each time they are called to save time.
            SetSectionProperties(b, t);
        }

        /// <summary>
        /// Width of the Rectangle section.
        /// </summary>
        public double b { get; set; }

        /// <summary>
        /// Height of the Rectangle section.
        /// </summary>
        public double t { get; set; }

        /// <summary>
        /// Sets the section properties.
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="t">The t.</param>
        private void SetSectionProperties(double b, double t)
        {
            this.A = b * t;
            this.Ay = this.Ax = this.A * 5.0 / 6.0;
            this.Ix = b * t * t * t / 12.0;
            this.Iy = b * b * b * t / 12.0;
            double _t = Max(b, t);
            double _b = Min(b, t);
            double beta = 1.0 / 3.0 - 0.21 * (_b / _t) * (1 - Pow(_b / t, 4) / 12.0);
            this.J = beta * _b * _b * _b * _t;
            base.MaxWidth = b;
            base.MaxHeight = t;
        }
    }
}
