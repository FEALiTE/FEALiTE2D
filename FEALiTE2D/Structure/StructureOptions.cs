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
            public ShearStrainOption ShearStrainOption { get; set; } = ShearStrainOption.Without;
        }

        /// <summary>
        /// Default structure analysis options.
        /// </summary>
        public static StructureOption DefaultOptions { get; } = new StructureOption();

        /// <summary>
        /// Shear strain options for frame elements.
        /// </summary>
        public enum ShearStrainOption
        {
            /// <summary>
            /// Euler-Bernoulli beam theory (without shear strain).
            /// </summary>
            Without,

            /// <summary>
            /// https://hal.science/hal-01161516/document
            /// https://fr.scribd.com/document/481185791/Timoshenko-Beam-Finite-Element-pdf?v=0.066
            /// </summary>
            TwoNodesTBTheory,

            /// <summary>
            /// https://hrcak.srce.hr/file/171774
            /// </summary>
            ModifiedTBTheory,

            // To be investigated ?
            // https://www.researchgate.net/profile/Yunhua-Luo/publication/259345479_Shear_Locking_in_Finite_Elements/links/5c86db6f299bf1e02e28586f/Shear-Locking-in-Finite-Elements.pdf?__cf_chl_rt_tk=Z.9JqauawZc8k8aYxFSouiePq1c2HWqC9GPon9ooLrY-1765990150-1.0.1.1-dWDKAP2wjbpHzLxtnyRGNB4DR46OZp_vMQ.b39RwWpc

        }
    }
}
