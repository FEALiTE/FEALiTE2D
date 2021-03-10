using FEALiTE2D.Structure;

namespace FEALiTE2D.Meshing
{
    /// <summary>
    /// This class represents a small linear segment of an element that is bounded by two distances from start node of the element.
    /// </summary>
    public class LinearMeshSegment
    {
        public double
               x1, // start distance of the segment measured from start node.
               x2, // end distance of the segment measured from start node.
               wx1, // uniform and trapezoidal load value in local x direction at x1
               wx2, // uniform and trapezoidal load value in local x direction at x2
               wy1, // uniform and trapezoidal load value in local y direction at x1
               wy2, // uniform and trapezoidal load value in local y direction at x2
               fx, // point force value in local x direction at x1
               fy, // point force value in local y direction at x1
               mz; // point moment value in local z direction at x1
        public Force Internalforces1; // internal forces at start of the segment.
        public Force Internalforces2; // internal forces at end of the segment.

        /// <summary>
        /// Creates a new instance of <see cref="LinearMeshSegment"/> class.
        /// </summary>
        public LinearMeshSegment()
        {
            this.Internalforces1 = new Force();
            this.Internalforces2 = new Force();
        }

        /// <summary>
        /// Get shear value at a distance of the segment.
        /// </summary>
        /// <param name="x">a distance</param>
        public double ShearAt(double x)
        {
            return this.Internalforces1.Fy
               + wy1 * x + 0.5 * x * x * (wy2 - wy1) / (x2 - x1); // uniform and trap load.
        }

        /// <summary>
        /// Get bending moment value at a distance of the segment.
        /// </summary>
        /// <param name="x">a distance</param>
        public double MomentAt(double x)
        {
            return this.Internalforces1.Mz
               - Internalforces1.Fy * x
               - 0.5 * wy1 * x * x - x * x * x * ((wy2 - wy1) / (x2 - x1)) / 6; // uniform and trap load.
        }

        /// <summary>
        /// Get axial force value at a distance of the segment.
        /// </summary>
        /// <param name="x">a distance</param>
        public double AxialAt(double x)
        {
            return this.Internalforces1.Fx +
                wx1 * x + x * x * (wx2 - wx1) / (x2 - x1) / 2; // uniform and trap load.
        }

        /// <summary>
        /// Get internal forces at a distance of the segment.
        /// </summary>
        /// <param name="x">a distance</param>
        public Force GetInternalForceAt(double x)
        {
            return new Force()
            {
                Fx = AxialAt(x),
                Fy = ShearAt(x),
                Mz = MomentAt(x)
            };
        }
    }
}
