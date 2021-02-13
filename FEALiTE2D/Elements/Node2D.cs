using FEALiTE2D.Loads;
using System;
using System.Collections.Generic;
using static System.Math;

namespace FEALiTE2D.Elements
{
    /// <summary>
    /// This class represents a node element the can be in any arbitrary location in x-y plan.
    /// Each node has 3 degrees of freedom (DOF) one displacement in x-direction, one displacement in y-direction and one rotation about z-direction (perpendicular to the plan)
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("label:{Label}, x:{X}, y:{Y}")]
    public class Node2D
    {
        /// <summary>
        /// Creates a new instance of <see cref="Node2D"/> Class.
        /// </summary>
        public Node2D()
        {
            // initialize default values
            this.Restrains = new List<NodalDegreeOfFreedom>();
            this.NodalLoads = new List<NodalLoad>();
            this.SupportDisplacementLoad = new List<SupportDisplacementLoad>();
            this.CoordNumbers = new List<int>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="Node2D"/> Class.
        /// </summary>
        /// <param name="x">X-coordinate of the <see cref="Node2D"/> Element.</param>
        /// <param name="y">Y-coordinate of the <see cref="Node2D"/> Element.</param>
        /// <param name="label">name of the <see cref="Node2D"/> element</param>
        public Node2D(double x, double y, string label) :this()
        {
            // assign variable
            this.X = x;
            this.Y = y;
            this.Label = label;
        }

        /// <summary> 
        /// X-Coordinate of the <see cref="Node2D"/> element.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y-Coordinate of the <see cref="Node2D"/> element.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// A display name for the node that is set by the user.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// A list of ndof numbers that is set by the program.
        /// </summary>
        public List<int> CoordNumbers { get; internal set; }

        /// <summary>
        /// An angle of rotation of the of local axes of the node around Z-axis.
        /// </summary>
        public double RotaionAngle { get; set; }

        /// <summary>
        /// Gets number of degrees of freedom of the node.
        /// </summary>
        public int DOF { get { return 3 - Restrains.Count; } }

        /// <summary>
        /// Gets or sets the Degrees of freedom of the <see cref="Node2D"/>.
        /// </summary>
        public IList<NodalDegreeOfFreedom> Restrains { get; set; }

        /// <summary>
        /// Gets or sets the nodal displacement.
        /// </summary>
        public List<SupportDisplacementLoad> SupportDisplacementLoad { get; set; }

        /// <summary>
        /// Loads on the <see cref="Node2D"/>.
        /// </summary>
        public List<NodalLoad> NodalLoads { get; set; }

        /// <summary>
        /// Distances between 2 nodes.
        /// </summary>
        /// <param name="other">The other node.</param>
        /// <returns>Distances between 2 nodes</returns>
        public double DistanceBetween(Node2D other) => Sqrt(Pow(X - other.X, 2) + Pow(Y - other.Y, 2));

        /// <summary>
        /// Gets or sets the parent structure that this element is part of it.
        /// </summary>
        public Structure.Structure ParentStructure { get; set; }

        /// <summary>
        /// Transformation matrix of the Node due to a <see cref="RotaionAngle"/>.
        /// </summary>
        public CSparse.Double.DenseMatrix TransformationMatrix
        {
            get
            {
                double c = Cos(RotaionAngle);
                double s = Sin(RotaionAngle);
                double[,] t = new double[,]
                {
                    { c, s, 0 },
                    {-s, c, 0 },
                    { 0, 0, 1 }
                };
                return CSparse.Double.DenseMatrix.OfArray(t) as CSparse.Double.DenseMatrix;
            }
        }

        /// <summary>
        /// Restrains the specified dof.
        /// </summary>
        /// <param name="dof">The <see cref="NodalDegreeOfFreedom"/>.</param>
        public void Restrain(NodalDegreeOfFreedom dof)
        {
            if (!this.Restrains.Contains(dof))
                this.Restrains.Add(dof);
        }

        /// <summary>
        /// Restrains the specified dofs.
        /// </summary>
        /// <param name="dofs">The <see cref="NodalDegreeOfFreedom"/>.</param>
        public void Restrain(params NodalDegreeOfFreedom[] dofs)
        {
            foreach (NodalDegreeOfFreedom dof in dofs)
                if (!this.Restrains.Contains(dof))
                    this.Restrains.Add(dof);
        }

        /// <summary>
        /// Unrestrains the specified dof.
        /// </summary>
        /// <param name="dof">The <see cref="NodalDegreeOfFreedom"/>.</param>
        public void Unrestrain(NodalDegreeOfFreedom dof)
        {
            if (this.Restrains.Contains(dof))
                this.Restrains.Remove(dof);
        }

        /// <summary>
        /// Unrestrains the specified dofs.
        /// </summary>
        /// <param name="dofs">The <see cref="NodalDegreeOfFreedom"/>.</param>
        public void Unrestrain(params NodalDegreeOfFreedom[] dofs)
        {
            foreach (NodalDegreeOfFreedom dof in dofs)
                if (this.Restrains.Contains(dof))
                    this.Restrains.Remove(dof);
        }

        /// <summary>
        /// Determines whether the specified dof is restrains.
        /// </summary>
        /// <param name="dof">The dof.</param>
        /// <returns><c>true</c> if the specified dof is restrains; otherwise, <c>false</c>.</returns>
        public bool IsRestrained(NodalDegreeOfFreedom dof)
        {
            if (this.Restrains.Contains(dof))
                return true;
            return false;
        }

     
    }
}
