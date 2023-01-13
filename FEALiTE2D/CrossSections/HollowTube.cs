using FEALiTE2D.Materials;
using static System.Math;

namespace FEALiTE2D.CrossSections;

/// <summary>
/// Represent a solid tube with hole.
/// </summary>
/// <seealso cref="FEALiTE2D.CrossSections.IFrame2DSection" />
[System.Serializable]
public class HollowTube : IFrame2DSection
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HollowTube"/> class.
    /// </summary>
    /// <param name="d">The diameter.</param>
    /// <param name="thickness">The thickness.</param>
    /// <param name="material">The material.</param>
    public HollowTube(double d, double thickness, IMaterial material)
    {
        D = d;
        Thickness = thickness;
        Material = material;

        // calculate section properties and set them here 
        // to avoid recalculation in each time they are called to save time.
        SetSectionProperties(d, thickness);
    }

    /// <summary>
    /// Gets or sets the diameter.
    /// </summary>
    /// <value>The d.</value>
    public double D { get; set; }

    /// <summary>
    /// Gets or sets the thickness.
    /// </summary>
    public double Thickness { get; set; }

    /// <summary>
    /// Sets the section properties.
    /// </summary>
    /// <param name="d">diameter</param>
    /// <param name="t">thickness</param>
    private void SetSectionProperties(double d, double t)
    {
        var di = d - 2 * t;
        A = 0.25 * PI * (d * d - di * di);
        Ay = Ax = A * 0.5;
        Iy = Iy = PI * Pow(d / 2, 3) * t;
        J = Iy + Ix;
        base.MaxWidth = base.MaxHeight = d;
    }       
}