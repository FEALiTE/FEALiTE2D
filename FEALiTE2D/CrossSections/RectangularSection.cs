using FEALiTE2D.Materials;

namespace FEALiTE2D.CrossSections
{
    /// <summary>
    /// Represents a Solid Rectangular Cross-Section.
    /// </summary>
    [System.Serializable]
    public class RectangularSection : IFrame2DSection
    {
        /// <summary>
        /// Creates new instance of a <see cref="RectangularSection"/>.
        /// </summary>
        /// <param name="b">Width of the <see cref="RectangularSection"/>.</param>
        /// <param name="t">Height of the <see cref="RectangularSection"/>.</param>
        /// <param name="material">Material of the rectangle section.</param>
        public RectangularSection(double b, double t, IMaterial material)
        {
            this.B = b;
            this.T = t;
            Material = material;

            // calculate section properties and set them here 
            // to avoid recalculation in each time they are called to save time.
            SetSectionProperties(b, t);
        }

        /// <summary>
        /// Width of the Rectangle section.
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// Height of the Rectangle section.
        /// </summary>
        public double T { get; set; }

        /// <summary>
        /// Sets the section properties.
        /// </summary>
        /// <param name="width">The b.</param>
        /// <param name="thickness">The t.</param>
        private void SetSectionProperties(double width, double thickness)
        {
            A = width * thickness;
            Ay = Ax = A * 5.0 / 6.0;
            Ix = width * thickness * thickness * thickness / 12.0;
            Iy = width * width * width * thickness / 12.0;
            var t = System.Math.Max(width, thickness);
            var b = System.Math.Min(width, thickness);
            var beta = 1.0 / 3.0 - 0.21 * (b / t) * (1 - System.Math.Pow(b / thickness, 4) / 12.0);
            J = beta * b * b * b * t;
            base.MaxWidth = width;
            base.MaxHeight = thickness;
        }
    }
}
