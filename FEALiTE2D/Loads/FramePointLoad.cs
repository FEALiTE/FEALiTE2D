namespace FEALiTE2D.Loads
{
    /// <summary>
    /// Represents a class for <see cref="FramePointLoad"/>.
    /// </summary>
    public class FramePointLoad : ILoad
    {
        /// <summary>
        /// Creates a new instance of <see cref="FramePointLoad"/> class.
        /// </summary>
        public FramePointLoad()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FramePointLoad"/> class.
        /// </summary>
        /// <param name="fx">force parallel to X direction.</param>
        /// <param name="fy">force parallel to Y direction.</param>
        /// <param name="mz">Moment parallel to Z direction.</param>
        /// <param name="l1">distance from <see cref="FEALiTE2D.Elements. FrameElement2D.StartNode"/>.</param>
        /// <param name="loadCase">load case.</param>
        /// <param name="direction">load direction.</param>
        public FramePointLoad(double fx, double fy, double mz, double l1, LoadCase loadCase, LoadDirection direction)
        {
            this.Fx = fx;
            this.Fy = fy;
            this.Mz = mz;
            this.L1 = l1;
            this.LoadCase = loadCase;
            this.LoadDirection = direction;
        }

        /// <summary>
        /// Force in X-Direction.
        /// </summary>
        public double Fx { get; set; }

        /// <summary>
        /// Force in Y-Direction.
        /// </summary>
        public double Fy { get; set; }

        /// <summary>
        /// Moment in Z-Direction.
        /// </summary>
        public double Mz { get; set; }

        /// <summary>
        /// Distance of the <see cref="FramePointLoad"/> from the <see cref="FrameElement.StartNode"/>.
        /// </summary>
        public double L1 { get; set; }

        /// <inheritdoc/>
        public LoadDirection LoadDirection { get; set; }

        /// <inheritdoc/>
        public LoadType LoadType => LoadType.FramePointLoad;

        /// <inheritdoc/>
        public LoadCase LoadCase { get; set; }

        /// <inheritdoc/>
        public double[] GetGlobalFixedEndForces(FEALiTE2D.Elements.FrameElement2D element)
        {
            double[] fem = new double[6];
            double fx = this.Fx,
                   fy = this.Fy,
                   mz = this.Mz,
                   l = element.Length;

            // transform forces and moments from global to local.
            if (LoadDirection == LoadDirection.Global)
            {
                double[] F = new double[] { this.Fx, this.Fy, this.Mz };

                double[] Q = new double[3];
                element.LocalCoordinateSystemMatrix.Multiply(F, Q);

                // assign the transformed values to the main new forces values.
                fx = Q[0]; fy = Q[1]; mz = Q[2];
            }

            // 0 |Qx start|
            // 1 |Qy start|
            // 2 |Mz start|
            // 3 |Qx end  |
            // 4 |Qy end  |
            // 5 |Mz end  |

            double a = L1 / l, l2 = l - L1, B = (l - L1) / l;


            // Qx start
            fem[0] = fx * B;

            // Qy start 
            fem[1] = fy * B * B * (3 * a + B) // due to forces
                        - 6.0 * mz * a * B / l; // due to moments

            // Mz start
            fem[2] = fy * L1 * B * B // due to forces
                        + mz * B * (B - 2.0 * a); // due to moments

            // Qx end
            fem[3] = fx * a;

            // Qy end   
            fem[4] = fy * a * a * (a + 3 * B) // due to forces
                       + 6.0 * mz * a * B / l; // due to moments

            // Mz end
            fem[5] = -fy * a * a * l2 // due to forces
                        + mz * a * (a - 2.0 * B); // due to moments

            // back to global
            var fansewr = new double[6];
            element.TransformationMatrix.TransposeMultiply(fem, fansewr);
            return fansewr;
        }

        /// <inheritdoc/>
        public ILoad GetLoadValueAt(Elements.IElement element, double x)
        {
            FramePointLoad load = null;

            if (x == this.L1 || (x - this.L1) <= 1e-8)
            {
                load = new FramePointLoad();
                load.LoadCase = this.LoadCase;
                if (this.LoadDirection == LoadDirection.Global)
                {
                    double[] F = new double[] { this.Fx, this.Fy, 0 };

                    double[] Q = new double[3];
                    element.LocalCoordinateSystemMatrix.Multiply(F, Q);

                    // assign the transformed values to the main new forces values.
                    load.Fx = Q[0];
                    load.Fy = Q[1];
                    load.Mz = this.Mz;
                    load.LoadDirection = LoadDirection.Local;
                }
            }

            return load;
        }
    }
}
