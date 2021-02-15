using FEALiTE2D.Elements;
using System;

namespace FEALiTE2D.Loads
{
    /// <summary>
    /// Represent a class for support displacement displacements in Global Coordinates system.
    /// </summary>
    public class SupportDisplacementLoad
    {
        /// <summary>
        /// Creates a new class of <see cref="NodalDisplacement"/>.
        /// </summary>
        public SupportDisplacementLoad()
        {
        }

        /// <summary>
        /// Creates a new class of <see cref="NodalDisplacement"/>
        /// </summary>
        /// <param name="ux">global displacement in X-Direction.</param>
        /// <param name="uy">global displacement in Y-Direction.</param>
        /// <param name="rz">global rotation about Z-Direction.</param>
        /// <param name="loadCase">load case.</param>
        public SupportDisplacementLoad(double ux, double uy, double rz, LoadCase loadCase)
            : this()
        {
            this.Ux = ux;
            this.Uy = uy;
            this.Rz = rz;
            this.LoadCase = loadCase;
        }

        /// <summary>
        /// Displacement in X-Direction.
        /// </summary>
        public double Ux { get; set; }

        /// <summary>
        /// Displacement in Y-Direction.
        /// </summary>
        public double Uy { get; set; }

        /// <summary>
        /// rotation about Z-Direction.
        /// </summary>
        public double Rz { get; set; }

        /// <inheritdoc/>
        public LoadDirection LoadDirection { get; set; }

        /// <inheritdoc/>
        public LoadType LoadType => LoadType.SupportSettelement;

        /// <inheritdoc/>
        public LoadCase LoadCase { get; set; }

        /// <inheritdoc/>
        public double[] GetGlobalFixedEndDisplacement(Node2D node)
        {
            // create force vector
            double[] Q = new double[3] { Ux, Uy, Rz };

            // if the forces is in global coordinate system of the node then return it.
            if (this.LoadDirection == LoadDirection.Global)
            {
                return Q;
            }
            // transform the load vector to the local coordinate of the node.
            double[] F = new double[3];
            node.TransformationMatrix.TransposeMultiply(Q, F);
            return F;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            if (obj.GetType() != typeof(SupportDisplacementLoad))
            {
                return false;
            }
            SupportDisplacementLoad nd = obj as SupportDisplacementLoad;
            if (Ux != nd.Ux || Uy != nd.Uy || Rz != nd.Rz || LoadCase != nd.LoadCase)
            {
                return false;
            }
            return true;
        }

        public static bool operator ==(SupportDisplacementLoad nl1, SupportDisplacementLoad nl2)
        {
            if (ReferenceEquals(nl1, null))
            {
                return false;
            }
            return nl1.Equals(nl2);
        }

        public static bool operator !=(SupportDisplacementLoad nl1, SupportDisplacementLoad nl2)
        {
            if (ReferenceEquals(nl1, null))
            {
                return false;
            }
            return !nl1.Equals(nl2);
        }

        public override int GetHashCode()
        {
            int result = 0;
            result += (Ux + 1e-10).GetHashCode();
            result += (Uy + 2e-10).GetHashCode();
            result += (Rz + 6e-10).GetHashCode();
            result += LoadCase.GetHashCode();
            return result;
        }
    }
}
