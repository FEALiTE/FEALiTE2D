using FEALiTE2D.Elements;
using FEALiTE2D.Materials;

namespace FEALiTE2D.CrossSections
{
    /// <summary>
    /// Creates a generic section (user defined section), the user will have to give all required section properties for the section.
    /// </summary>
    public class Generic2DSection : IFrame2DSection
    {
        /// <summary>
        /// Creates a new instance of the <see cref="GenericSection"/>
        /// </summary>
        /// <param name="A">Cross section area.</param>
        /// <param name="Ax">Shear area in x-x direction.</param>
        /// <param name="Ay">Shear area in y-y direction.</param>
        /// <param name="Ix">Area moment of inertia about x axis.</param>
        /// <param name="Iy">Area moment of inertia about y axis.</param>
        /// <param name="J">Torsional constant.</param>
        /// <param name="hmax">max. height.</param>
        /// <param name="wmax">max. width.</param>
        /// <param name="material">material.</param>
        public Generic2DSection(double A, double Ax, double Ay, double Ix, double Iy, double J, double hmax, double wmax, IMaterial material) : base()
        {
            base.A = A;
            base.Ax = Ax;
            base.Ay = Ay;
            base.Ix = Ix;
            base.Iy = Iy;
            base.J = J;
            base.MaxHeight = hmax;
            base.MaxWidth = wmax;
            base.Material = material;
        }

    }
}
