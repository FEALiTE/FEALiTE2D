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
        /// Gets the global fixed end forces.
        /// </summary>
        /// <returns>Vector.</returns>
        double[] GetGlobalFixedEndForces();

    }
}
