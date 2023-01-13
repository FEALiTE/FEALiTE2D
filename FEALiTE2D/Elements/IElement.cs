using FEALiTE2D.CrossSections;
using FEALiTE2D.Loads;
using System.Collections.Generic;

namespace FEALiTE2D.Elements
{
    /// <summary>
    /// Represents a class for <see cref="IElement"/> interface
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Gets the number of degrees of freedom of this element.
        /// </summary>
        int DOF { get; }

        /// <summary>
        /// Name of the <see cref="IElement"/>
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        Node2D[] Nodes { get; }

        /// <summary>
        /// Gets the coordinates numbers of degrees of freedom of the nodes connected to this element.
        /// </summary>
        List<int> DegreeOfFreedoms { get; }

        /// <summary>
        /// Gets or sets the loads.
        /// </summary>
        IList<ILoad> Loads { get; set; }

        /// <summary>
        /// Gets or sets the cross section for this element.
        /// </summary>
        IFrame2DSection CrossSection { get; set; }

        /// <summary>
        /// Length of the member.
        /// </summary>
        double Length { get; }

        /// <summary>
        /// Gets or sets the parent structure that this element is part of it.
        /// </summary>
        Structure.Structure ParentStructure { get; set; }

        /// <summary>
        /// This is a dictionary for global fixed end forces for each <see cref="IElement"/> at a load case.
        /// this is used to store these values to reuse them later instead of recalling them.
        /// </summary>
        Dictionary<LoadCase, double[]> GlobalEndForcesForLoadCase { get; }

        /// <summary>
        /// Gets the local coordinate system, This should be called after <see cref="Initialize"/>
        /// </summary>
        CSparse.Double.DenseMatrix LocalCoordinateSystemMatrix { get; }

        /// <summary>
        /// Gets the transformation matrix, This should be called after <see cref="Initialize"/>
        /// </summary>
        CSparse.Double.DenseMatrix TransformationMatrix { get; }

        /// <summary>
        /// Gets the local stiffness matrix, This should be called after <see cref="Initialize"/>
        /// </summary>
        CSparse.Double.DenseMatrix LocalStiffnessMatrix { get; }

        /// <summary>
        /// Gets the global stiffness matrix, This should be called after <see cref="Initialize"/>
        /// </summary>
        CSparse.Double.DenseMatrix GlobalStiffnessMatrix { get; }

        /// <summary>
        /// A list of mesh segments of change in external forces and moments.
        /// </summary>
       List<Meshing.LinearMeshSegment> MeshSegments { get; }

        /// <summary>
        /// A collection of additional locations on the <see cref="FrameElement2D"/> to mesh the elements at these locations.
        /// </summary>
        SortedSet<double> AdditionalMeshPoints { get; set; }

        /// <summary>
        /// Initializes the <see cref="IElement"/> to calculate its matrices or other needed properties,
        /// so this will save time when we calculate all properties once and store them into variables instead of 
        /// recalculating them every time they are called.
        /// <para>You must  call this method before using it's properties like <see cref="LocalStiffnessMatrix"/>, <see cref="GlobalStiffnessMatrix"/>, etc. this may occur in test cases.</para>
        /// </summary>
        void Initialize();

        /// <summary>
        /// Evaluate the global fixed end forces of loads at given load case.
        /// </summary>
        void EvaluateGlobalFixedEndForces(LoadCase loadCase);

        /// <summary>
        /// Get The shape function at a point along the frame elements, including displacement and rotation.
        /// </summary>
        /// <param name="x">distance measured from start node</param>
        CSparse.Double.DenseMatrix GetShapeFunctionAt(double x);

    }
}
