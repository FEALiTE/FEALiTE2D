using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Loads
{
    /// <summary>
    /// Represents the load type.
    /// </summary>
    public enum LoadType
    {
        /// <summary>
        /// Loads that can only be applied to <see cref="FEALiTE2D.Elements.Node2D"/> elements.
        /// </summary>
        NodalForces = 0,

        /// <summary>
        /// Loads that can only be applied to <see cref="FEALiTE2D.Elements.Node2D"/> elements when restrained.
        /// </summary>
        SupportSettelement= 0,

        /// <summary>
        /// Loads that can only be applied to <see cref="FEALiTE2D.Elements.FrameElement2D"/> elements.
        /// </summary>
        FramePointLoad,

        /// <summary>
        /// Loads that can only be applied to <see cref="FEALiTE2D.Elements.FrameElement2D"/> elements.
        /// </summary>
        FrameLinerLoad,

        /// <summary>
        /// Other load types that are not supported by the library or created by the developer.
        /// </summary>
        Other
    }
}
