using FEALiTE2D.Loads;
using System;
using System.Collections.Generic;
using static System.Math;

namespace FEALiTE2D.Elements;

/// <summary>
/// This class represents a node element the can be in any arbitrary location in x-y plan.
/// Each node has 3 degrees of freedom (DOF) one displacement in x-direction, one displacement in y-direction and one rotation about z-direction (perpendicular to the plan)
/// </summary>
[System.Diagnostics.DebuggerDisplay("label:{Label}, x:{X}, y:{Y}")]
[Serializable]
public class Node2D
{
    /// <summary>
    /// Creates a new instance of <see cref="Node2D"/> Class.
    /// </summary>
    public Node2D()
    {
        // initialize default values
        NodalLoads = new List<NodalLoad>();
        SupportDisplacementLoad = new List<SupportDisplacementLoad>();
        DegreeOfFreedomIndices = new List<int>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="Node2D"/> Class.
    /// </summary>
    /// <param name="x">X-coordinate of the <see cref="Node2D"/> Element.</param>
    /// <param name="y">Y-coordinate of the <see cref="Node2D"/> Element.</param>
    /// <param name="label">name of the <see cref="Node2D"/> element</param>
    public Node2D(double x, double y, string label) : this()
    {
        // assign variable
        X = x;
        Y = y;
        Label = label;
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
    /// A list of dof numbers that is set by the program.
    /// </summary>
    public List<int> DegreeOfFreedomIndices { get; internal set; }

    /// <summary>
    /// An angle of rotation of the of local axes of the node around Z-axis.
    /// </summary>
    public double RotationAngle { get; set; }

    /// <summary>
    /// Gets number of degrees of freedom of the node.
    /// </summary>
    public int Dof =>
        Support switch
        {
            null => 3,
            NodalSpringSupport _ => 3,
            _ => 3 - Support.RestraintCount
        };

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
    /// Transformation matrix of the Node due to a <see cref="RotationAngle"/>.
    /// </summary>
    public CSparse.Double.DenseMatrix TransformationMatrix
    {
        get
        {
            var c = Cos(RotationAngle);
            var s = Sin(RotationAngle);
            var t = new[,]
            {
                { c, s, 0 },
                {-s, c, 0 },
                { 0, 0, 1 }
            };
            return CSparse.Double.DenseMatrix.OfArray(t) as CSparse.Double.DenseMatrix;
        }
    }

    /// <summary>
    /// Gets whether the node is free or restrained.
    /// </summary>
    public bool IsFree => Support == null;

    /// <summary>
    /// Gets or Sets the rigid support of the node.
    /// </summary>
    public NodalSupport Support { get; set; }

    /// <summary>
    /// Determines whether the specified dof is a restraint.
    /// </summary>
    /// <param name="dof">The dof.</param>
    /// <returns><c>true</c> if the specified dof is a restraint; otherwise, <c>false</c>.</returns>
    public bool IsRestrained(NodalDegreeOfFreedom dof)
    {
        if (Support == null) return false;
        return dof switch
        {
            NodalDegreeOfFreedom.Ux => Support.Ux,
            NodalDegreeOfFreedom.Uy => Support.Uy,
            NodalDegreeOfFreedom.Rz => Support.Rz,
            _ => false
        };
    }

}