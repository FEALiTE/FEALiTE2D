using FEALiTE2D.Helper;
using MathNet.Numerics.Integration;
using System;

namespace FEALiTE2D.Loads;

/// <summary>
/// Represents a trapezoidal loads on frame elements.
/// </summary>
[Serializable]
public class FrameTrapezoidalLoad : ILoad
{
    /// <summary>
    /// Creates a new instance of <see cref="FrameTrapezoidalLoad"/> class.
    /// <para>The load is trapezoidally distributed along part of the span.</para>
    /// </summary>
    public FrameTrapezoidalLoad() { }

    /// <summary>
    /// Creates a new instance of <see cref="FrameTrapezoidalLoad"/> class.
    /// <para>The load is trapezoidally distributed along part of the span.</para>
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

        Wx1 = wx1; Wx2 = wx2;
        Wy1 = wy1; Wy2 = wy2;
        LoadDirection = direction;
        LoadCase = loadCase;
        L1 = l1;
        L2 = l2;
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
    /// Distance from start node.
    /// </summary>
    public double L1 { get; set; }

    /// <summary>
    /// Distance from the end node.
    /// </summary>
    public double L2 { get; set; }

    /// <summary>
    /// The direction of the load.
    /// </summary>
    public LoadDirection LoadDirection { get; set; }

    /// <summary>
    /// the load case.
    /// </summary>
    public LoadCase LoadCase { get; set; }

    /// <inheritdoc/>
    public double[] GetGlobalFixedEndForces(Elements.FrameElement2D element)
    {
        double wx1 = Wx1,
            wy1 = Wy1,
            wx2 = Wx2,
            wy2 = Wy2,
            l = element.Length;

        if (LoadDirection == LoadDirection.Global)
        {
            // transform forces and moments from global to local.

            var f1 = new[] { Wx1, Wy1, 0 };
            var f2 = new[] { Wx2, Wy2, 0 };

            var q1 = new double[3];
            var q2 = new double[3];

            element.LocalCoordinateSystemMatrix.Multiply(f1, q1);
            element.LocalCoordinateSystemMatrix.Multiply(f2, q2);

            // assign the transformed values to the main new forces values.
            wx1 = q1[0]; wy1 = q1[1];
            wx2 = q2[0]; wy2 = q2[1];
        }

        // Trapezoidal Load is a linear equation of first degree
        var wx = new LinearFunction(L1, l - L2, wx1, wx2);
        var wy = new LinearFunction(L1, l - L2, wy1, wy2);

        // integrate using 7 points
        var nn = 7;
        var rule = new GaussLegendreRule(L1, l - L2, nn);
        var weights = rule.Weights;
        var xi = rule.Abscissas;

        var fem = new double[6];
        for (var i = 0; i < nn; i++)
        {
            var n = element.GetShapeFunctionAt(xi[i]);
            var nx = wx.GetValueAt(xi[i]);
            var ny = wy.GetValueAt(xi[i]);
            fem[0] += n[0,0] * nx * weights[i];
            fem[1] += n[1,1] * ny * weights[i];
            fem[2] += n[1,2] * ny * weights[i];
            fem[3] += n[0,3] * nx * weights[i];
            fem[4] += n[1,4] * ny * weights[i];
            fem[5] += n[1,5] * ny * weights[i];
        }

        var globalFixedEndForces = new double[6];
        element.TransformationMatrix.TransposeMultiply(fem, globalFixedEndForces);
        return globalFixedEndForces;
    }

    /// <inheritdoc/>
    public ILoad GetLoadValueAt(Elements.IElement element, double x)
    {
        var l = element.Length;
        if (!(x >= L1) || !(x <= l - L2)) return null;
        var load = new FrameTrapezoidalLoad { LoadCase = LoadCase };
        if (LoadDirection == LoadDirection.Global)
        {
            // transform forces and moments from global to local.

            var f1 = new[] { Wx1, Wy1, 0 };
            var f2 = new[] { Wx2, Wy2, 0 };

            var q1 = new double[3];
            var q2 = new double[3];

            element.LocalCoordinateSystemMatrix.Multiply(f1, q1);
            element.LocalCoordinateSystemMatrix.Multiply(f2, q2);

            // Trapezoidal Load is a linear equation of first degree
            var wxFunc = new LinearFunction(L1, l - L2, q1[0], q2[0]);
            var wyFunc = new LinearFunction(L1, l - L2, q1[1], q2[1]);

            load.Wx1 = load.Wx2 = wxFunc.GetValueAt(x);
            load.Wy1 = load.Wy2 = wyFunc.GetValueAt(x);
            load.L1 = load.L2 = x;
            load.LoadDirection = LoadDirection.Local;
        }
        else
        {
            var wxFunc = new LinearFunction(L1, l - L2, Wx1, Wx2);
            var wyFunc = new LinearFunction(L1, l - L2, Wy1, Wy2);
            load.Wx1 = load.Wx2 = wxFunc.GetValueAt(x);
            load.Wy1 = load.Wy2 = wyFunc.GetValueAt(x);
            load.L1 = load.L2 = x;
        }

        return load;
    }
}