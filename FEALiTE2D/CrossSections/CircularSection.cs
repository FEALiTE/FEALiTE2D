using FEALiTE2D.Materials;

namespace FEALiTE2D.CrossSections;

/// <summary>
/// Represents a Solid Circular Cross-Section.
/// </summary>
/// <seealso cref="Frame2DSection" />
[System.Serializable]
public class CircularSection : Frame2DSection
{
    /// <summary>
    /// Creates new instance of a <see cref="CircularSection"/>.
    /// </summary>
    /// <param name="d">diameter of the <see cref="CircularSection"/>.</param>
    /// <param name="material">Material of the cross-section.</param>
    public CircularSection(double d, IMaterial material)
    {
        D = d;
        Material = material;

        // calculate section properties and set them here 
        // to avoid recalculation in each time they are called to save time.
        SetSectionProperties(d);
    }

    /// <summary>
    /// Diameter of the circular section.
    /// </summary>
    public double D { get; set; }

    /// <summary>
    /// Sets the section properties.
    /// </summary>
    /// <param name="d">diameter of the <see cref="CircularSection"/>.</param>
    private void SetSectionProperties(double d)
    {
        A = System.Math.PI * d * d / 4.0;
        Ay = Ax = A * 0.9;
        Iy = Ix = System.Math.PI * d * d * d * d / 64.0;
        J = 0.5 * Iy;
        base.MaxWidth = base.MaxHeight = d;
    }
}