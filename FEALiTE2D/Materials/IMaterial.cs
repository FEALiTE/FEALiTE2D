namespace FEALiTE2D.Materials;

/// <summary>
/// Represents an interface for <see cref="IMaterial"/> class
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "UnusedMemberInSuper.Global")]
public interface IMaterial
{
    /// <summary>
    /// Modulus of elasticity
    /// </summary>
    double E { get; set; }

    /// <summary>
    /// Shear modulus
    /// </summary>
    double G { get; }

    /// <summary>
    /// Poisson's ratio
    /// </summary>
    double U { get; set; }

    /// <summary>
    /// Thermal expansion coefficient
    /// </summary>
    double Alpha { get; set; }

    /// <summary>
    /// Unit weight
    /// </summary>
    double Gama { get; set; }

    /// <summary>
    /// Material type
    /// </summary>
    MaterialType MaterialType { get; }

    /// <summary>
    /// Name of the <see cref="IMaterial"/>
    /// </summary>
    string Label { get; set; }
}