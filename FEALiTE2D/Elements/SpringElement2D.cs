using CSparse.Double;
using FEALiTE2D.CrossSections;
using FEALiTE2D.Loads;
using FEALiTE2D.Meshing;
using System;
using System.Collections.Generic;
using static System.Math;

namespace FEALiTE2D.Elements;

/// <summary>
/// Represents a spring element/Fictitious bar in 2d space that has a spring stiffness and has 2 dof at each node. Spring may be longitudinal or rotational.
/// </summary>
[Serializable]
public class SpringElement2D : IElement
{
    /// <summary>
    /// Creates a new instance of <see cref="SpringElement2D"/> class.
    /// </summary>
    public SpringElement2D()
    {
        Loads = new List<ILoad>();
        GlobalEndForcesForLoadCase = new Dictionary<LoadCase, double[]>();
        MeshSegments = new List<LinearMeshSegment>();
        AdditionalMeshPoints = new SortedSet<double>();
    }

    /// <summary>
    /// Creates a new instance of <see cref="SpringElement2D"/> class.
    /// </summary>
    /// <param name="startNode">Start node</param>
    /// <param name="endNode">End node</param>
    /// <param name="label">name of the spring</param>
    public SpringElement2D(Node2D startNode, Node2D endNode, string label) : this()
    {
        StartNode = startNode;
        EndNode = endNode;
        Label = label;
        LocalCoordinateSystemMatrix = GetLocalCoordinateSystemMatrix();
        TransformationMatrix = GetTransformationMatrix();
    }

    /// <summary>
    /// Longitudinal spring stiffness, units are [force/length]
    /// </summary>
    public double K { get; set; }

    /// <summary>
    /// Rotational spring stiffness about z-direction, units are [force.length/radians]
    /// </summary>
    public double R { get; set; }

    /// <summary>
    /// Start node of the <see cref="SpringElement2D"/>.
    /// </summary>
    public Node2D StartNode { get; set; }

    /// <summary>
    /// End node of the <see cref="SpringElement2D"/>.
    /// </summary>
    public Node2D EndNode { get; set; }

    /// <inheritdoc/>
    public int Dof
    {
        get
        {
            var coords = new List<int>();
            coords.AddRange(StartNode.DegreeOfFreedomIndices);
            coords.AddRange(EndNode.DegreeOfFreedomIndices);
            return coords.Count;
        }
    }

    /// <inheritdoc/>
    public string Label { get; set; }

    /// <inheritdoc/>
    public Node2D[] Nodes => new[] { StartNode, EndNode };

    /// <inheritdoc/>
    public List<int> DegreeOfFreedoms
    {
        get
        {
            var coords = new List<int>();
            coords.AddRange(StartNode.DegreeOfFreedomIndices);
            coords.AddRange(EndNode.DegreeOfFreedomIndices);
            return coords;
        }
    }

    /// <inheritdoc/>
    public IList<ILoad> Loads { get; set; }

    /// <inheritdoc/>
    [Obsolete("spring element doesn't have cross section properties.", true)]
    public IFrame2DSection CrossSection { get; set; }

    /// <inheritdoc/>
    public double Length => Sqrt(Pow(EndNode.X - StartNode.X, 2) + Pow(EndNode.Y - StartNode.Y, 2));

    /// <inheritdoc/>
    public Structure.Structure ParentStructure { get; set; }

    /// <inheritdoc/>
    public Dictionary<LoadCase, double[]> GlobalEndForcesForLoadCase { get; private set; }

    /// <inheritdoc/>
    public DenseMatrix LocalCoordinateSystemMatrix { get; private set; }
    private DenseMatrix GetLocalCoordinateSystemMatrix()
    {
        var l = Length;
        var s = (EndNode.Y - StartNode.Y) / l;
        var c = (EndNode.X - StartNode.X) / l;
        var T = new DenseMatrix(3, 3);
        T[0, 0] = T[1, 1] = c;
        T[0, 1] = s;
        T[1, 0] = -s;
        T[2, 2] = 1;
        return T;
    }

    /// <inheritdoc/>
    public DenseMatrix TransformationMatrix { get; private set; }
    private DenseMatrix GetTransformationMatrix()
    {
        var T = new DenseMatrix(6, 6);
        var lcs = LocalCoordinateSystemMatrix;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                T[i, j] = lcs.At(i, j);
                T[i + 3, j + 3] = lcs.At(i, j);
            }
        }
        return T;
    }

    /// <inheritdoc/>
    public DenseMatrix LocalStiffnessMatrix { get; private set; }
    private DenseMatrix GetLocalStiffnessMatrix()
    {
        var kSpring = new[,]
        {
            { +K , 0 ,  0 , -K , 0,  0 },
            {  0 , 0 ,  0 ,  0 , 0,  0 },
            {  0 , 0 , +R ,  0 , 0, -R },
            { -K , 0 ,  0 , +K , 0,  0 },
            {  0 , 0 ,  0 ,  0 , 0,  0 },
            {  0 , 0 , -R ,  0 , 0, +R },
        };
        return DenseMatrix.OfArray(kSpring) as DenseMatrix;
    }

    /// <inheritdoc/>
    public DenseMatrix GlobalStiffnessMatrix { get; private set; }
    private DenseMatrix GetGlobalStiffnessMatrix()
    {
        var T = TransformationMatrix;
        var tt = T.Transpose();
        var ttKl = tt.Multiply(LocalStiffnessMatrix);
        return ttKl.Multiply(T) as DenseMatrix;
    }

    /// <inheritdoc/>
    public List<LinearMeshSegment> MeshSegments { get; }

    /// <inheritdoc/>
    public SortedSet<double> AdditionalMeshPoints { get; set; }

    /// <inheritdoc/>
    public void EvaluateGlobalFixedEndForces(LoadCase loadCase)
    {
        var f = new double[6];
        GlobalEndForcesForLoadCase.Add(loadCase, f);
    }

    /// <inheritdoc/>
    public DenseMatrix GetShapeFunctionAt(double x)
    {
        var l = Length;
        var xsi = x / l;

        var n1 = 1.0 - xsi;
        var nu = new[,]
        {
            {n1 , 0  , 0  , xsi , 0  , 0 },
            { 0 , n1 , 0  , 0  , xsi , 0 },
            { 0 , 0  , n1 , 0  , 0  , xsi}
        };

        return DenseMatrix.OfArray(nu) as DenseMatrix;
    }

    /// <inheritdoc/>
    public void Initialize()
    {
        LocalCoordinateSystemMatrix = GetLocalCoordinateSystemMatrix();
        TransformationMatrix = GetTransformationMatrix();
        LocalStiffnessMatrix = GetLocalStiffnessMatrix();
        GlobalStiffnessMatrix = GetGlobalStiffnessMatrix();
    }
}