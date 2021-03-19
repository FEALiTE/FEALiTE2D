using CSparse.Double;
using FEALiTE2D.CrossSections;
using FEALiTE2D.Loads;
using FEALiTE2D.Meshing;
using System;
using System.Collections.Generic;
using static System.Math;

namespace FEALiTE2D.Elements
{
    /// <summary>
    /// Represents a spring element/Fictitious bar in 2d space that has a spring stiffness and has 2 dof at each node. Spring may be longitudinal or rotational
    /// </summary>
    public class SpringElement2D : IElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="SpringElement2D"/> class.
        /// </summary>
        public SpringElement2D()
        {
            this.Loads = new List<ILoad>();
            this.GlobalEndForcesForLoadCase = new Dictionary<LoadCase, double[]>();
            this.MeshSegments = new List<Meshing.LinearMeshSegment>();
            this.LoadCasesToIgnore = new List<LoadCase>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="SpringElement2D"/> class.
        /// </summary>
        /// <param name="startNode">Start node</param>
        /// <param name="endNode">End node</param>
        /// <param name="label">name of the spring</param>
        /// <param name="orientation">orientation of the sprig</param>
        public SpringElement2D(Node2D startNode, Node2D endNode, string label) : this()
        {
            this.StartNode = startNode;
            this.EndNode = endNode;
            this.Label = label;
            this.LocalCoordinateSystemMatrix = GetLocalCoordinateSystemMatrix();
            this.TransformationMatrix = GetTransformationMatrix();
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
        public int DOF { get; private set; }

        /// <inheritdoc/>
        public string Label { get; set; }

        /// <inheritdoc/>
        public Node2D[] Nodes => new[] { StartNode, EndNode };

        /// <inheritdoc/>
        public List<int> DegreeOfFreedoms
        {
            get
            {
                List<int> coords = new List<int>();
                coords.AddRange(StartNode.CoordNumbers);
                coords.AddRange(EndNode.CoordNumbers);

                this.DOF = coords.Count;

                return coords;
            }
        }

        public IList<ILoad> Loads { get; set; }

        [Obsolete("spring element doesn't have cross section properties.", true)]
        public IFrame2DSection CrossSection { get; set; }

        /// <inheritdoc/>
        public double Length => Sqrt(Pow(EndNode.X - StartNode.X, 2) + Pow(EndNode.Y - StartNode.Y, 2));

        /// <inheritdoc/>
        public Structure.Structure ParentStructure { get; set; }

        /// <inheritdoc/>
        public Dictionary<LoadCase, double[]> GlobalEndForcesForLoadCase { get; private set; }

        /// <inheritdoc/>
        public bool IsActive { get; set; }

        /// <inheritdoc/>
        public bool TensionOnly { get; set; }

        /// <inheritdoc/>
        public List<LoadCase> LoadCasesToIgnore { get; set; }

        /// <inheritdoc/>
        public DenseMatrix LocalCoordinateSystemMatrix { get; private set; }
        private DenseMatrix GetLocalCoordinateSystemMatrix()
        {
            double l = this.Length;
            double s = (EndNode.Y - StartNode.Y) / l;
            double c = (EndNode.X - StartNode.X) / l;
            DenseMatrix T = new DenseMatrix(3, 3);
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
            DenseMatrix T = new DenseMatrix(6, 6);
            var lcs = this.LocalCoordinateSystemMatrix;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
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
            double[,] kspring = new double[6, 6]
            {
                { +K , 0 ,  0 , -K , 0,  0 },
                {  0 , 0 ,  0 ,  0 , 0,  0 },
                {  0 , 0 , +R ,  0 , 0, -R },
                { -K , 0 ,  0 , +K , 0,  0 },
                {  0 , 0 ,  0 ,  0 , 0,  0 },
                {  0 , 0 , -R ,  0 , 0, +R },
            };
            return DenseMatrix.OfArray(kspring) as DenseMatrix;
        }

        /// <inheritdoc/>
        public DenseMatrix GlobalStiffnessMatrix { get; private set; }
        private DenseMatrix GetGlobalStiffnessMatrix()
        {
            var T = this.TransformationMatrix;
            var Tt = T.Transpose();
            var Tt_Kl = Tt.Multiply(LocalStiffnessMatrix);
            return Tt_Kl.Multiply(T) as DenseMatrix;
        }

        /// <inheritdoc/>
        public List<Meshing.LinearMeshSegment> MeshSegments { get; set; }

        /// <inheritdoc/>
        public void EvaluateGlobalFixedEndForces(LoadCase loadCase)
        {
            double[] f = new double[6];
            this.GlobalEndForcesForLoadCase.Add(loadCase, f);
        }

        /// <inheritdoc/>
        public DenseMatrix GetShapeFunctionAt(double x)
        {
            double l = this.Length;
            double xsi = x / l;

            double N1 = 1.0 - xsi, N2 = xsi;
            double[,] nu = new double[,]
            {
                {N1 , 0  , 0  , N2 , 0  , 0 },
                { 0 , N1 , 0  , 0  , N2 , 0 },
                { 0 , 0  , N1 , 0  , 0  , N2}
            };

            return DenseMatrix.OfArray(nu) as DenseMatrix;
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            this.LocalCoordinateSystemMatrix = GetLocalCoordinateSystemMatrix();
            this.TransformationMatrix = GetTransformationMatrix();
            this.LocalStiffnessMatrix = GetLocalStiffnessMatrix();
            this.GlobalStiffnessMatrix = GetGlobalStiffnessMatrix();
        }
    }

}