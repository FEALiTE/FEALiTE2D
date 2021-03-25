using CSparse.Double;

namespace FEALiTE2D.Loads
{
    /// <summary>
    /// Represents an interface for all loads.
    /// </summary>
    public interface ILoad
    {
        /// <summary>
        /// The direction of the load.
        /// </summary>
        LoadDirection LoadDirection { get; set; }

        /// <summary>
        /// the load case.
        /// </summary>
        LoadCase LoadCase { get; set; }

        /// <summary>
        /// Gets the global fixed end forces in global coordinates system.
        /// </summary>
        /// <param name="element">frame element</param>
        /// <returns>global fixed end forces of this load</returns>
        double[] GetGlobalFixedEndForces(FEALiTE2D.Elements.FrameElement2D element);

        /// <summary>
        /// Get the magnitude of a load at given distance in local coordinates system.
        /// </summary>
        /// <param name="element">the element which this load is applied into to process></param>
        /// <param name="x">distance measured from start node of an element.</param>
        /// <returns>load value</returns>
        ILoad GetLoadValueAt(FEALiTE2D.Elements.IElement element, double x);
    }
}
