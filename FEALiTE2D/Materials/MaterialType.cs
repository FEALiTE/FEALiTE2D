namespace FEALiTE2D.Materials;

/// <summary>
/// Material type
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum MaterialType
{
    /// <summary>
    /// Concrete Material
    /// </summary>
    Concrete = 0,

    /// <summary>
    /// Steel Material
    /// </summary>
    Steel,

    /// <summary>
    /// Timber Material
    /// </summary>
    Timber,

    /// <summary>
    /// Aluminum Material
    /// </summary>
    Aluminum,

    /// <summary>
    /// Other Material
    /// </summary>
    UserDefined
}