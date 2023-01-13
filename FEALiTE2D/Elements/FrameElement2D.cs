using CSparse.Double;
using FEALiTE2D.CrossSections;
using FEALiTE2D.Loads;
using System.Linq;
using System.Collections.Generic;
using static System.Math;

namespace FEALiTE2D.Elements
{
    /// <summary>
    /// Represents a class for frame elements, each frame element has 3 dof at each node.
    /// </summary>
    [System.Serializable]
    public class FrameElement2D : IElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="FrameElement2D"/> Class
        /// </summary>
        public FrameElement2D()
        {
            Loads = new List<ILoad>();
            EndRelease = Frame2DEndRelease.NoRelease;
            GlobalEndForcesForLoadCase = new Dictionary<LoadCase, double[]>();
            MeshSegments = new List<Meshing.LinearMeshSegment>();
            AdditionalMeshPoints = new SortedSet<double>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="FrameElement2D"/> Class
        /// </summary>
        /// <param name="label">name of the frame element</param>
        public FrameElement2D(string label) : this()
        {
            Label = label;
        }

        /// <summary>
        /// Creates a new instance of <see cref="FrameElement2D"/> Class
        /// </summary>
        /// <param name="startNode">Start node</param>
        /// <param name="endNode">End node</param>
        /// <param name="label">name of the frame</param>
        public FrameElement2D(Node2D startNode, Node2D endNode, string label) : this(label)
        {
            StartNode = startNode;
            EndNode = endNode;
            LocalCoordinateSystemMatrix = GetLocalCoordinateSystemMatrix();
            TransformationMatrix = GetTransformationMatrix();
        }

        /// <summary>
        /// Start node of the <see cref="FrameElement2D"/>.
        /// </summary>
        public Node2D StartNode { get; set; }

        /// <summary>
        /// End node of the <see cref="FrameElement2D"/>.
        /// </summary>
        public Node2D EndNode { get; set; }

        /// <summary>
        /// Cross section of the <see cref="FrameElement2D"/>.
        /// </summary>
        public IFrame2DSection CrossSection { get; set; }

        /// <inheritdoc/>
        public double Length => Sqrt(Pow(EndNode.X - StartNode.X, 2) + Pow(EndNode.Y - StartNode.Y, 2));

        /// <inheritdoc/>
        public int Dof { get; private set; }

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

                Dof = coords.Count;

                return coords;
            }
        }

        /// <summary>
        /// Gets or sets the end release condition of the <see cref="FrameElement2D"/>
        /// </summary>
        public Frame2DEndRelease EndRelease { get; set; }

        /// <summary>
        /// Loads on the <see cref="FrameElement2D"/>.
        /// </summary>
        public IList<ILoad> Loads { get; set; }

        /// <inheritdoc/>
        public Structure.Structure ParentStructure { get; set; }

        /// <inheritdoc/>
        public Dictionary<LoadCase, double[]> GlobalEndForcesForLoadCase { get; private set; }

        /// <inheritdoc/>
        public List<Meshing.LinearMeshSegment> MeshSegments { get; }

        /// <inheritdoc/>
        public SortedSet<double> AdditionalMeshPoints { get; set; }

        /// <summary>
        /// Get The shape function at a point along the frame elements, including displacement and rotation.
        /// </summary>
        /// <param name="x">distance measured from start node</param>
        public DenseMatrix GetShapeFunctionAt(double x)
        {
            var l = Length;
            var xsi = x / l;
            var xsi2 = xsi * xsi;
            var xsi3 = xsi * xsi * xsi;

            double n1 = 1.0 - xsi,
                n3 = 1.0 - 3 * xsi2 + 2 * xsi3,
                   n4 = l * (xsi - 2 * xsi2 + xsi3),
                   n5 = 3 * xsi2 - 2 * xsi3,
                   n6 = l * (-xsi2 + xsi3),
                   n7 = 6.0 * (-xsi + xsi2) / l,
                   n8 = 1 - 4 * xsi - 3 * xsi2,
                   n9 = 6.0 * (xsi - xsi2) / l,
                   n10 = -2 * xsi + 3.0 * xsi2;
            var nu = new[,]
            {
                {n1 , 0 , 0 , xsi, 0 ,  0 },
                { 0 , n3, n4, 0 , n5, n6 },
                { 0 , n7, n8, 0 , n9, n10}
            };

            return DenseMatrix.OfArray(nu) as DenseMatrix;
        }

        /// <summary>
        /// Get Constitutive matrix.
        /// </summary>
        public DenseMatrix GetConstitutiveMatrix() =>
            new DenseMatrix(3, 3)
            {
                [0, 0] = CrossSection.A * CrossSection.Material.E,
                [1, 1] = CrossSection.Ax * CrossSection.Material.G,
                [2, 2] = CrossSection.Ix * CrossSection.Material.E
            };

        /// <summary>
        /// Get strain gradient matrix.
        /// </summary>
        /// <param name="x">distance from start of the element</param>
        public DenseMatrix GetSubMatrix(double x)
        {
            var l = Length;
            var l2 = l * l;
            var xsi = x / l;

            return new DenseMatrix(3, 6)
            {
                [0, 0] = -1 / l,
                [0, 3] = +1 / l,
                [2, 1] = +6 * (2 * xsi - 1) / l2,
                [2, 4] = -6 * (2 * xsi - 1) / l2,
                [2, 2] = (6 * xsi - 4) / l,
                [2, 5] = (6 * xsi - 2) / l
            };
        }

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
            switch (EndRelease)
            {
                default:
                case Frame2DEndRelease.NoRelease:
                    {
                        return Kl1_1();
                    }
                case Frame2DEndRelease.StartRelease:
                    {
                        return Kl0_1();
                    }
                case Frame2DEndRelease.EndRelease:
                    {
                        return Kl1_0();
                    }
                case Frame2DEndRelease.FullRelease:
                    {
                        return Kl0_0();
                    }
            }
        }

        /// <summary>
        /// calculate local stiffness matrix when the frame has no releases
        /// </summary>
        private DenseMatrix Kl1_1()
        {
            var l = Length;
            var l2 = l * l;
            var l3 = l * l * l;
            var eal = CrossSection.Material.E * CrossSection.A / l;
            var eil = CrossSection.Material.E * CrossSection.Ix / l;
            var eil2 = CrossSection.Material.E * CrossSection.Ix / l2;
            var eil3 = CrossSection.Material.E * CrossSection.Ix / l3;

            return new DenseMatrix(6, 6)
            {
                [0, 0] = eal,
                [3, 3] = eal,
                [0, 3] = -eal,
                [3, 0] = -eal,
                [1, 1] = 12 * eil3,
                [4, 4] = 12 * eil3,
                [1, 4] = -12 * eil3,
                [4, 1] = -12 * eil3,
                [2, 1] = 6 * eil2,
                [5, 1] = 6 * eil2,
                [1, 2] = 6 * eil2,
                [1, 5] = 6 * eil2,
                [2, 4] = -6 * eil2,
                [5, 4] = -6 * eil2,
                [4, 2] = -6 * eil2,
                [4, 5] = -6 * eil2,
                [2, 2] = 4 * eil,
                [5, 5] = 4 * eil,
                [2, 5] = 2 * eil,
                [5, 2] = 2 * eil
            };
        }

        /// <summary>
        /// calculate local stiffness matrix when there is a release at it's start
        /// </summary>
        private DenseMatrix Kl0_1()
        {
            var l = Length;
            var l2 = l * l;
            var l3 = l * l * l;
            var eal = CrossSection.Material.E * CrossSection.A / l;
            var eil = CrossSection.Material.E * CrossSection.Ix / l;
            var eil2 = CrossSection.Material.E * CrossSection.Ix / l2;
            var eil3 = CrossSection.Material.E * CrossSection.Ix / l3;

            return new DenseMatrix(6, 6)
            {
                [0, 0] = eal,
                [3, 3] = eal,
                [0, 3] = -eal,
                [3, 0] = -eal,
                [1, 1] = 3 * eil3,
                [4, 4] = 3 * eil3,
                [1, 4] = -3 * eil3,
                [4, 1] = -3 * eil3,
                [5, 1] = 3 * eil2,
                [1, 5] = 3 * eil2,
                [5, 4] = -3 * eil2,
                [4, 5] = -3 * eil2,
                [5, 5] = 3 * eil
            };
        }

        /// <summary>
        /// calculate local stiffness matrix when there is a release at it's end
        /// </summary>
        private DenseMatrix Kl1_0()
        {
            var l = Length;
            var l2 = l * l;
            var l3 = l * l * l;
            var eal = CrossSection.Material.E * CrossSection.A / l;
            var eil = CrossSection.Material.E * CrossSection.Ix / l;
            var eil2 = CrossSection.Material.E * CrossSection.Ix / l2;
            var eil3 = CrossSection.Material.E * CrossSection.Ix / l3;

            return new DenseMatrix(6, 6)
            {
                [0, 0] = eal,
                [3, 3] = eal,
                [0, 3] = -eal,
                [3, 0] = -eal,
                [1, 1] = 3 * eil3,
                [4, 4] = 3 * eil3,
                [1, 4] = -3 * eil3,
                [4, 1] = -3 * eil3,
                [2, 1] = 3 * eil2,
                [1, 2] = 3 * eil2,
                [2, 4] = -3 * eil2,
                [4, 2] = -3 * eil2,
                [2, 2] = 3 * eil
            };
        }

        /// <summary>
        /// calculate local stiffness matrix when it's fully released.
        /// </summary>
        private DenseMatrix Kl0_0()
        {
            var l = Length;
            var eal = CrossSection.Material.E * CrossSection.A / l;

            return new DenseMatrix(6, 6)
            {
                [0, 0] = eal,
                [3, 3] = eal,
                [0, 3] = -eal,
                [3, 0] = -eal
            };
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
        public void EvaluateGlobalFixedEndForces(LoadCase loadCase)
        {
            var f = new double[6];

            // get loads that are in current load case
            var loads = Loads.Where(xx => xx.LoadCase == loadCase);
            foreach (var load in loads)
            {
                var fg = load.GetGlobalFixedEndForces(this);
                f[0] -= fg[0];
                f[1] -= fg[1];
                f[2] -= fg[2];
                f[3] -= fg[3];
                f[4] -= fg[4];
                f[5] -= fg[5];
            }

            var l = Length;
            switch (EndRelease)
            {
                default:
                case Frame2DEndRelease.NoRelease:
                    break;
                case Frame2DEndRelease.StartRelease:
                    {
                        f[1] -= 1.5 * f[2] / l;
                        f[4] += 1.5 * f[2] / l;
                        f[5] -= 0.5 * f[2];
                        f[2] = 0;
                        break;
                    }
                case Frame2DEndRelease.EndRelease:
                    {
                        f[1] -= 1.5 * f[5] / l;
                        f[2] -= 0.5 * f[5];
                        f[4] += 1.5 * f[5] / l;
                        f[5] = 0;
                        break;
                    }
                case Frame2DEndRelease.FullRelease:
                    {
                        f[1] -= (f[2] + f[5]) / l;
                        f[4] += (f[2] + f[5]) / l;
                        f[2] = 0;
                        f[5] = 0;
                        break;
                    }
            }
            GlobalEndForcesForLoadCase.Add(loadCase, f);
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
        FullRelease
    }
}
