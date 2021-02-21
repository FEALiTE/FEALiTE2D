namespace FEALiTE2D.Meshing
{
    /// <summary>
    /// This interface represents a mesher interface for 1D elements.
    /// </summary>
    public interface ILinearMesher
    {
        /// <summary>
        /// Gets or sets the minimum number of segments that the <see cref="Elements.IElement"/> be discretized into.
        /// </summary>
        int NumberSegements { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of segments that the <see cref="Elements.IElement"/> be discretized into.
        /// </summary>
        double MinDistance { get; set; }

        /// <summary>
        /// This method should set locations of change in external forces and moments to properly calculate internal forces.
        /// </summary>
        /// <param name="element">an element to set up points on.</param>
        void SetupDiscreteLocations(FEALiTE2D.Elements.IElement element);

    }
}
