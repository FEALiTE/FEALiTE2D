namespace FEALiTE2D.Elements
{
    /// <summary>
    /// Represents an enum for possible degrees of freedom of a single <see cref="FEALiTE2D.Elements.Node2D"/>
    /// </summary>
    public enum NodalDegreeOfFreedom
    {
        /// <summary>
        /// Linear translation along the X axis
        /// </summary>
        Ux = 0,

        /// <summary>
        /// Linear translation along the Y axis
        /// </summary>
        Uy = 1,

        /// <summary>
        /// Rotation around the Z axis
        /// </summary>
        Rz = 2,
    }
}
