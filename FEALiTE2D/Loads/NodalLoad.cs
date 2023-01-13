using System.Diagnostics.CodeAnalysis;
using FEALiTE2D.Elements;

namespace FEALiTE2D.Loads;

/// <summary>
/// Represent a class for Nodal loads in Global or local coordinates system.
/// </summary>
[System.Serializable]
public class NodalLoad
{
    /// <summary>
    /// Creates a new class of <see cref="NodalLoad"/>.
    /// </summary>
    public NodalLoad() { }

    /// <summary>
    /// Creates a new class of <see cref="NodalLoad"/>
    /// </summary>
    /// <param name="fx">force parallel to Global X direction.</param>
    /// <param name="fy">force parallel to Global Y direction.</param>
    /// <param name="mz">Moment parallel to Global Z direction.</param>
    /// <param name="direction">load direction.</param>
    /// <param name="loadCase">load case.</param>
    public NodalLoad(double fx, double fy, double mz, LoadDirection direction, LoadCase loadCase) : this()
    {
        Fx = fx;
        Fy = fy;
        Mz = mz;
        LoadDirection = direction;
        LoadCase = loadCase;
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

    /// <inheritdoc/>
    public LoadDirection LoadDirection { get; set; }

    /// <inheritdoc/>
    public LoadCase LoadCase { get; set; }

    /// <inheritdoc/>
    public double[] GetGlobalFixedEndForces(Node2D node)
    {
        // create force vector
        var q = new double[3] { Fx, Fy, Mz };
            
        // if the forces is in global coordinate system of the node then return it.
        if (LoadDirection == LoadDirection.Global)
        {
            return q;
        }
        // transform the load vector to the local coordinate of the node.
        var f = new double[3];
        node.TransformationMatrix.TransposeMultiply(q, f);
        return f;
    }

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }
        if (obj.GetType() != typeof(NodalLoad))
        {
            return false;
        }
        var nl = obj as NodalLoad;
        if (Fx != nl.Fx || Fy != nl.Fy || Mz != nl.Mz || LoadCase != nl.LoadCase)
        {
            return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public static bool operator ==(NodalLoad nl1, NodalLoad nl2)
    {
        if (nl1 is null)
        {
            return false;
        }
        return nl1.Equals(nl2);
    }

    /// <inheritdoc/>
    public static bool operator !=(NodalLoad nl1, NodalLoad nl2)
    {
        if (ReferenceEquals(nl1, null))
        {
            return false;
        }
        return !nl1.Equals(nl2);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var result = 0;
        result += (Fx + 1e-10).GetHashCode();
        result += (Fy + 2e-10).GetHashCode();
        result += (Mz + 6e-10).GetHashCode();
        result += LoadCase.GetHashCode();
        return result;
    }


}