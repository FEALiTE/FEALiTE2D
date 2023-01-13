using FEALiTE2D.Materials;

namespace FEALiTE2D.CrossSections;

/// <summary>
/// Represents a class for <see cref="IFrame2DSection"/>.
/// </summary>
[System.Serializable]
public abstract class IFrame2DSection
{
    /// <summary>
    /// Material of the <see cref="IFrame2DSection"/>.
    /// </summary>
    public IMaterial Material { get; set; }

    /// <summary>
    /// Label of <see cref="IFrame2DSection"/>.
    /// </summary>
    public string Label { get; set; }
    /// <summary>
    /// Cross-sectional area.
    /// </summary>
    public double A { get; set; }

    /// <summary>
    /// Shear area of the cross section in x-x direction.
    /// </summary>
    public virtual double Ax { get; set; }

    /// <summary>
    /// Shear area of the cross section in y-y direction.
    /// </summary>
    public virtual double Ay { get; set; }

    /// <summary>
    /// Area moment of inertia about x axis.
    /// </summary>
    public virtual double Ix { get; set; }

    /// <summary>
    /// Area moment of inertia about y axis.
    /// </summary>
    public virtual double Iy { get; set; }

    /// <summary>
    /// Torsional constant.
    /// </summary>
    public virtual double J { get; set; }

    /// <summary>
    /// Gets or sets the maximum height.
    /// </summary>
    public virtual double MaxHeight { get; set; }

    /// <summary>
    /// Gets or sets the maximum width.
    /// </summary>
    public virtual double MaxWidth { get; set; }        
}