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
    [System.Serializable]
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
            if (element.GetType() != typeof(SpringElement2D))
            {
                int n1 = (int)Math.Floor(len / this.MinDistance);
                int n2 = this.NumberSegements;
                int n = Math.Max(n1, n2);
                double dx = len / n;

                for (int i = 0; i < n; i++)
                {
                    discreteLocations.Add(i * dx);
                }
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
                // set geometric properties.
                if (!(element is SpringElement2D))
                {
                    segment.E = element.CrossSection.Material.E;
                    segment.Ix = element.CrossSection.Ix;
                    segment.A = element.CrossSection.A;
                }

                element.MeshSegments.Add(segment);
            }
        }
    }
}
