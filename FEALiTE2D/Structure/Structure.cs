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
    public class Structure
    {
        /// <summary>
        /// Create a new instance of <see cref="Structure"/>.
        /// </summary>
        public Structure()
        {
            this.Nodes = new List<Node2D>();
            this.Elements = new List<IElement>();
            this.LoadCasesToRun = new List<LoadCase>();
            this.FixedEndLoadsVectors = new Dictionary<LoadCase, double[]>();
            this.DisplacementVectors = new Dictionary<LoadCase, double[]>();

        }

        /// <summary>
        /// Represents a list of Nodes that connects fem elements together.
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
        public AnalysisResult AnalysisResult { get; set; }

        /// <summary>
        /// Gets or sets the tolerance.
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        /// number of dof which is the sum of free dof in each node.
        /// </summary>
        public int nDOF { get; private set; }

        /// <summary>
        /// Adds a node to the structure, We check if the node is already added to avoid duplicate nodes.
        /// </summary>
        /// <param name="node">The node.</param>
        public void AddNode(Node2D node)
        {
            if (!Nodes.Contains(node))
            {
                this.Nodes.Add(node);
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
                this.AddNode(node);
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

            if (addNodes == true)
            {
                foreach (Node2D n in element.Nodes)
                {
                    this.AddNode(n);
                }
            }
            // check to see if this element already exists
            if (!this.Elements.Contains(element))
            {
                this.Elements.Add(element);
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
                this.AddElement(item, addNodes);
            }
        }

        /// <summary>
        /// Calculates fixed end forces and moments at each node of an element and add them to <see cref="IElement.GlobalEndForcesForLoadCase"/> dictionary.
        /// </summary>
        private void PrepareLoadsOnElements()
        {
            foreach (IElement element in this.Elements)
            {
                // get global fixed end forces for each load assigned to this element.
                foreach (LoadCase loadCase in this.LoadCasesToRun)
                {
                    double[] fg = element.EvaluateGlobalFixedEndForces(loadCase);
                    element.GlobalEndForcesForLoadCase.Add(loadCase, fg);
                }
            }
        }

        /// <summary>
        ///  Order the nodes by number of dofs then renumber the node indexes according to that.
        /// </summary>
        private void ReNumberNodes()
        {
            nDOF = 0;
            Nodes = Nodes.OrderBy(i => i.DOF).ToList();

            // get total number of degrees of freedom by summing all ndof for each node.
            foreach (Node2D node in Nodes)
            {
                nDOF += node.DOF;
            }

            Queue<int> freeNumber = new Queue<int>(Enumerable.Range(0, nDOF));
            Queue<int> restrainedNumber = new Queue<int>(Enumerable.Range(nDOF, Nodes.Count * 3 - nDOF));

            foreach (Node2D node in Nodes)
            {
                node.CoordNumbers.Clear();

                // add a free number for a certain dof if this dof is free i.e not restrained

                if (!node.IsRestrained(NodalDegreeOfFreedom.UX))
                    node.CoordNumbers.Add(freeNumber.Dequeue());
                else
                    node.CoordNumbers.Add(restrainedNumber.Dequeue());

                if (!node.IsRestrained(NodalDegreeOfFreedom.UY))
                    node.CoordNumbers.Add(freeNumber.Dequeue());
                else
                    node.CoordNumbers.Add(restrainedNumber.Dequeue());

                if (!node.IsRestrained(NodalDegreeOfFreedom.RZ))
                    node.CoordNumbers.Add(freeNumber.Dequeue());
                else
                    node.CoordNumbers.Add(restrainedNumber.Dequeue());

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

            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            if (this.LoadCasesToRun.Count <= 0)
            {
                AnalysisResult = AnalysisResult.Failure;
                throw new InvalidOperationException("No load cases are set for analysis.");
            }

            this.PrepareLoadsOnElements();

            this.ReNumberNodes();

            Assembler assembler = new Assembler(this);

            this.StructuralStiffnessMatrix = assembler.AssembleGlobalStiffnessMatrix();

            for (int i = 0; i < LoadCasesToRun.Count; i++)
            {
                LoadCase currentLC = this.LoadCasesToRun[i];
                double[] loadVec = assembler.AssembleGlobalEquivalentLoadVector(currentLC);
                this.FixedEndLoadsVectors.Add(currentLC, loadVec);
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

            for (int i = 0; i < LoadCasesToRun.Count; i++)
            {
                LoadCase currentLC = this.LoadCasesToRun[i];
                double[] displacementVector = new double[nDOF];

                cholesky.Solve(FixedEndLoadsVectors[currentLC], displacementVector);

                this.DisplacementVectors.Add(currentLC, displacementVector);
            }

            AnalysisResult = AnalysisResult.Successful;

            sw.Stop();
            Console.WriteLine($" No. of Equations: {this.nDOF}");
            Console.WriteLine($" Analysis End Date: {DateTime.Now}.");
            Console.WriteLine($" Analysis Took {sw.Elapsed.TotalSeconds} sec.");
        }


        /// <summary>
        /// Get Node's global displacement due to applied load in a load case.
        /// </summary>
        /// <param name="loadcase">load case</param>
        /// <returns>Nodal Displacement</returns>
        public Displacement GetNodeGlobalDisplacement(Node2D node, LoadCase loadCase)
        {
            Displacement nd = new Displacement();

            if (this.AnalysisResult == AnalysisResult.Successful)
            {
                double[] dVector = this.DisplacementVectors[loadCase];

                if (node.CoordNumbers[0] < dVector.Length)
                    nd.Ux = dVector[node.CoordNumbers[0]];

                if (node.CoordNumbers[1] < dVector.Length)
                    nd.Uy = dVector[node.CoordNumbers[1]];

                if (node.CoordNumbers[2] < dVector.Length)
                    nd.Rz = dVector[node.CoordNumbers[2]];
            }
            return nd;
        }


    }
}
