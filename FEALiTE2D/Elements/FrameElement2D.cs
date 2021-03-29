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
            this.Loads = new List<ILoad>();
            this.EndRelease = Frame2DEndRelease.NoRelease;
            this.GlobalEndForcesForLoadCase = new Dictionary<LoadCase, double[]>();
            this.MeshSegments = new List<Meshing.LinearMeshSegment>();
        }

        /// <summary>
        /// Creates a new instance of <see cref="FrameElement2D"/> Class
        /// </summary>
        /// <param name="label">name of the frame element</param>
        public FrameElement2D(string label) : this()
        {
            this.Label = label;
        }

        /// <summary>
        /// Creates a new instance of <see cref="FrameElement2D"/> Class
        /// </summary>
        /// <param name="startNode">Start node</param>
        /// <param name="endNode">End node</param>
        /// <param name="label">name of the frame</param>
        public FrameElement2D(Node2D startNode, Node2D endNode, string label) : this(label)
        {
            this.StartNode = startNode;
            this.EndNode = endNode;
            this.LocalCoordinateSystemMatrix = GetLocalCoordinateSystemMatrix();
            this.TransformationMatrix = GetTransformationMatrix();
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
        public List<Meshing.LinearMeshSegment> MeshSegments { get; set; }

        /// <summary>
        /// Get The shape function at a point along the frame elements, including displacement and rotation.
        /// </summary>
        /// <param name="x">distance measured from start node</param>
        public DenseMatrix GetShapeFunctionAt(double x)
        {
            double l = this.Length;
            double xsi = x / l;
            double xsi2 = xsi * xsi;
            double xsi3 = xsi * xsi * xsi;

            double N1 = 1.0 - xsi,
                   N2 = xsi,
                   N3 = 1.0 - 3 * xsi2 + 2 * xsi3,
                   N4 = l * (xsi - 2 * xsi2 + xsi3),
                   N5 = 3 * xsi2 - 2 * xsi3,
                   N6 = l * (-xsi2 + xsi3),
                   N7 = 6.0 * (-xsi + xsi2) / l,
                   N8 = 1 - 4 * xsi - 3 * xsi2,
                   N9 = 6.0 * (xsi - xsi2) / l,
                   N10 = -2 * xsi + 3.0 * xsi2;
            double[,] nu = new double[,]
            {
                {N1 , 0 , 0 , N2, 0 ,  0 },
                { 0 , N3, N4, 0 , N5, N6 },
                { 0 , N7, N8, 0 , N9, N10}
            };

            return DenseMatrix.OfArray(nu) as DenseMatrix;
        }

        public DenseMatrix GetConstitutiveMatrix()
        {
            DenseMatrix D = new DenseMatrix(3, 3);
            D[0, 0] = CrossSection.A * CrossSection.Material.E;
            D[1, 1] = CrossSection.Ax * CrossSection.Material.G;
            D[2, 2] = CrossSection.Ix * CrossSection.Material.E;
            return D;
        }

        public DenseMatrix GetBmatrixAt(double x)
        {
            double l = this.Length;
            double l2 = l * l;
            double xsi = x / l;

            DenseMatrix B = new DenseMatrix(3, 6);

            B[0, 0] = -1 / l;
            B[0, 3] = +1 / l;

            B[2, 1] = +6 * (2 * xsi - 1) / l2;
            B[2, 4] = -6 * (2 * xsi - 1) / l2;

            B[2, 2] = (6 * xsi - 4) / l;
            B[2, 5] = (6 * xsi - 2) / l;

            return B;
        }

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
            switch (this.EndRelease)
            {
                default:
                case Frame2DEndRelease.NoRelease:
                    {
                        return kl1_1();
                    }
                case Frame2DEndRelease.StartRelease:
                    {
                        return kl0_1();
                    }
                case Frame2DEndRelease.EndRelease:
                    {
                        return kl1_0();
                    }
                case Frame2DEndRelease.FullRlease:
                    {
                        return kl0_0();
                    }
            }
        }

        /// <summary>
        /// calculate local stiffness matrix when the frame has no releases
        /// </summary>
        private DenseMatrix kl1_1()
        {
            double l = this.Length;
            double l2 = l * l;
            double l3 = l * l * l;
            double EAL = this.CrossSection.Material.E * this.CrossSection.A / l;
            double EIL = this.CrossSection.Material.E * this.CrossSection.Ix / l;
            double EIL2 = this.CrossSection.Material.E * this.CrossSection.Ix / l2;
            double EIL3 = this.CrossSection.Material.E * this.CrossSection.Ix / l3;

            DenseMatrix k = new DenseMatrix(6, 6);

            k[0, 0] = EAL;
            k[3, 3] = EAL;
            k[0, 3] = -EAL;
            k[3, 0] = -EAL;

            k[1, 1] = 12 * EIL3;
            k[4, 4] = 12 * EIL3;
            k[1, 4] = -12 * EIL3;
            k[4, 1] = -12 * EIL3;

            k[2, 1] = 6 * EIL2;
            k[5, 1] = 6 * EIL2;
            k[1, 2] = 6 * EIL2;
            k[1, 5] = 6 * EIL2;
            k[2, 4] = -6 * EIL2;
            k[5, 4] = -6 * EIL2;
            k[4, 2] = -6 * EIL2;
            k[4, 5] = -6 * EIL2;

            k[2, 2] = 4 * EIL;
            k[5, 5] = 4 * EIL;

            k[2, 5] = 2 * EIL;
            k[5, 2] = 2 * EIL;
            return k;
        }

        /// <summary>
        /// calculate local stiffness matrix when there is a release at it's start
        /// </summary>
        private DenseMatrix kl0_1()
        {
            double l = this.Length;
            double l2 = l * l;
            double l3 = l * l * l;
            double EAL = this.CrossSection.Material.E * this.CrossSection.A / l;
            double EIL = this.CrossSection.Material.E * this.CrossSection.Ix / l;
            double EIL2 = this.CrossSection.Material.E * this.CrossSection.Ix / l2;
            double EIL3 = this.CrossSection.Material.E * this.CrossSection.Ix / l3;

            DenseMatrix k = new DenseMatrix(6, 6);

            k[0, 0] = EAL;
            k[3, 3] = EAL;
            k[0, 3] = -EAL;
            k[3, 0] = -EAL;

            k[1, 1] = 3 * EIL3;
            k[4, 4] = 3 * EIL3;
            k[1, 4] = -3 * EIL3;
            k[4, 1] = -3 * EIL3;

            k[5, 1] = 3 * EIL2;
            k[1, 5] = 3 * EIL2;
            k[5, 4] = -3 * EIL2;
            k[4, 5] = -3 * EIL2;

            k[5, 5] = 3 * EIL;
            return k;
        }

        /// <summary>
        /// calculate local stiffness matrix when there is a release at it's end
        /// </summary>
        private DenseMatrix kl1_0()
        {
            double l = this.Length;
            double l2 = l * l;
            double l3 = l * l * l;
            double EAL = this.CrossSection.Material.E * this.CrossSection.A / l;
            double EIL = this.CrossSection.Material.E * this.CrossSection.Ix / l;
            double EIL2 = this.CrossSection.Material.E * this.CrossSection.Ix / l2;
            double EIL3 = this.CrossSection.Material.E * this.CrossSection.Ix / l3;

            DenseMatrix k = new DenseMatrix(6, 6);

            k[0, 0] = EAL;
            k[3, 3] = EAL;
            k[0, 3] = -EAL;
            k[3, 0] = -EAL;

            k[1, 1] = 3 * EIL3;
            k[4, 4] = 3 * EIL3;
            k[1, 4] = -3 * EIL3;
            k[4, 1] = -3 * EIL3;

            k[2, 1] = 3 * EIL2;
            k[1, 2] = 3 * EIL2;
            k[2, 4] = -3 * EIL2;
            k[4, 2] = -3 * EIL2;

            k[2, 2] = 3 * EIL;
            return k;
        }

        /// <summary>
        /// calculate local stiffness matrix when it's fully released.
        /// </summary>
        private DenseMatrix kl0_0()
        {
            double l = this.Length;
            double EAL = this.CrossSection.Material.E * this.CrossSection.A / l;

            DenseMatrix k = new DenseMatrix(6, 6);

            k[0, 0] = EAL;
            k[3, 3] = EAL;
            k[0, 3] = -EAL;
            k[3, 0] = -EAL;
            return k;
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
        public void EvaluateGlobalFixedEndForces(LoadCase loadCase)
        {
            double[] f = new double[6];

            // get loads that are in current load case
            IEnumerable<ILoad> loads = this.Loads.Where(xx => xx.LoadCase == loadCase);
            foreach (ILoad load in loads)
            {
                double[] fg = load.GetGlobalFixedEndForces(this);
                f[0] -= fg[0];
                f[1] -= fg[1];
                f[2] -= fg[2];
                f[3] -= fg[3];
                f[4] -= fg[4];
                f[5] -= fg[5];
            }

            double l = this.Length;
            switch (this.EndRelease)
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
                case Frame2DEndRelease.FullRlease:
                    {
                        f[1] -= (f[2] + f[5]) / l;
                        f[4] += (f[2] + f[5]) / l;
                        f[2] = 0;
                        f[5] = 0;
                        break;
                    }
            }
            this.GlobalEndForcesForLoadCase.Add(loadCase, f);
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
