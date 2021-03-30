using FEALiTE2D.Structure;

namespace FEALiTE2D.Meshing
{
    /// <summary>
    /// This class represents a small linear segment of an element that is bounded by two distances from start node of the element.
    /// </summary>
    [System.Serializable]
    public class LinearMeshSegment
    {
        public double
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
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
        public Displacement Displacement1, // displacement at start of the segment.
                            Displacement2;  // displacement at end of  the segment.
        public double E, // modulus of elasticity of material of the cross-section at this segment.
                      A, // area of the cross-section at the segment.
                      Ix; // second moment of inertial of the cross-section at the segment.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member


        /// <summary>
        /// Creates a new instance of <see cref="LinearMeshSegment"/> class.
        /// </summary>
        public LinearMeshSegment()
        {
            this.Internalforces1 = new Force();
            this.Internalforces2 = new Force();
            this.Displacement1 = new Displacement();
            this.Displacement2 = new Displacement();
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

        /// <summary>
        /// Get slope angle of a point on a segment due to loads
        /// </summary>
        /// <param name="x">a distance</param>
        public double SlopeAngleAt(double x)
        {
            return Displacement1.Rz -
                (
                     this.Internalforces1.Mz * x
                     - Internalforces1.Fy * x * x / 2.0
                     - wy1 * x * x * x / 6.0
                     - x * x * x * x * ((wy2 - wy1) / (x2 - x1)) / 24 // uniform and trap load.
                ) / (E * Ix);
        }

        /// <summary>
        /// Get Deflection of a point on a segment due to vertical loads 
        /// </summary>
        /// <param name="x">a distance</param>
        public double VerticalDisplacementAt(double x)
        {
            return Displacement1.Uy +
                (
                    Displacement1.Rz * x -
                    (
                         this.Internalforces1.Mz * x * x / 2.0
                         - Internalforces1.Fy * x * x * x / 6.0
                         - wy1 * x * x * x * x / 24.0
                         - x * x * x * x * x * ((wy2 - wy1) / (x2 - x1)) / 120.0 // uniform and trap load.
                    ) / (E * Ix)
                );
        }

        /// <summary>
        /// Get Axial displacement of a point on a segment due to vertical loads 
        /// </summary>
        /// <param name="x">a distance</param>
        public double AxialDisplacementAt(double x)
        {
            double EA = E * A;
            return Displacement1.Ux -
                Internalforces1.Fx * x / EA + wx1 * x * x / (2.0 * EA) + (wx2 - wx1) * x * x * x / (6.0 * EA * (x2 - x1));
        }

        /// <summary>
        /// Get 3 components of displacement at a distance of the segment.
        /// </summary>
        /// <param name="x">a distance</param>
        public Displacement GetDisplacementAt(double x)
        {
            return new Displacement()
            {
                Rz = SlopeAngleAt(x),
                Ux = AxialDisplacementAt(x),
                Uy = VerticalDisplacementAt(x)
            };
        }
    }
}
