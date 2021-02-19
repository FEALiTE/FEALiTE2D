using CSparse.Double;
using FEALiTE2D.Loads;
using FEALiTE2D.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using FEALiTE2D.Helper;

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
            int nDOF = structure.nDOF;

            // check if there are free dofs available.
            if (structure.nDOF == 0)
                throw new InvalidOperationException("There are no sufficient degrees of freedom, thus there are no displacements or rotations could occur!");

            // global stiffness matrix.
            DenseMatrix Kg = new DenseMatrix(nDOF, nDOF);

            // start assembling
            foreach (var elem in structure.Elements)
            {
                //TODO: support other types.

                DenseMatrix elem_kg = elem.GlobalStiffnessMatrix;
                List<int> elemCoord = elem.DegreeOfFreedoms; // element's coordinates vector.

                int dofElem = elem.DOF; //number of dof for the elements.
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

            int nDOF = structure.nDOF;

            // global fem vector.
            double[] Qf = new double[nDOF];


            foreach (Node2D node in structure.Nodes)
            {
                double[] nodeLoad = new double[3];

                // get node loads.
                //1- add external nodal loads
                foreach (NodalLoad load in node.NodalLoads)
                {
                    if (load.LoadCase == loadCase)
                    {
                        double[] f = load.GetGlobalFixedEndForces(node);

                        nodeLoad[0] += f[0];
                        nodeLoad[1] += f[1];
                        nodeLoad[2] += f[2];
                    }
                }

                //2- get loads from connected elements to this node.
                IEnumerable<IElement> connectedElements = structure.Elements.Where(o => o.Nodes.Contains(node));

                if (connectedElements.Count() == 0)
                    throw new InvalidOperationException("Isolated node @" + node.Label);

                // get connected elements at this node.
                foreach (IElement elem in connectedElements)
                {
                    //TODO: support other types.
                    if (elem is FrameElement2D)
                    {
                        double[] fem = elem.GlobalEndForcesForLoadCase[loadCase];

                        for (int i = 0; i < 3; i++)
                        {
                            // add and reverse the sign of the force and moment then subtract P(nodal)  -(P(element)) will lead to positive sign
                            if (elem.Nodes[0] == node)
                                nodeLoad[i] -= fem[i];

                            if (elem.Nodes[1] == node)
                                nodeLoad[i] -= fem[i + 3];
                        }
                    }
                }


                for (int i = 0; i < 3; i++)
                    if (node.CoordNumbers[i] < structure.nDOF)
                        Qf[node.CoordNumbers[i]] += nodeLoad[i];


                //3- get node loads due to support displacement for restraint nodes only.
                if (node.DOF == 3) // not restrained
                    continue;

                foreach (SupportDisplacementLoad dis in node.SupportDisplacementLoad)
                {
                    if (dis.LoadCase != loadCase)
                        continue;

                    double[] dg = dis.GetGlobalFixedEndDisplacement(node);
                    foreach (IElement elem in connectedElements)
                    {
                        double[] d = new double[6];

                        if (elem is FrameElement2D)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (elem.Nodes[0] == node)
                                    d[i] = dg[i];

                                if (elem.Nodes[1] == node)
                                    d[i + 3] = dg[i];
                            }

                            // fixed end vector due to support displacement.
                            double[] fg = new double[6];
                            elem.GlobalStiffnessMatrix.Multiply(d, fg);

                            for (int i = 0; i < 3; i++)
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