using System.Diagnostics.CodeAnalysis;

namespace FEALiTE2D.Loads;

/// <summary>
/// Represents a class for <see cref="FramePointLoad"/>.
/// </summary>
[System.Serializable]
[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
public class FramePointLoad : ILoad
{
    /// <summary>
    /// Creates a new instance of <see cref="FramePointLoad"/> class.
    /// </summary>
    public FramePointLoad()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="FramePointLoad"/> class.
    /// </summary>
    /// <param name="fx">force parallel to X direction.</param>
    /// <param name="fy">force parallel to Y direction.</param>
    /// <param name="mz">Moment parallel to Z direction.</param>
    /// <param name="l1">distance from <see cref="FEALiTE2D.Elements. FrameElement2D.StartNode"/>.</param>
    /// <param name="direction">load direction.</param>
    /// <param name="loadCase">load case.</param>
    public FramePointLoad(double fx, double fy, double mz, double l1, LoadDirection direction, LoadCase loadCase)
    {
        Fx = fx;
        Fy = fy;
        Mz = mz;
        L1 = l1;
        LoadCase = loadCase;
        LoadDirection = direction;
    }

    /// <summary>
    /// Force in X-Direction.
    /// </summary>
    public double Fx { get; set; }

    /// <summary>
    /// Force in Y-Direction.
    /// </summary>
    public double Fy { get; set; }

    /// <summary>
    /// Moment in Z-Direction.
    /// </summary>
    public double Mz { get; set; }

    /// <summary>
    /// Distance of the <see cref="FramePointLoad"/> from the start node.
    /// </summary>
    public double L1 { get; set; }

    /// <inheritdoc/>
    public LoadDirection LoadDirection { get; set; }

    /// <inheritdoc/>
    public LoadCase LoadCase { get; set; }

    /// <inheritdoc/>
    public double[] GetGlobalFixedEndForces(Elements.FrameElement2D element)
    {
        var fem = new double[6];
        double fx = Fx,
            fy = Fy,
            mz = Mz,
            l = element.Length;

        // transform forces and moments from global to local.
        if (LoadDirection == LoadDirection.Global)
        {
            var f = new[] { Fx, Fy, Mz };

            var q = new double[3];
            element.LocalCoordinateSystemMatrix.Multiply(f, q);

            // assign the transformed values to the main new forces values.
            fx = q[0]; fy = q[1]; mz = q[2];
        }

        // 0 |Qx start|
        // 1 |Qy start|
        // 2 |Mz start|
        // 3 |Qx end  |
        // 4 |Qy end  |
        // 5 |Mz end  |

        double a = L1 / l, l2 = l - L1, b = (l - L1) / l;


        // Qx start
        fem[0] = fx * b;

        // Qy start 
        fem[1] = fy * b * b * (3 * a + b) // due to forces
                 - 6.0 * mz * a * b / l; // due to moments

        // Mz start
        fem[2] = fy * L1 * b * b // due to forces
                 + mz * b * (b - 2.0 * a); // due to moments

        // Qx end
        fem[3] = fx * a;

        // Qy end   
        fem[4] = fy * a * a * (a + 3 * b) // due to forces
                 + 6.0 * mz * a * b / l; // due to moments

        // Mz end
        fem[5] = -fy * a * a * l2 // due to forces
                 + mz * a * (a - 2.0 * b); // due to moments

        // back to global
        var globalFixedEndForces = new double[6];
        element.TransformationMatrix.TransposeMultiply(fem, globalFixedEndForces);
        return globalFixedEndForces;
    }

    /// <inheritdoc/>
    public ILoad GetLoadValueAt(Elements.IElement element, double x)
    {
        if (x != L1) return null;
        var load = new FramePointLoad { LoadCase = LoadCase };
        if (LoadDirection == LoadDirection.Global)
        {
            var f = new[] { Fx, Fy, 0 };

            var q = new double[3];
            element.LocalCoordinateSystemMatrix.Multiply(f, q);

            // assign the transformed values to the main new forces values.
            load.Fx = q[0];
            load.Fy = q[1];
            load.Mz = Mz;
            load.LoadDirection = LoadDirection.Local;
        }
        else
        {
            load.Fx = Fx;
            load.Fy = Fy;
            load.Mz = Mz;
        }

        return load;
    }
}