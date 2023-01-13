using FEALiTE2D.Materials;

namespace FEALiTE2D.CrossSections;

/// <summary>
/// Creates a generic section (user defined section), the user will have to give all required section properties for the section.
/// </summary>
[System.Serializable]
public class Generic2DSection : IFrame2DSection
{
    /// <summary>
    /// Creates a new instance of the <see cref="Generic2DSection"/>
    /// </summary>
    /// <param name="a">Cross section area.</param>
    /// <param name="ax">Shear area in x-x direction.</param>
    /// <param name="ay">Shear area in y-y direction.</param>
    /// <param name="ix">Area moment of inertia about x axis.</param>
    /// <param name="iy">Area moment of inertia about y axis.</param>
    /// <param name="j">Torsional constant.</param>
    /// <param name="maxHeight">max. height.</param>
    /// <param name="maxWidth">max. width.</param>
    /// <param name="material">material.</param>
    public Generic2DSection(double a, double ax, double ay, double ix, double iy, double j, double maxHeight, double maxWidth, IMaterial material)
    {
        A = a;
        base.Ax = ax;
        base.Ay = ay;
        base.Ix = ix;
        base.Iy = iy;
        base.J = j;
        base.MaxHeight = maxHeight;
        base.MaxWidth = maxWidth;
        Material = material;
    }

}