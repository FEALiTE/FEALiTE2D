using CSparse;
using FEALiTE2D.Elements;
using FEALiTE2D.Loads;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FEALiTE2D.Structure;

/// <summary>
/// Represent a structural model that has many elements connected to each other through nodes.
/// These elements are subjected to external actions.
/// To solve a structural model, the model must have at least one degree of freedom.
/// </summary>
[Serializable]
public class Structure
{
    /// <summary>
    /// Create a new instance of <see cref="Structure"/>.
    /// </summary>
    public Structure()
    {
        Nodes = new List<Node2D>();
        Elements = new List<IElement>();
        LoadCasesToRun = new List<LoadCase>();
        FixedEndLoadsVectors = new Dictionary<LoadCase, double[]>();
        DisplacementVectors = new Dictionary<LoadCase, double[]>();
        LinearMesher = new Meshing.LinearMesher();

    }

    /// <summary>
    /// Represents a list of Nodes that connect fem elements together.
    /// </summary>
    public List<Node2D> Nodes { get; set; }

    /// <summary>
    /// Represents a list of fem elements.
    /// </summary>
    public List<IElement> Elements { get; set; }

    /// <summary>
    /// A dictionary of coordinates and assembled global stiffness matrices
    /// </summary>
    public CSparse.Double.SparseMatrix StructuralStiffnessMatrix { get; set; }

    /// <summary>
    /// Assembled Fixed end forces of loads in a load case.
    /// </summary>
    public Dictionary<LoadCase, double[]> FixedEndLoadsVectors { get; private set; }

    /// <summary>
    /// displacement of nodes due to loads in a load case.
    /// </summary>
    public Dictionary<LoadCase, double[]> DisplacementVectors { get; private set; }

    /// <summary>
    /// Gets or sets the load cases to be included in analysis.
    /// </summary>
    public List<LoadCase> LoadCasesToRun { get; set; }

    /// <summary>
    /// Gets or sets the analysis result.
    /// </summary>
    public AnalysisStatus AnalysisStatus { get; private set; }

    /// <summary>
    /// Gets or sets the tolerance.
    /// </summary>
    public double Tolerance { get; set; }

    /// <summary>
    /// number of dof which is the sum of free dof in each node.
    /// </summary>
    public int nDOF { get; private set; }

    /// <summary>
    /// Get analysis results
    /// </summary>
    public PostProcessor Results { get; private set; }

    /// <summary>
    /// Get or set Linear mesher class for <see cref="IElement"/>.
    /// </summary>
    public Meshing.ILinearMesher LinearMesher { get; set; }

    /// <summary>
    /// Adds a node to the structure, We check if the node is already added to avoid duplicate nodes.
    /// </summary>
    /// <param name="node">The node.</param>
    public void AddNode(Node2D node)
    {
        if (!Nodes.Contains(node))
        {
            Nodes.Add(node);
            node.ParentStructure = this;
        }
    }

    /// <summary>
    /// Adds nodes to the structure, We check if the nodes are already added to avoid duplicate nodes.
    /// </summary>
    /// <param name="nodes">The nodes.</param>
    public void AddNode(params Node2D[] nodes)
    {
        foreach (var node in nodes)
            AddNode(node);
    }

    /// <summary>
    /// Adds elements to the structure.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="addNodes">Add the nodes of the element to Nodes list?</param>
    public void AddElement(IElement element, bool addNodes = false)
    {
        if (element == null)
            throw new NullReferenceException($"element {element.Label} is null");

        if (addNodes)
        {
            foreach (var n in element.Nodes)
            {
                AddNode(n);
            }
        }
        // check to see if this element already exists
        if (!Elements.Contains(element))
        {
            Elements.Add(element);
            element.Initialize();
            element.ParentStructure = this;
        }
    }

    /// <summary>
    /// Adds elements to the structure.
    /// </summary>
    /// <param name="elements">collection of elements.</param>
    /// <param name="addNodes">Add the nodes of the element to Nodes list?</param>
    public void AddElement(IEnumerable<IElement> elements, bool addNodes = false)
    {
        foreach (var item in elements)
        {
            AddElement(item, addNodes);
        }
    }

    /// <summary>
    /// Calculates fixed end forces and moments at each node of an element and add them to <see cref="IElement.GlobalEndForcesForLoadCase"/> dictionary.
    /// </summary>
    private void PrepareLoadsOnElements()
    {
        foreach (var element in Elements)
        {
            // get global fixed end forces for each load assigned to this element.
            foreach (var loadCase in LoadCasesToRun)
            {
                element.EvaluateGlobalFixedEndForces(loadCase);
            }
        }
    }

