namespace FEALiTE2D.Elements
{
    /// <summary>
    /// A Class that Represents a nodal rigid support that prevents motion or rotation in XY plan. 
    /// </summary>
    public class NodalSupport
    {

        /// <summary>
        /// Create a new instance of <see cref="NodalSupport"/> class.
        /// </summary>
        public NodalSupport()
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="NodalSupport"/> class.
        /// </summary>
        /// <param name="ux">translation in X-Direction</param>
        /// <param name="uy">translation in Y-Direction</param>
        /// <param name="rz">rotation about Z-Direction</param>
        public NodalSupport(bool ux, bool uy, bool rz)
        {
            this.Ux = ux;
            this.Uy = uy;
            this.Rz = rz;
        }

        /// <summary>
        /// Get or set whether node can have a displacement in X-Direction
        /// </summary>
        public bool Ux { get; set; }

        /// <summary>
        /// Get or set whether node can have a displacement in Y-Direction
        /// </summary>
        public bool Uy { get; set; }

        /// <summary>
        /// Get or set whether node can have a rotation about Z-Direction
        /// </summary>
        public bool Rz { get; set; }

        /// <summary>
        /// Get number of restrained degrees of freedom
        /// </summary>
        public int RestraintCount
        {
            get
            {
                int i = 0;
                if (this.Ux == true) i++;
                if (this.Uy == true) i++;
                if (this.Rz == true) i++;
                return i;
            }
        }
    }
}