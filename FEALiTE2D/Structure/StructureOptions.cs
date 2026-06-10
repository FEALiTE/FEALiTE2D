using CSparse;
using FEALiTE2D.Elements;
using FEALiTE2D.Loads;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FEALiTE2D.Structure
{
    /// <summary>
    /// Represent a structural model that has many elements connected to each other through nodes.
    /// These elements are subjected to external actions.
    /// To solve a structural model, the model must have at least one degree of freedom.
    /// </summary>
    public partial class Structure
    {
        /// <summary>
        /// Structure analysis options.
        /// </summary>
        public class StructureOption
        {
            /// <summary>
            /// Create a new instance of <see cref="StructureOption"/>.
            /// </summary>
            public StructureOption() { }

            /// <summary>
            /// Include shear strain in the element stiffness matrix calculation?
            /// </summary>
            public BeamTheory BeamTheory { get; set; } = BeamTheory.EulerBernoulli;
        }

        /// <summary>
        /// Default structure analysis options.
        /// </summary>
        public static StructureOption DefaultOptions { get; } = new StructureOption();

        /// <summary>
        /// Shear strain options for frame elements.
        /// </summary>
        public enum BeamTheory
        {
            /// <summary>
            /// Euler-Bernoulli beam theory (without shear strain).
            /// </summary>
            EulerBernoulli,

            /// <summary>
            /// Timoshenko beam theory (with shear strain).
            /// </summary>
            Timoshenko,
        }
    }
}
