﻿using FEALiTE2D.Elements;
using FEALiTE2D.Helper;
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

            // check if this node have an elastic support
            if (node.Support is NodalSpringSupport)
            {
                // F= K * D
                // get node displacement
                Displacement d = this.GetNodeGlobalDisplacement(node, loadCase);
                double[] f = new double[3];
                ((NodalSpringSupport)node.Support).GlobalStiffnessMatrix.Multiply(d.ToVector(), f);

                R.Fx -= f[0];
                R.Fy -= f[1];
                R.Mz -= f[2];

                return R;
            }

            //add external loads on nodes
            foreach (NodalLoad load in node.NodalLoads.Where(ii => ii.LoadCase == loadCase))
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

            // get rid of redundant values which are very small such as 1e-15
            if (node.IsRestrained(NodalDegreeOfFreedom.UX) != true) R.Fx = 0;
            if (node.IsRestrained(NodalDegreeOfFreedom.UY) != true) R.Fy = 0;
            if (node.IsRestrained(NodalDegreeOfFreedom.RZ) != true) R.Mz = 0;

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
                foreach (var load in node.SupportDisplacementLoad)
                {
                    if (load.LoadCase == loadCase)
                    {
                        nd.Ux += load.Ux;
                        nd.Uy += load.Uy;
                        nd.Rz += load.Rz;
                    }
                }

                // get displacement from displacement vector if this node is free
                double[] dVector = structure.DisplacementVectors[loadCase];

                if (node.CoordNumbers[0] < dVector.Length)
                    nd.Ux = dVector[node.CoordNumbers[0]];

                if (node.CoordNumbers[1] < dVector.Length)
                    nd.Uy = dVector[node.CoordNumbers[1]];

                if (node.CoordNumbers[2] < dVector.Length)
                    nd.Rz = dVector[node.CoordNumbers[2]];
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
            double[] dl = this.GetElementLocalEndDisplacement(element, loadCase);

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

        /// <summary>
        /// Get a local end displacement of an element at a given load case.
        /// </summary>
        /// <param name="element">an element to process</param>
        /// <param name="loadCase">a load case to display displacement</param>
        public double[] GetElementLocalEndDisplacement(IElement element, LoadCase loadCase)
        {
            // get displacement of fist node and second node of the element
            Displacement nd1 = this.GetNodeGlobalDisplacement(element.Nodes[0], loadCase);
            Displacement nd2 = this.GetNodeGlobalDisplacement(element.Nodes[1], loadCase);

            double[] dg = new double[] { nd1.Ux, nd1.Uy, nd1.Rz, nd2.Ux, nd2.Uy, nd2.Rz };
            double[] dl = new double[dg.Length];
            element.TransformationMatrix.Multiply(dg, dl); // dl = T*dg

            return dl;
        }

        /// <summary>
        /// Get set of segments containing displacements at a given load case.
        /// </summary>
        /// <param name="element">an element to process</param>
        /// <param name="loadCase">a load case to display displacement</param>
        public List<FEALiTE2D.Meshing.LinearMeshSegment> GetElementLocalDisplacement(IElement element, LoadCase loadCase)
        {
            // retrieve mesh segments that have loads and internal forces set
            // so we can calculate displacement components.
            List<FEALiTE2D.Meshing.LinearMeshSegment> meshSegments = this.GetElementInternalForces(element, loadCase);
            double[] dl = this.GetElementLocalEndDisplacement(element, loadCase);
            /*double[] fr = this.GetElementLocalFixedEndForeces(element, loadCase);
            double[] fl = element.GlobalEndForcesForLoadCase[loadCase];*/

            for (int i = 0; i < meshSegments.Count; i++)
            {
                FEALiTE2D.Meshing.LinearMeshSegment segment = meshSegments[i];
                double l = segment.x2 - segment.x1;





                //set first segment
                if (i == 0)
                {
                    segment.Displacement1.Ux = dl[0];
                    segment.Displacement1.Uy = dl[1];
                    segment.Displacement1.Rz = dl[2];  /*(1 / 3) * ((fl[2] - fr[2]) * l / (segment.E * segment.Iy) - (fl[5] - fr[5] * l * 0.5 * (segment.E * segment.Iy) + 3.0 * (dl[4] - dl[1]) / l));*/
                    //1/3*((m1z - fem1z)*L/(E*Iz) - (m2z - fem2z)*L/(2*E*Iz) + 3*(delta2y - delta1y)/L)
                }
                else // set other segment
                {
                    FEALiTE2D.Meshing.LinearMeshSegment Psegment = meshSegments[i - 1];
                    segment.Displacement1 = Psegment.GetDisplacementAt(Psegment.x2 - Psegment.x1);
                }

                //segment.EndDisplacement = segment.GetDisplacementAt(segment.x2 - segment.x1);
            }

            return meshSegments;
        }

        /// <summary>
        /// Get internal forces of an element. note that segments length and count are based on the <see cref="FEALiTE2D.Meshing.ILinearMesher"/>
        /// </summary>
        /// <param name="element">an element to get its internal forces</param>
        /// <param name="loadCase">a load case to get the internal forces in an element</param>
        /// <returns>List of segments containing internal forces of an element.</returns>
        public List<FEALiTE2D.Meshing.LinearMeshSegment> GetElementInternalForces(IElement element, LoadCase loadCase)
        {
            double len = element.Length;

            // get local end forces after the structure is solved
            double[] fl = this.GetElementLocalFixedEndForeces(element, loadCase);
            double[] dl = this.GetElementLocalEndDisplacement(element, loadCase);

            //loop through each segment
            for (int i = 0; i < element.MeshSegments.Count; i++)
            {
                var segment = element.MeshSegments[i];
                double x = segment.x1;

                // force 0 values for start and end uniform and Trap. loads.
                segment.wx1 = 0; segment.wx2 = 0;
                segment.wy1 = 0; segment.wy2 = 0;

                //assign local end displacements and rotations to start segment
                if (i == 0)
                {
                    double ls = segment.x2 - segment.x1;
                    segment.Displacement1 = new Displacement()
                    {
                        Ux = dl[0],
                        Uy = dl[1],
                        Rz = dl[2]
                    };
                }
                else
                {
                    // get a reference to previous segment
                    FEALiTE2D.Meshing.LinearMeshSegment pS = element.MeshSegments[i - 1];
                    segment.Displacement1.Ux = pS.AxialDisplacementAt(pS.x2 - pS.x1);
                    segment.Displacement1.Uy = pS.VerticalDisplacementAt(pS.x2 - pS.x1);
                    segment.Displacement1.Rz = pS.SlopeAngleAt(pS.x2 - pS.x1);
                }

                // assign local end forces at start of each segment 
                segment.Internalforces1.Fx = fl[0];
                segment.Internalforces1.Fy = fl[1];
                segment.Internalforces1.Mz = fl[2] - fl[1] * x;

                // add effect of point load, uniform and trap. load to start of each segment if this load is before the segment
                IEnumerable<ILoad> loads = element.Loads.Where(o => o.LoadCase == loadCase);
                foreach (ILoad load in loads)
                {
                    if (load is FramePointLoad pL)
                    {
                        if (pL.L1 <= x)
                        {
                            // get point load in lcs.
                            var p = pL.GetLoadValueAt(element, pL.L1) as FramePointLoad;
                            segment.Internalforces1.Fx += p.Fx;
                            segment.Internalforces1.Fy += p.Fy;
                            segment.Internalforces1.Mz += p.Mz - p.Fy * (x - pL.L1);
                        }
                    }
                    else if (load is FrameUniformLoad uL)
                    {
                        // check the load is before or just before the segment
                        if (uL.L1 <= x)
                        {
                            // get uniform load in lcs.
                            FrameUniformLoad uniformLoad = uL.GetLoadValueAt(element, x) as FrameUniformLoad;
                            double wx = uniformLoad.Wx,
                                   wy = uniformLoad.Wy,
                                   x1 = uL.L1,
                                   x2 = len - uL.L2; // make x2 measured from start of the element

                            // check if the load stops at start  of the segment 
                            if (x2 > x)
                            {
                                segment.wx1 += wx;
                                segment.wx2 += wx;
                                segment.wy1 += wy;
                                segment.wy2 += wy;
                                x2 = x;
                            }

                            segment.Internalforces1.Fx += wx * (x2 - x1);
                            segment.Internalforces1.Fy += wy * (x2 - x1);
                            segment.Internalforces1.Mz -= wy * (x2 - x1) * (x - 0.5 * x2 - 0.5 * x1);
                        }
                    }
                    else if (load is FrameTrapezoidalLoad tL)
                    {
                        // check the load is before or just before the segment
                        if (tL.L1 <= x)
                        {
                            // get trap load in lcs.
                            FrameTrapezoidalLoad tl1 = tL.GetLoadValueAt(element, tL.L1) as FrameTrapezoidalLoad;
                            FrameTrapezoidalLoad tl2 = tL.GetLoadValueAt(element, len - tL.L2) as FrameTrapezoidalLoad;
                            double wx1 = tl1.Wx1,
                                   wx2 = tl2.Wx1,
                                   wy1 = tl1.Wy1,
                                   wy2 = tl2.Wy1,
                                   x1 = tl1.L1,
                                   x2 = len - tL.L2; // make x2 of the load measured from start of the element

                            //check if the load stops at start  of the segment
                            if (x2 > x)
                            {
                                // add load to the segment.
                                segment.wx1 += ((wx2 - wx1) / (x2 - x1)) * (x - x1) + wx1;
                                segment.wx2 += ((wx2 - wx1) / (x2 - x1)) * (segment.x2 - x1) + wx1;
                                segment.wy1 += ((wy2 - wy1) / (x2 - x1)) * (x - x1) + wy1;
                                segment.wy2 += ((wy2 - wy1) / (x2 - x1)) * (segment.x2 - x1) + wy1;

                                //measure the load at start of the segment if it passes the start of the segment
                                wx2 = ((wx2 - wx1) / (x2 - x1)) * (x - x1) + wx1;
                                wy2 = ((wy2 - wy1) / (x2 - x1)) * (x - x1) + wy1;
                                x2 = x;
                            }

                            segment.Internalforces1.Fx += 0.5 * (wx1 + wx2) * (x2 - x1);
                            segment.Internalforces1.Fy += 0.5 * (wy1 + wy2) * (x2 - x1);
                            segment.Internalforces1.Mz += (2 * wy1 * x1 - 3 * wy1 * x + wy1 * x2 + wy2 * x1 - 3 * wy2 * x + 2 * wy2 * x2) * (x2 - x1) / 6;
                        }
                    }
                }

                // set internal forces at the end
                segment.Internalforces2 = segment.GetInternalForceAt(segment.x2 - segment.x1);
                segment.Displacement2 = segment.GetDisplacementAt(segment.x2 - segment.x1);
            }

            return element.MeshSegments;
        }

        /// <summary>
        /// Get internal forces of an element at a distance measured from start node.
        /// </summary>
        /// <param name="element">an element to get its internal forces</param>
        /// <param name="loadCase">a load case to get the internal forces in an element</param>
        /// <param name="x">distance measured from start node.</param>
        /// <returns>internal forces of an element at a distance.</returns>
        public Force GetElementInternalForcesAt(IElement element, LoadCase loadCase, double x)
        {
            double len = element.Length;
            // get a list of meshed segments of the element
            List<Meshing.LinearMeshSegment> segments = this.GetElementInternalForces(element, loadCase);

            // loop through the segments to find which segment this distance is bounded to.
            foreach (var segment in segments)
                if (x >= segment.x1 && x <= segment.x2)
                    return segment.GetInternalForceAt(x - segment.x1);

            return null;

        }

    }
}
