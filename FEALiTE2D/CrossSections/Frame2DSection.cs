using FEALiTE2D.Materials;

namespace FEALiTE2D.CrossSections
{
    /// <summary>
    /// Represents a class for <see cref="Frame2DSection"/>.
    /// </summary>
    [System.Serializable]
    public abstract class Frame2DSection
    {
        /// <summary>
        /// Material of the <see cref="Frame2DSection"/>.
        /// </summary>
        public IMaterial Material { get; set; }

        /// <summary>
        /// Label of <see cref="Frame2DSection"/>.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Cross-sectional area.
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Shear area of the cross section in z-z direction.
        /// </summary>
        public virtual double Az { get; set; }

        /// <summary>
        /// Shear area of the cross section in y-y direction.
        /// </summary>
        public virtual double Ay { get; set; }

        /// <summary>
        /// Area moment of inertia about z axis.
        /// </summary>
        public virtual double Iz { get; set; }

        /// <summary>
        /// Area moment of inertia about y axis.
        /// </summary>
        public virtual double Iy { get; set; }

        /// <summary>
        /// Torsional constant.
        /// </summary>
        public virtual double J { get; set; }

        /// <summary>
        /// Gets or sets the maximum height.
        /// </summary>
        public virtual double MaxHeight { get; set; }

        /// <summary>
        /// Gets or sets the maximum width.
        /// </summary>
        public virtual double MaxWidth { get; set; }

        private double? _EIz;
        internal double EIz
        {
            get
            {
                if (_EIz == null)
                {
                    _EIz = Material.E * Iz;
                }
                return _EIz.Value;
            }
        }
        private double? _EIy;
        internal double EIy
        {
            get
            {
                if (_EIy == null)
                {
                    _EIy = Material.E * Iy;
                }
                return _EIy.Value;
            }
        }
        private double? _GAz;
        internal double GAz
        {
            get
            {
                if (_GAz == null)
                {
                    _GAz = Material.G * Az;
                }
                return _GAz.Value;
            }
        }
        private double? _EA;
        internal double EA
        {
            get
            {
                if (_EA == null)
                {
                    _EA = Material.E * A;
                }
                return _EA.Value;
            }
        }    
    }
}
