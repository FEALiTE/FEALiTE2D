using FEALiTE2D.Helper;
using MathNet.Numerics.Integration;
using System;

namespace FEALiTE2D.Loads
{
    /// <summary>
    /// Represents a trapezoidal loads on frame elements.
    /// </summary>
    public class FrameTrapezoidalLoad : ILoad
    {
        /// <summary>
        /// Creates a new instance of <see cref="FrameTrapezoidalLoad"/> class.
        /// <para>The load is trapezoidally distributed along the span.</para>
        /// </summary>
        /// <param name="wx1">left load intensity in x direction.</param>
        /// <param name="wx2">right load intensity in x direction.</param>
        /// <param name="wy1">left load intensity in y direction.</param>
        /// <param name="wy2">right load intensity in y direction.</param>
        /// <param name="direction">load direction.</param>
        /// <param name="loadCase">load case.</param>
        /// <param name="l1">distance from start.</param>
        /// <param name="l2">distance from end.</param>
        public FrameTrapezoidalLoad(double wx1, double wx2, double wy1, double wy2, LoadDirection direction,
                                     LoadCase loadCase, double l1 = 0, double l2 = 0)
        {

            this.Wx1 = wx1; this.Wx2 = wx2;
            this.Wy1 = wy1; this.Wy2 = wy2;
            this.LoadDirection = direction;
            this.LoadCase = loadCase;
            this.L1 = l1;
            this.L2 = l2;
        }

        /// <summary>
        /// Magnitude of the uniform load in X-direction (left).
        /// </summary>
        public double Wx1 { get; set; }

        /// <summary>
        /// Magnitude of the uniform load in X-direction (right).
        /// </summary>
        public double Wx2 { get; set; }

        /// <summary>
        /// Magnitude of the uniform load in Y-direction (left).
        /// </summary>
        public double Wy1 { get; set; }

        /// <summary>
        /// Magnitude of the uniform load in Y-direction (right).
        /// </summary>
        public double Wy2 { get; set; }

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

        /// <inheritdoc/>
        public double[] GetGlobalFixedEndForces(FEALiTE2D.Elements.FrameElement2D element)
        {
            double wx1 = this.Wx1,
                   wy1 = this.Wy1,
                   wx2 = this.Wx2,
                   wy2 = this.Wy2,
                   l = element.Length;

            if (LoadDirection == LoadDirection.Global)
            {
                // transform forces and moments from global to local.

                double[] F1 = new[] { this.Wx1, this.Wy1, 0 };
                double[] F2 = new[] { this.Wx2, this.Wy2, 0 };

                double[] Q1 = new double[3];
                double[] Q2 = new double[3];

                element.LocalCoordinateSystemMatrix.Multiply(F1, Q1);
                element.LocalCoordinateSystemMatrix.Multiply(F2, Q2);

                // assign the transformed values to the main new forces values.
                wx1 = Q1[0]; wy1 = Q1[1];
                wx2 = Q2[0]; wy2 = Q2[1];
            }

            // Trapezoidal Load is an linear equation of first degree
            LinearFunction wx = new LinearFunction(L1, l - L2, wx1, wx2);
            LinearFunction wy = new LinearFunction(L1, l - L2, wy1, wy2);

            // integrate using 7 points
            int nn = 7;
            GaussLegendreRule rule = new GaussLegendreRule(L1, l - L2, nn);
            var weights = rule.Weights;
            var xi = rule.Abscissas;

            double[] fem = new double[6];
            for (int i = 0; i < nn; i++)
            {
                var n = element.GetShapeFunctionNuAt(xi[i]);
                var nx = wx.GetValueAt(xi[i]);
                var ny = wy.GetValueAt(xi[i]);
                fem[0] += n[0,0] * nx * weights[i];
                fem[1] += n[1,1] * ny * weights[i];
                fem[2] += n[1,2] * ny * weights[i];
                fem[3] += n[0,3] * nx * weights[i];
                fem[4] += n[1,4] * ny * weights[i];
                fem[5] += n[1,5] * ny * weights[i];
            }

            var fansewr = new double[6];
            element.TransformationMatrix.TransposeMultiply(fem, fansewr);
            return fansewr;
        }
    }
}
