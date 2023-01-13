namespace FEALiTE2D.Loads;

/// <summary>
/// Represents the direction of the load whether it's in Global or Local X, Y, or Z.
/// </summary>
public enum LoadDirection
{
    /// <summary>
    /// Represent the coordinates in global axes (X and Y)
    /// </summary>
    Global = 0,

    /// <summary>
    /// Represent the coordinates in local axes of the element (x and y)
    /// </summary>
    Local
}