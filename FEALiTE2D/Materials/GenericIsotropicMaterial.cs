namespace FEALiTE2D.Materials
{
    /// <summary>
    /// Represents an interface for <see cref="GenericIsotropicMaterial"/> class
    /// </summary>
    [System.Serializable]
    public class GenericIsotropicMaterial : IMaterial
    {
        /// <summary>
        /// Name of the <see cref="GenericIsotropicMaterial"/>
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Modulus of elasticity
        /// </summary>
        public double E { get; set; }

        /// <summary>
        /// Shear modulus
        /// </summary>
        public double G => 0.5 * E / (1.0 + U);

        /// <summary>
        /// Poisson's ratio
        /// </summary>
        public double U { get; set; }

        /// <summary>
        /// Thermal expansion coefficient
        /// </summary>
        public double Alpha { get; set; }

        /// <summary>
        /// Unit weight
        /// </summary>
        public double Gama { get; set; }

        /// <summary>
        /// Material type
        /// </summary>
        public MaterialType MaterialType { get; set; }
    }
}