    /// <summary>
    /// Set up mesh segments of the element.
    /// </summary>
    private void SetUpMeshingSegments()
    {
        // generate discretization segments on the element.
        Elements.ForEach((IElement element) => { LinearMesher.SetupMeshSegments(element); });
    }

    /// <summary>
    ///  Order the nodes by number of dofs then renumber the node indexes according to that.
    /// </summary>
    private void ReNumberNodes()
    {
        nDOF = 0;
        Nodes = Nodes.OrderBy(i => i.Dof).ToList();

        // get total number of degrees of freedom by summing all ndof for each node.
        foreach (var node in Nodes)
        {
            nDOF += node.Dof;
        }

        var freeNumber = new Queue<int>(Enumerable.Range(0, nDOF));
        var restrainedNumber = new Queue<int>(Enumerable.Range(nDOF, Nodes.Count * 3 - nDOF));

        foreach (var node in Nodes)
        {
            node.DegreeOfFreedomIndices.Clear();

            // add a free number for a certain dof if this dof is free i.e not restrained

            if (!node.IsRestrained(NodalDegreeOfFreedom.Ux))
                node.DegreeOfFreedomIndices.Add(freeNumber.Dequeue());
            else
                node.DegreeOfFreedomIndices.Add(restrainedNumber.Dequeue());

            if (!node.IsRestrained(NodalDegreeOfFreedom.Uy))
                node.DegreeOfFreedomIndices.Add(freeNumber.Dequeue());
            else
                node.DegreeOfFreedomIndices.Add(restrainedNumber.Dequeue());

            if (!node.IsRestrained(NodalDegreeOfFreedom.Rz))
                node.DegreeOfFreedomIndices.Add(freeNumber.Dequeue());
            else
                node.DegreeOfFreedomIndices.Add(restrainedNumber.Dequeue());

        }
    }

    /// <summary>
    /// Solve the structure.
    /// </summary>
    public void Solve()
    {
        Console.WriteLine(" ================= FEALiTE Analysis Solver ================= ");
        Console.WriteLine(" FEALiTE2D V1.0.0 - Copyright (C) 2021 Mohamed S. Ibrahim");
        Console.WriteLine(" Linear Analysis of 1D structures.");
        Console.WriteLine($" Analysis Start: {DateTime.Now}.");

        var sw = System.Diagnostics.Stopwatch.StartNew();

        if (LoadCasesToRun.Count <= 0)
        {
            AnalysisStatus = AnalysisStatus.Failure;
            throw new InvalidOperationException("No load cases are set for analysis.");
        }

        PrepareLoadsOnElements();

        ReNumberNodes();

        var assembler = new Assembler(this);

        StructuralStiffnessMatrix = assembler.AssembleGlobalStiffnessMatrix();

        for (var i = 0; i < LoadCasesToRun.Count; i++)
        {
            var currentLC = LoadCasesToRun[i];
            var loadVec = assembler.AssembleGlobalEquivalentLoadVector(currentLC);
            FixedEndLoadsVectors.Add(currentLC, loadVec);
        }

        CSparse.Factorization.ISparseFactorization<double> cholesky = null;

        try
        {
            cholesky = CSparse.Double.Factorization.SparseCholesky.Create(StructuralStiffnessMatrix, ColumnOrdering.MinimumDegreeAtPlusA);
        }
        catch (Exception e)
        {
            if (e.Message.Contains("Matrix must be symmetric positive definite."))
            {
                cholesky = CSparse.Double.Factorization.SparseQR.Create(StructuralStiffnessMatrix, ColumnOrdering.Natural);
            }
        }

        for (var i = 0; i < LoadCasesToRun.Count; i++)
        {
            var currentLC = LoadCasesToRun[i];
            var displacementVector = new double[nDOF];

            cholesky.Solve(FixedEndLoadsVectors[currentLC], displacementVector);

            DisplacementVectors.Add(currentLC, displacementVector);
        }

        AnalysisStatus = AnalysisStatus.Successful;

        sw.Stop();
        Console.WriteLine($" No. of Equations: {nDOF}");
        Console.WriteLine($" Analysis End Date: {DateTime.Now}.");
        Console.WriteLine($" Analysis Took {sw.Elapsed.TotalSeconds} sec.");

        Results = new PostProcessor(this);
        SetUpMeshingSegments();
    }

}