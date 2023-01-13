using MathNet.Numerics.Integration;

namespace FEALiTE2D.Loads;

/// <summary>
/// Represents a uniform loads on frame elements.
/// </summary>
[System.Serializable]
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
        Wx = wx;
        Wy = wy;
        LoadDirection = direction;
        LoadCase = loadCase;
        L1 = l1;
        L2 = l2;
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
    /// the load case.
    /// </summary>
    public LoadCase LoadCase { get; set; }

    /// <inheritdoc/>
    public double[] GetGlobalFixedEndForces(Elements.FrameElement2D element)
    {
        var fem = new double[6];
        double wx = Wx, wy = Wy, l = element.Length;

        // transform forces and moments from global to local.
        if (LoadDirection == LoadDirection.Global)
        {
            var f = new[] { Wx, Wy, 0 };

            var q = new double[3];
            element.LocalCoordinateSystemMatrix.Multiply(f, q);

            // assign the transformed values to the main new forces values.
            wx = q[0]; wy = q[1];
        }

        // 0 |Qx start|
        // 1 |Qy start|
        // 2 |Mz start|
        // 3 |Qx end  |
        // 4 |Qy end  |
        // 5 |Mz end  |

        // integrate using 5 points
        var nn = 5;
        var rule = new GaussLegendreRule(L1, l - L2, nn);
        var weights = rule.Weights;
        var xi = rule.Abscissas;

        for (var i = 0; i < nn; i++)
        {
            var n = element.GetShapeFunctionAt(xi[i]);
            fem[0] += n[0, 0] * wx * weights[i];
            fem[1] += n[1, 1] * wy * weights[i];
            fem[2] += n[1, 2] * wy * weights[i];
            fem[3] += n[0, 3] * wx * weights[i];
            fem[4] += n[1, 4] * wy * weights[i];
            fem[5] += n[1, 5] * wy * weights[i];
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
        var load = new FrameUniformLoad { LoadCase = LoadCase };
        if (LoadDirection == LoadDirection.Global)
        {
            var f = new[] { Wx, Wy, 0 };

            var q = new double[3];
            element.LocalCoordinateSystemMatrix.Multiply(f, q);

            // assign the transformed values to the main new forces values.
            load.Wx = q[0];
            load.Wy = q[1];
            load.L1 = load.L2 = x;
            load.LoadDirection = LoadDirection.Local;
        }
        else
        {
            load.Wx = Wx;
            load.Wy = Wy;
            load.L1 = load.L2 = x;
        }

        return load;
    }
}