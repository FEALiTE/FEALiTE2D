using FEALiTE2D.Loads;
using System.Collections.Generic;

namespace FEALiTE2D.Elements
{
    /// <summary>
    /// A Class that Represents a nodal spring support that have three values of spring constants. 
    /// </summary>
    public class NodalSpringSupport
    {

        /// <summary>
        /// Create a new instance of <see cref="NodalSpringSupport"/> class.
        /// </summary>
        public NodalSpringSupport()
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="NodalSpringSupport"/> class.
        /// </summary>
        /// <param name="kx">Spring constant for translation in X-Direction</param>
        /// <param name="ky">Spring constant for translation in Y-Direction</param>
        /// <param name="cz">Spring constant for rotation about Z-Direction</param>
        public NodalSpringSupport(double kx, double ky, double cz)
        {
            this.Kx = kx;
            this.Ky = ky;
            this.Cz = cz;
        }

        /// <summary>
        /// Spring constant for translation in X-Direction
        /// </summary>
        public double Kx { get; set; }

        /// <summary>
        /// Spring constant for translation in Y-Direction
        /// </summary>
        public double Ky { get; set; }

        /// <summary>
        /// Spring constant for rotation about Z-Direction
        /// </summary>
        public double Cz { get; set; }

        /// Indicates that the <see cref="FrameElement2D"/> is active in the current load case or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// /Indicates that this element can only subject to Tension Loads like cables.
        /// </summary>
        public bool TensionOnly { get; set; }

        /// <summary>
        /// A list of <see cref="LoadCase"/> in which.. this frame element is not active.
        /// </summary>
        public List<LoadCase> LoadCasesToIgnore { get; set; }

        /// <summary>
        /// Gets the global stiffness matrix, This should be called after <see cref="Initialize"/>
        /// </summary>
        public CSparse.Double.DenseMatrix GlobalStiffnessMatrix
        {
            get
            {
                double[,] k = new double[,]
                {
                    {Kx, 0 , 0 },
                    { 0, Ky, 0 },
                    { 0, 0 , Cz},
                };
                return CSparse.Double.DenseMatrix.OfArray(k) as CSparse.Double.DenseMatrix;
            }
        }
    }
}
