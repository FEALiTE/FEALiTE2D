using FEALiTE2D.Elements;
using FEALiTE2D.Loads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEALiTE2D.Structure
{
    /// <summary>
    /// This class represents a post-processor for <see cref="Structure"/> class to retrieve elements data after the structure is solved. 
    /// </summary>
    public class PostProcessor
    {
        private readonly Structure structure;

        /// <summary>
        /// Creates a new instance of <see cref="PostProcessor"/> class.
        /// </summary>
        /// <param name="structure">a structure to extract data from.</param>
        public PostProcessor(Structure structure)
        {
            if (structure == null)
            {
                throw new ArgumentNullException("Structure can't be null.");
            }
            if (structure.AnalysisStatus == AnalysisStatus.Failure)
            {
                throw new ArgumentException("Structure should be successfully run.");
            }
            this.structure = structure;
        }

        /// <summary>
        /// Get support reaction for a load case.
        /// </summary>
        /// <param name="node">node which is restrained</param>
        /// <param name="loadCase">a load case</param>
        /// <returns>support reaction</returns>
        public Force GetSupportReaction(Node2D node, LoadCase loadCase)
        {
            Force R = new Force();

            if (node.IsFree == true)
            {
                return R; // 0 reactions.
            }

            //add external loads on nodes
            foreach (NodalLoad load in node.NodalLoads)
            {
                R.Fx -= load.Fx;
                R.Fy -= load.Fy;
                R.Mz -= load.Mz;
            }

            // get connected elements to this node
            IEnumerable<IElement> connectedElements = structure.Elements.Where(o => o.Nodes.Contains(node));

            // reactions are the sum of global external fixed end forces
            foreach (var e in connectedElements)
            {
                double[] fext = this.GetElementGlobalFixedEndForeces(e, loadCase);

                if (e.Nodes[0] == node)
                {
                    R.Fx += fext[0];
                    R.Fy += fext[1];
                    R.Mz += fext[2];
                }
                else
                {
                    R.Fx += fext[3];
                    R.Fy += fext[4];
                    R.Mz += fext[5];
                }
            }

            /*
            // calculate reaction due to support displacement
            foreach (SupportDisplacementLoad dis in node.SupportDisplacementLoad)
            {
                if (dis.LoadCase == loadCase)
                {
                    double[] dng = dis.GetGlobalFixedEndDisplacement(node);
                    foreach (IElement elem in connectedElements)
                    {
                        double[] dg = new double[6];

                        if (elem is FrameElement2D)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (elem.Nodes[0] == node)
                                    dg[i] = dng[i];

                                if (elem.Nodes[1] == node)
                                    dg[i + 3] = dng[i];
                            }

                            // fixed end vector due to support displacement.
                            double[] dl = new double[dg.Length];
                            elem.TransformationMatrix.TransposeMultiply(dg, dl);
                            double[] fl = new double[dg.Length];
                            elem.LocalStiffnessMatrix.Multiply(dl, fl);
                            double[] fg = new double[6];
                            elem.TransformationMatrix.TransposeMultiply(fl, fg);

                            if (elem.Nodes[0] == node)
                            {
                                R.Fx += fg[0];
                                R.Fy += fg[1];
                                R.Mz += fg[2];
                            }
                            else
                            {
                                R.Fx += fg[3];
                                R.Fy += fg[4];
                                R.Mz += fg[5];
                            }
                        }
                    }
                }
            }
            */
            return R;
        }

        /// <summary>
        /// Get Node's global displacement due to applied load in a load case.
        /// </summary>
        /// <param name="loadcase">load case</param>
        /// <returns>Nodal Displacement</returns>
        public Displacement GetNodeGlobalDisplacement(Node2D node, LoadCase loadCase)
        {
            Displacement nd = new Displacement();

            if (structure.AnalysisStatus == AnalysisStatus.Successful)
            {
                if (node.IsFree == false)
                {
                    foreach (var load in node.SupportDisplacementLoad)
                    {
                        if (load.LoadCase == loadCase)
                        {
                            nd.Ux += load.Ux;
                            nd.Uy += load.Uy;
                            nd.Rz += load.Rz;
                        }
                    }
                }
                else
                {
                    // get displacement from displacement vector if this node is free
                    double[] dVector = structure.DisplacementVectors[loadCase];

                    if (node.CoordNumbers[0] < dVector.Length)
                        nd.Ux = dVector[node.CoordNumbers[0]];

                    if (node.CoordNumbers[1] < dVector.Length)
                        nd.Uy = dVector[node.CoordNumbers[1]];

                    if (node.CoordNumbers[2] < dVector.Length)
                        nd.Rz = dVector[node.CoordNumbers[2]];
                }
            }
            return nd;
        }

        /// <summary>
        /// Get Local End forces of an element after the structure is solved.
        /// </summary>
        /// <param name="element">an element</param>
        /// <param name="loadCase">a load case</param>
        /// <returns>Local End forces of an element after the structure is solved.</returns>
        public double[] GetElementLocalFixedEndForeces(IElement element, LoadCase loadCase)
        {
            double[] q = new double[6];

            // Q = k*u+qf
            Displacement d1g = this.GetNodeGlobalDisplacement(element.Nodes[0], loadCase);
            Displacement d2g = this.GetNodeGlobalDisplacement(element.Nodes[1], loadCase);
            double[] dg = new double[] { d1g.Ux, d1g.Uy, d1g.Rz, d2g.Ux, d2g.Uy, d2g.Rz };
            double[] dl = new double[dg.Length];

            // dl = T*dg
            element.TransformationMatrix.Multiply(dg, dl);
            //ql = kl*dl
            element.LocalStiffnessMatrix.Multiply(dl, q);

            // get external fixed end loads from element load dictionary.
            double[] qextg = element.GlobalEndForcesForLoadCase[loadCase];
            double[] qextl = new double[qextg.Length];
            element.TransformationMatrix.Multiply(qextg, qextl);

            for (int i = 0; i < qextl.Length; i++)
            {
                q[i] += qextl[i];
            }

            return q;
        }

        /// <summary>
        /// Get Global End forces of an element after the structure is solved.
        /// </summary>
        /// <param name="element">an element</param>
        /// <param name="loadCase">a load case</param>
        /// <returns>Global End forces of an element after the structure is solved.</returns>
        public double[] GetElementGlobalFixedEndForeces(IElement element, LoadCase loadCase)
        {
            double[] ql = this.GetElementLocalFixedEndForeces(element, loadCase);

            // fg = Tt*fl
            double[] fg = new double[ql.Length];
            element.TransformationMatrix.TransposeMultiply(ql, fg);
            return fg;
        }
    }
}
