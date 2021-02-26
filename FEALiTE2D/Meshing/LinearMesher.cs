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
                    var _load = load as FrameTrapezoidalLoad;
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

            discreteLocations.Add(len);

            // loop through each segment to find if there's load applied on it
            // for point loads, the load will be applied to start of the segment
            for (int i = 0; i < discreteLocations.Count - 1; i++)
            {
                // Create a new segment and apply loads on it.
                LinearMeshSegment segment = new LinearMeshSegment();
                segment.x1 = discreteLocations.ElementAt(i);
                segment.x2 = discreteLocations.ElementAt(i + 1);
                element.MeshSegments.Add(segment);

                foreach (ILoad load in element.Loads)
                {
                    if (load is FramePointLoad)
                    {
                        if (((FramePointLoad)load).L1 == segment.x1)
                        {
                            if (load.GetLoadValueAt(element, segment.x1) is FramePointLoad pointLoad)
                            {
                                segment.fx += pointLoad.Fx;
                                segment.fy += pointLoad.Fy;
                                segment.mz += pointLoad.Mz;
                            }
                        }
                    }
                    else if (load is FrameUniformLoad uniformLoad)
                    {
                        FrameUniformLoad uL1 = uniformLoad.GetLoadValueAt(element, segment.x1) as FrameUniformLoad;
                        FrameUniformLoad uL2 = uniformLoad.GetLoadValueAt(element, segment.x2) as FrameUniformLoad;

                        if (uL1 != null || uL2 != null)
                        {
                            segment.wx += uL1.Wx;
                            segment.wy += uL1.Wy;
                        }
                    }
                    else if (load is FrameTrapezoidalLoad trapezoidalLoad)
                    {
                        FrameTrapezoidalLoad tL1 = trapezoidalLoad.GetLoadValueAt(element, segment.x1) as FrameTrapezoidalLoad;
                        FrameTrapezoidalLoad tL2 = trapezoidalLoad.GetLoadValueAt(element, segment.x2) as FrameTrapezoidalLoad;

                        if (tL1 != null || tL2 != null)
                        {
                            segment.wx1 += tL1.Wx1;
                            segment.wx2 += tL1.Wx2;
                            segment.wy1 += tL1.Wy1;
                            segment.wy2 += tL1.Wy2;
                        }
                    }
                }

            }
        }
    }
}
