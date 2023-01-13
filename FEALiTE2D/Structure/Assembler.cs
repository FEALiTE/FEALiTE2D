using CSparse.Double;
using FEALiTE2D.Loads;
using FEALiTE2D.Elements;
using System;
using System.Linq;

namespace FEALiTE2D.Structure;

/// <summary>
/// Assemble global stiffness matrices of elements as well as global load vector.
/// </summary>
internal class Assembler
{
    private readonly Structure _structure;

    /// <summary>
    /// Create a new instance of assembler class
    /// </summary>
    /// <param name="structure">a structure to assemble global stiffness matrices of elements as well as global load vector</param>
    internal Assembler(Structure structure)
    {
        _structure = structure;
    }

    /// <summary>
    /// Assemble structural stiffness matrix.
    /// </summary>
    internal SparseMatrix AssembleGlobalStiffnessMatrix()
    {
        var nDof = _structure.NDof;

        // check if there are free DOFs available.
        if (_structure.NDof == 0)
            throw new InvalidOperationException("There are no sufficient degrees of freedom, thus there are no displacements or rotations could occur!");

        // global stiffness matrix.
        var kg = new DenseMatrix(nDof, nDof);

        // start assembling
        foreach (var elem in _structure.Elements)
        {
            //TODO: support other types.

            var elemKg = elem.GlobalStiffnessMatrix;
            var elemCoords = elem.DegreeOfFreedoms; // element's coordinates vector.

            var dofElem = elem.Dof; //number of dof for the elements.
            int i;

            for (i = 0; i < dofElem; i++)
            {
                var ii = elemCoords[i];
                if (ii >= nDof) continue;
                for (var j = 0; j < dofElem; j++)
                {
                    var jj = elemCoords[j];
                    if (jj >= nDof) continue;
                    var kij = elemKg[i, j];
                    if (kij != 0) { kg[ii, jj] += kij; }
                }
            }
        }

        // assemble elastic supports data
        var elasticNodes = _structure.Nodes.Where(o => o.Support is NodalSpringSupport);
        foreach (var node in elasticNodes)
        {
            var dofNode = node.Dof; //number of dof for the node.
            int i;

            var nodeKg = ((NodalSpringSupport)node.Support).GlobalStiffnessMatrix;
            var elemCoords = node.DegreeOfFreedomIndices; // node's coordinates vector.

            for (i = 0; i < dofNode; i++)
            {
                var ii = elemCoords[i];
                if (ii >= nDof) continue;
                var kij = nodeKg[i, i];
                if (kij != 0) { kg[ii, ii] += kij; }
            }
        }

        return SparseMatrix.OfMatrix(kg) as SparseMatrix;

    }

    /// <summary>
    /// Assemble load vector for the give load case due to nodal loads..
    /// </summary>
    /// <param name="loadCase">load case</param>
    internal double[] AssembleGlobalEquivalentLoadVector(LoadCase loadCase)
    {
        // check if there are free DOFs available.
        if (_structure.NDof == 0)
            throw new InvalidOperationException("There are no sufficient degrees of freedom, thus there are no displacements or rotations could occur!");

        var nDof = _structure.NDof;

        // global fem vector.
        var qf = new double[nDof];


        foreach (var node in _structure.Nodes)
        {
            var nodeLoad = new double[3];

            // get node loads.
            //1- add external nodal loads
            foreach (var load in node.NodalLoads)
            {
                if (load.LoadCase != loadCase) continue;
                var f = load.GetGlobalFixedEndForces(node);

                nodeLoad[0] += f[0];
                nodeLoad[1] += f[1];
                nodeLoad[2] += f[2];
            }

            //2- get loads from connected elements to this node.
            var connectedElements = _structure.Elements.Where(o => o.Nodes.Contains(node)).ToList();

            if (!connectedElements.Any()) { throw new InvalidOperationException("Isolated node @" + node.Label); }

            // get connected elements at this node.
            foreach (var elem in connectedElements)
            {
                //TODO: support other types.
                if (elem is not FrameElement2D) continue;
                var fem = elem.GlobalEndForcesForLoadCase[loadCase];

                for (var i = 0; i < 3; i++)
                {
                    // add and reverse the sign of the force and moment then subtract P(nodal)  -(P(element)) will lead to positive sign
                    if (elem.Nodes[0] == node) { nodeLoad[i] -= fem[i]; }
                    if (elem.Nodes[1] == node) { nodeLoad[i] -= fem[i + 3]; }
                }
            }


            for (var i = 0; i < 3; i++)
            {
                if (node.DegreeOfFreedomIndices[i] < _structure.NDof)
                {
                    qf[node.DegreeOfFreedomIndices[i]] += nodeLoad[i];
                }
            }


            //3- get node loads due to support displacement for restraint nodes only.
            if (node.Dof == 3) /* not restrained */ { continue; }

            foreach (var dis in node.SupportDisplacementLoad)
            {
                if (dis.LoadCase != loadCase) { continue; }

                var dg = dis.GetGlobalFixedEndDisplacement(node);
                foreach (var elem in connectedElements)
                {
                    var d = new double[6];
                    if (elem is not FrameElement2D) continue;
                    for (var i = 0; i < 3; i++)
                    {
                        if (elem.Nodes[0] == node)
                            d[i] = dg[i];

                        if (elem.Nodes[1] == node)
                            d[i + 3] = dg[i];
                    }

                    // fixed end vector due to support displacement.
                    var fg = new double[6];
                    elem.GlobalStiffnessMatrix.Multiply(d, fg);

                    for (var i = 0; i < 3; i++)
                    {
                        if (elem.Nodes[0].DegreeOfFreedomIndices[i] < _structure.NDof)
                        {
                            qf[elem.Nodes[0].DegreeOfFreedomIndices[i]] -= fg[i];
                        }

                        if (elem.Nodes[1].DegreeOfFreedomIndices[i] < _structure.NDof)
                        {
                            qf[elem.Nodes[1].DegreeOfFreedomIndices[i]] -= fg[i + 3];
                        }

                    }
                }
            }
        }

        return qf;
    }
}