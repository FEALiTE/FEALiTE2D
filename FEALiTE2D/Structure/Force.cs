namespace FEALiTE2D.Structure
{
    /// <summary>
    /// Defines a force of 3 components (Fx, Fy, Mz)
    /// </summary>
    public class Force
    {
        /// <summary>
        /// Gets or sets the fx.
        /// </summary>
        public double Fx { get; set; }

        /// <summary>
        /// Gets or sets the fy.
        /// </summary>
        public double Fy { get; set; }

        /// <summary>
        /// Gets or sets the mz.
        /// </summary>
        public double Mz { get; set; }

        /// <summary>
        /// Implements the operator + on 2 forces.
        /// </summary>
        /// <param name="f1">first force.</param>
        /// <param name="f2">second force.</param>
        public static Force operator +(Force f1, Force f2) => new Force { Fx = f1.Fx + f2.Fx, Fy = f1.Fy + f2.Fy, Mz = f1.Mz + f2.Mz };

        /// <summary>
        /// Implements the operator - on 2 forces.
        /// </summary>
        /// <param name="f1">first force.</param>
        /// <param name="f2">second force.</param>
        public static Force operator -(Force f1, Force f2) => new Force { Fx = f1.Fx - f2.Fx, Fy = f1.Fy - f2.Fy, Mz = f1.Mz - f2.Mz };

        /// <summary>
        /// Implements the operator number*force.
        /// </summary>
        /// <param name="factor">factor.</param>
        /// <param name="f">force.</param>
        public static Force operator *(double factor, Force f) => new Force { Fx = factor * f.Fx, Fy = factor * f.Fy, Mz = factor * f.Mz };

        /// <summary>
        /// Convert To vector.
        /// </summary>
        public double[] ToVector()
        {
            return new[] { Fx, Fy, Mz };
        }

        /// <summary>
        /// Create force from a given vector.
        /// </summary>
        /// <param name="f">a Vector containing force</param>
        public static Force FromVector(double[] f)
        {
            if (f.Length == 3)
            {
                return new Force() { Fx = f[0], Fy = f[1], Mz = f[2] };
            }
            return null;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return
                $"Fx = {Fx} \r\n" +
                $"Fy = {Fy} \r\n" +
                $"Mz = {Mz} \r\n";
        }
    }
}
