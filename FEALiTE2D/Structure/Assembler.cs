using CSparse.Double;
using FEALiTE2D.Loads;
using FEALiTE2D.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FEALiTE2D.Structure
{
    /// <summary>
    /// Assemble global stiffness matrices of elements as well as global load vector.
    /// </summary>
    internal class Assembler
    {
        private readonly Structure structure;

        /// <summary>
        /// Create a new instance of assembler class
        /// </summary>
        /// <param name="structure">a structure to assemble global stiffness matrices of elements as well as global load vector</param>
        internal Assembler(Structure structure)
        {
            this.structure = structure;
        }

        /// <summary>
        /// Assemble structural stiffness matrix.
        /// </summary>
        internal SparseMatrix AssembleGlobalStiffnessMatrix()
        {
            var nDOF = structure.nDOF;

            // check if there are free dofs available.
            if (structure.nDOF == 0)
                throw new InvalidOperationException("There are no sufficient degrees of freedom, thus there are no displacements or rotations could occur!");

            // global stiffness matrix.
            var Kg = new DenseMatrix(nDOF, nDOF);

            // start assembling
            foreach (var elem in structure.Elements)
            {
                //TODO: support other types.

                var elem_kg = elem.GlobalStiffnessMatrix;
                var elemCoord = elem.DegreeOfFreedoms; // element's coordinates vector.

                var dofElem = elem.DOF; //number of dof for the elements.
                double kij;
                int i, j, ii, jj;

                for (i = 0; i < dofElem; i++)
                {
                    ii = elemCoord[i];
                    if (ii < nDOF)
                    {
                        for (j = 0; j < dofElem; j++)
                        {
                            jj = elemCoord[j];
                            if (jj < nDOF)
                            {
                                kij = elem_kg[i, j];
                                if (kij != 0)
                                {
                                    Kg[ii, jj] += kij;
                                }
                            }
                        }
                    }
                }
            }

            // assemble elastic supports data
            var elasticNodes = structure.Nodes.Where(o => o.Support is NodalSpringSupport);
            foreach (var node in elasticNodes)
            {
                var dofNode = node.DOF; //number of dof for the node.
                double kij;
                int i, ii;

                var node_kg = ((NodalSpringSupport)node.Support).GlobalStiffnessMatrix;
                var elemCoord = node.CoordNumbers; // node's coordinates vector.

                for (i = 0; i < dofNode; i++)
                {
                    ii = elemCoord[i];
                    if (ii < nDOF)
                    {
                        kij = node_kg[i, i];
                        if (kij != 0)
                        {
                            Kg[ii, ii] += kij;
                        }
                    }
                }
            }

            return SparseMatrix.OfMatrix(Kg) as SparseMatrix;

        }

        /// <summary>
        /// Assemble load vector for the give load case due to nodal loads..
        /// </summary>
        /// <param name="loadCase">load case</param>
        internal double[] AssembleGlobalEquivalentLoadVector(LoadCase loadCase)
        {
            // check if there are free dofs available.
            if (structure.nDOF == 0)
                throw new InvalidOperationException("There are no sufficient degrees of freedom, thus there are no displacements or rotations could occur!");

            var nDOF = structure.nDOF;

            // global fem vector.
            var Qf = new double[nDOF];


            foreach (var node in structure.Nodes)
            {
                var nodeLoad = new double[3];

                // get node loads.
                //1- add external nodal loads
                foreach (var load in node.NodalLoads)
                {
                    if (load.LoadCase == loadCase)
                    {
                        var f = load.GetGlobalFixedEndForces(node);

                        nodeLoad[0] += f[0];
                        nodeLoad[1] += f[1];
                        nodeLoad[2] += f[2];
                    }
                }

                //2- get loads from connected elements to this node.
                var connectedElements = structure.Elements.Where(o => o.Nodes.Contains(node));

                if (connectedElements.Count() == 0)
                    throw new InvalidOperationException("Isolated node @" + node.Label);

                // get connected elements at this node.
                foreach (var elem in connectedElements)
                {
                    //TODO: support other types.
                    if (elem is FrameElement2D)
                    {
                        var fem = elem.GlobalEndForcesForLoadCase[loadCase];

                        for (var i = 0; i < 3; i++)
                        {
                            // add and reverse the sign of the force and moment then subtract P(nodal)  -(P(element)) will lead to positive sign
                            if (elem.Nodes[0] == node)
                                nodeLoad[i] -= fem[i];

                            if (elem.Nodes[1] == node)
                                nodeLoad[i] -= fem[i + 3];
                        }
                    }
                }


                for (var i = 0; i < 3; i++)
                    if (node.CoordNumbers[i] < structure.nDOF)
                        Qf[node.CoordNumbers[i]] += nodeLoad[i];


                //3- get node loads due to support displacement for restraint nodes only.
                if (node.DOF == 3) // not restrained
                    continue;

                foreach (var dis in node.SupportDisplacementLoad)
                {
                    if (dis.LoadCase != loadCase)
                        continue;

                    var dg = dis.GetGlobalFixedEndDisplacement(node);
                    foreach (var elem in connectedElements)
                    {
                        var d = new double[6];

                        if (elem is FrameElement2D)
                        {
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
                                if (elem.Nodes[0].CoordNumbers[i] < structure.nDOF)
                                    Qf[elem.Nodes[0].CoordNumbers[i]] -= fg[i];

                                if (elem.Nodes[1].CoordNumbers[i] < structure.nDOF)
                                    Qf[elem.Nodes[1].CoordNumbers[i]] -= fg[i + 3];

                            }
                        }
                    }
                }
            }

            return Qf;

        }
    }
}