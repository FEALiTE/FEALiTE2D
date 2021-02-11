﻿using MathNet.Numerics.Integration;
using System;

namespace FEALiTE2D.Loads
{
    /// <summary>
    /// Represents a uniform liner loads on<see cref="FrameElement"/>.
    /// </summary>
    public class FrameUniformLoad : ILoad
    {
        /// <summary>
        /// Creates a new instance of <see cref="FrameUniformLoad"/> class.
        /// </summary>
        public FrameUniformLoad()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FrameUniformLoad"/> class.
        /// <para>The load is uniformly distributed along part of the span.</para>
        /// </summary>
        /// <param name="wx">load intensity in x direction.</param>
        /// <param name="wy">load intensity in y direction.</param>
        /// <param name="direction">load direction.</param>
        /// <param name="loadCase">load case.</param>
        /// <param name="l1">distance from start.</param>
        /// <param name="l2">distance from end.</param>
        public FrameUniformLoad(double wx, double wy, LoadDirection direction, LoadCase loadCase, double l1 = 0, double l2 = 0) : this()
        {
            this.Wx = wx;
            this.Wy = wy;
            this.LoadDirection = direction;
            this.LoadCase = loadCase;
            this.L1 = l1;
            this.L2 = l2;
        }

        /// <summary>
        /// Magnitude of the uniform load in X-direction.
        /// </summary>
        public double Wx { get; set; }

        /// <summary>
        /// Magnitude of the uniform load in Y-direction.
        /// </summary>
        public double Wy { get; set; }

        /// <summary>
        /// Distance from start.
        /// </summary>
        public double L1 { get; set; }

        /// <summary>
        /// Distance from the end.
        /// </summary>
        public double L2 { get; set; }

        /// <summary>
        /// The direction of the load.
        /// </summary>
        public LoadDirection LoadDirection { get; set; }

        /// <summary>
        /// Load type.
        /// </summary>
        public LoadType LoadType => LoadType.FrameLinerLoad;

        /// <summary>
        /// the load case.
        /// </summary>
        public LoadCase LoadCase { get; set; }

        /// <summary>
        /// return global fixed end forces of this load in a fast way when the beam segment is fully loaded along its span.
        /// </summary>
        /// <param name="element">frame element</param>
        public double[] GetGlobalFixedEndForcesFast(FEALiTE2D.Elements.FrameElement2D element)
        {
            double[] fem = new double[6];
            double wx = this.Wx,
                   wy = this.Wy,
                   l = element.Length;

            // transform forces and moments from global to local.
            if (LoadDirection == LoadDirection.Global)
            {
                double[] F = new double[] { this.Wx, this.Wy, 0 };

                double[] Q = new double[3];
                element.LocalCoordinateSystemMatrix.Multiply(F, Q);

                // assign the transformed values to the main new forces values.
                wx = Q[0]; wy = Q[1];
            }

            // 0 |Qx start|
            // 1 |Qy start|
            // 2 |Mz start|
            // 3 |Qx end  |
            // 4 |Qy end  |
            // 5 |Mz end  |

            // start
            fem[0] = wx * l / 2.0; // Qx                                     
            fem[1] = wy * l / 2.0; // Qz   
            fem[2] = wy * l * l / 12.0; // My 

            // end
            fem[3] = wx * l / 2.0; // Qx
            fem[4] = wy * l / 2.0;  // Qz     
            fem[5] = -wy * l * l / 12.0; // My

            var fansewr = new double[6];
            element.TransformationMatrix.TransposeMultiply(fem, fansewr);
            return fansewr;
        }


        public double[] GetGlobalFixedEndForces(FEALiTE2D.Elements.FrameElement2D element)
        {
            double[] fem = new double[6];
            double wx = this.Wx,
                   wy = this.Wy,
                   l = element.Length;

            // transform forces and moments from global to local.
            if (LoadDirection == LoadDirection.Global)
            {
                double[] F = new double[] { this.Wx, this.Wy, 0 };

                double[] Q = new double[3];
                element.LocalCoordinateSystemMatrix.Multiply(F, Q);

                // assign the transformed values to the main new forces values.
                wx = Q[0]; wy = Q[1];
            }

            // 0 |Qx start|
            // 1 |Qy start|
            // 2 |Mz start|
            // 3 |Qx end  |
            // 4 |Qy end  |
            // 5 |Mz end  |

            // integrate using 5 points
            int nn = 5;
            GaussLegendreRule rule = new GaussLegendreRule(L1, l - L2, nn);
            var weights = rule.Weights;
            var xi = rule.Abscissas;

            for (int i = 0; i < nn; i++)
            {
                var n = element.GetShapeFunctionNuAt1(xi[i]);
                fem[0] += n[0] * wx * weights[i];
                fem[1] += n[1] * wy * weights[i];
                fem[2] += n[2] * wy * weights[i];
                fem[3] += n[3] * wx * weights[i];
                fem[4] += n[4] * wy * weights[i];
                fem[5] += n[5] * wy * weights[i];
            }

            var fansewr = new double[6];
            element.TransformationMatrix.TransposeMultiply(fem, fansewr);
            return fansewr;
        }
    }
}
