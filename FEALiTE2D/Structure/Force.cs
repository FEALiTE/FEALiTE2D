namespace FEALiTE2D.Structure
{
    /// <summary>
    /// Defines a force of 3 components (Fx, Fy, Mz)
    /// </summary>
    [System.Serializable]
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

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Force))
                return false;

            Force f = obj as Force;
            if (System.Math.Abs(f.Fx - this.Fx) > 1e-8 ||
                System.Math.Abs(f.Fy - this.Fy) > 1e-8 ||
                    System.Math.Abs(f.Mz - this.Mz) > 1e-8)
            {
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return
                $"Fx = {System.Math.Round(Fx, 8)} \r\n".GetHashCode() +
                $"Fy = {System.Math.Round(Fy, 8)} \r\n".GetHashCode() +
                $"Mz = {System.Math.Round(Mz, 8)} \r\n".GetHashCode();
        }
    }
}
