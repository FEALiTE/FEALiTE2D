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
            discreteLocations.UnionWith(element.AdditionalMeshPoints);
            double len = element.Length;

            // do some checks
            foreach (var distance in element.AdditionalMeshPoints)
            {
                if (CheckBounds(distance, len, 0))
                    discreteLocations.Add(distance);
                else
                    throw new ArgumentOutOfRangeException($"Additional mesh points contain a distance that is not within the bounds of the element {element.Label} !");
            }

            discreteLocations.Add(0);

            // 1- add locations base on load location.

            // loop through each load 
            foreach (ILoad load in element.Loads)
            {
                if (load is FramePointLoad load1)
                {
                    if (CheckBounds(load1.L1, len, 0))
                        discreteLocations.Add(load1.L1);
                    else
                        throw new ArgumentOutOfRangeException($"Frame point load on {element.Label} is not within the bounds of the element!");
                }
                else if (load is FrameUniformLoad load2)
                {
                    if (CheckBounds(load2.L1, len, 0) && CheckBounds(len - load2.L2, element.Length, 0))
                    {
                        discreteLocations.Add(load2.L1); // start location of the load
                        discreteLocations.Add(len - load2.L2); // end location of the load
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException($"Frame uniform load on {element.Label} is not within the bounds of the element!");
                    }
                }
                else if (load is FrameTrapezoidalLoad load3)
                {
                    if (CheckBounds(load3.L1, len, 0) && CheckBounds(len - load3.L2, element.Length, 0))
                    {
                        discreteLocations.Add(load3.L1); // start location of the load
                        discreteLocations.Add(len - load3.L2); // end location of the load
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException($"Frame trapezoidal load on {element.Label} is not within the bounds of the element!");
                    }
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

        /// <summary>
        /// Check a distance on an element is within its bounds.
        /// </summary>
        /// <param name="distance">a given distance</param>
        /// <param name="upperBound">upper bound of the element which is its length</param>
        /// <param name="lowerBound">lower bound of the element which is 0</param>
        /// <returns></returns>
        public bool CheckBounds(double distance, double upperBound, double lowerBound)
        {
            if (distance < lowerBound) return false;
            else if (distance > upperBound) return false;
            else return true;
        }
    }
}
