using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Elements
{
    /// <summary>
    /// Frame end releases and partial releases.
    /// </summary>
    public enum Frame2DEndRelease
    {
        /// <summary>
        /// No release at start of end of the element.
        /// </summary>
        NoRelease,

        /// <summary>
        /// there is at start of the element.
        /// </summary>
        StartRelease,

        /// <summary>
        /// there is at end of the element.
        /// </summary>
        EndRelease,

        /// <summary>
        /// there is at both start and end of the element.
        /// </summary>
        FullRlease
    }
}
