using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Elements
{
    /// <summary>
    /// This class represents a container for nodal degree of freedom at any node.
    /// </summary>
    public class DegreeOfFreedom
    {
        /// <summary>
        /// Creates a new instance of <see cref="DegreeOfFreedom"/>
        /// </summary>
        public DegreeOfFreedom(NodalDegreeOfFreedom dof)
        {
            this.NodalDegreeOfFreedom = dof;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DegreeOfFreedom"/>
        /// </summary>
        public DegreeOfFreedom(NodalDegreeOfFreedom dof, int number = 0) : this(dof)
        {
            this.Coordinate = number;
        }

        /// <summary>
        /// The coordinate number of the <see cref="FEALiTE2D.Elements.NodalDegreeOfFreedom"/>
        /// </summary>
        public int Coordinate { get; set; }

        /// <summary>
        /// Represents an enum for possible degrees of freedom of a single <see cref="FEALiTE2D.Elements.Node2D"/>
        /// </summary>
        public NodalDegreeOfFreedom NodalDegreeOfFreedom { get; set; }

    }
}
