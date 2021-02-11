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
        /// Load type.
        /// </summary>
        LoadType LoadType { get; }

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

    }
}
