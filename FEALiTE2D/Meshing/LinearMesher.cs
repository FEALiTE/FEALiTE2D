using FEALiTE2D.Elements;
using FEALiTE2D.Loads;
using System;
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
        public void SetupDiscreteLocations(IElement element)
        {
            element.DiscreteLocations = new SortedSet<double>();
            double len = element.Length;

            // 1- add locations base on load location.

            // loop through each load 
            foreach (ILoad load in element.Loads)
            {
                if (load is FramePointLoad)
                {
                    var _load = load as FramePointLoad;
                    element.DiscreteLocations.Add(_load.L1);
                }
                else if (load is FrameUniformLoad)
                {
                    var _load = load as FrameUniformLoad;
                    element.DiscreteLocations.Add(_load.L1); // start location of the load
                    element.DiscreteLocations.Add(len - _load.L2); // end location of the load
                    element.DiscreteLocations.Add(0.5 * (len - (_load.L2 - _load.L1))); // mid location of the load
                }
                else if (load is FrameTrapezoidalLoad)
                {
                    var _load = load as FrameTrapezoidalLoad;
                    element.DiscreteLocations.Add(_load.L1); // start location of the load
                    element.DiscreteLocations.Add(len - _load.L2); // end location of the load
                    element.DiscreteLocations.Add(0.5 * (len - (_load.L2 - _load.L1))); // mid location of the load
                }
            }

            // 2- add locations based on number of segments and length of the segment.
            int n1 = (int)Math.Floor(len / this.MinDistance);
            int n2 = this.NumberSegements;
            int n = Math.Max(n1, n2);
            double dx = len / n;

            for (int i = 0; i < n; i++)
            {
                element.DiscreteLocations.Add(i * dx);
            }
        }
    }
}
