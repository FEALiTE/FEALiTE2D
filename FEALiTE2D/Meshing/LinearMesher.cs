using FEALiTE2D.Elements;
using FEALiTE2D.Loads;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FEALiTE2D.Meshing
{
    /// <summary>
    /// This class represents a mesher for 1D elements.
    /// </summary>
    public class LinearMesher : FEALiTE2D.Meshing.ILinearMesher
    {
        /// <summary>
        /// Creates a new instance of <see cref="LinearMesher"/> class.
        /// </summary>
        public LinearMesher()
        {
            this.NumberSegements = 5;
        }

        /// <summary>
        /// Creates a new instance of <see cref="LinearMesher"/> class.
        /// </summary>
        /// <param name="n">minimum number of segments that the <see cref="Elements.IElement"/> be discretized into.</param>
        /// <param name="d">minimum length of segments that the <see cref="Elements.IElement"/> be discretized into.</param>
        public LinearMesher(int n, double d)
        {
            this.MinDistance = d;
            this.NumberSegements = n;
        }

        /// <inheritdoc/>
        public int NumberSegements { get; set; }

        /// <inheritdoc/>
        public double MinDistance { get; set; }

        /// <inheritdoc/>
        public void SetupMeshSegments(IElement element)
        {
            var discreteLocations = new SortedSet<double>();
            double len = element.Length;
            discreteLocations.Add(0);

            // 1- add locations base on load location.

            // loop through each load 
            foreach (ILoad load in element.Loads)
            {
                if (load is FramePointLoad load1)
                {
                    discreteLocations.Add(load1.L1);
                }
                else if (load is FrameUniformLoad load2)
                {
                    discreteLocations.Add(load2.L1); // start location of the load
                    discreteLocations.Add(len - load2.L2); // end location of the load
                }
                else if (load is FrameTrapezoidalLoad load3)
                {
                    discreteLocations.Add(load3.L1); // start location of the load
                    discreteLocations.Add(len - load3.L2); // end location of the load
                }
            }

            // 2- add locations based on number of segments and length of the segment.
            int n1 = (int)Math.Floor(len / this.MinDistance);
            int n2 = this.NumberSegements;
            int n = Math.Max(n1, n2);
            double dx = len / n;

            for (int i = 0; i < n; i++)
            {
                discreteLocations.Add(i * dx);
            }

            // add last point
            discreteLocations.Add(len);
            // clear mesh segments first
            element.MeshSegments.Clear();

            // loop through each segment to find if there's load applied on it
            // for point loads, the load will be applied to start of the segment
            for (int i = 0; i < discreteLocations.Count - 1; i++)
            {
                // Create a new segment and apply loads on it.
                LinearMeshSegment segment = new LinearMeshSegment();
                segment.x1 = discreteLocations.ElementAt(i);
                segment.x2 = discreteLocations.ElementAt(i + 1);
                element.MeshSegments.Add(segment);

                // loop through each load to see if this load is applied on the segment
                foreach (ILoad load in element.Loads)
                {
                    if (load is FramePointLoad pL)
                    {
                        // point loads must be applied on start of the segment
                        if (pL.L1 == segment.x1)
                        {
                            if (pL.GetLoadValueAt(element, segment.x1) is FramePointLoad pointLoad)
                            {
                                segment.fx += pointLoad.Fx;
                                segment.fy += pointLoad.Fy;
                                segment.mz += pointLoad.Mz;
                            }
                        }
                    }
                    else if (load is FrameUniformLoad uniformLoad)
                    {
                        // apply uniform load if the load's start touches the start of the segment and the load's end touches the end of the segment
                        if (uniformLoad.L1 <= segment.x1 && len - uniformLoad.L2 >= segment.x2)
                        {
                            FrameUniformLoad uL = uniformLoad.GetLoadValueAt(element, segment.x1) as FrameUniformLoad;

                            if (uL != null)
                            {
                                segment.wx1 += uL.Wx;
                                segment.wx2 += uL.Wx;
                                segment.wy1 += uL.Wy;
                                segment.wy2 += uL.Wy;
                            }
                        }
                    }
                    else if (load is FrameTrapezoidalLoad trapezoidalLoad)
                    {
                        // apply trapezoidal load if the load's start touches the start of the segment and the load's end touches the end of the segment
                        if (trapezoidalLoad.L1 <= segment.x1 && len - trapezoidalLoad.L2 >= segment.x2)
                        {
                            FrameTrapezoidalLoad tL1 = trapezoidalLoad.GetLoadValueAt(element, segment.x1) as FrameTrapezoidalLoad;
                            FrameTrapezoidalLoad tL2 = trapezoidalLoad.GetLoadValueAt(element, segment.x2) as FrameTrapezoidalLoad;

                            if (tL1 != null && tL2 != null)
                            {
                                segment.wx1 += tL1.Wx1;
                                segment.wx2 += tL2.Wx1;
                                segment.wy1 += tL1.Wy1;
                                segment.wy2 += tL2.Wy1;
                            }
                        }
                    }
                }

            }
        }
    }
}
