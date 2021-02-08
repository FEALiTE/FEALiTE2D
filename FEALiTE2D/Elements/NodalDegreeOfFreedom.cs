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
        UX = 0,

        /// <summary>
        /// Linear translation along the Y axis
        /// </summary>
        UY,

        /// <summary>
        /// Rotation around the Z axis
        /// </summary>
        RZ,
    }
}
