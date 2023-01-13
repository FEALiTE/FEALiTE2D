using System.Diagnostics.CodeAnalysis;
using FEALiTE2D.Elements;

namespace FEALiTE2D.Loads;

/// <summary>
/// Represent a class for support displacement displacements in Global Coordinates system.
/// </summary>
[System.Serializable]
public class SupportDisplacementLoad
{
    /// <summary>
    /// Creates a new class of <see cref="SupportDisplacementLoad"/>.
    /// </summary>
    public SupportDisplacementLoad() { }

    /// <summary>
    /// Creates a new class of <see cref="SupportDisplacementLoad"/>
    /// </summary>
    /// <param name="ux">global displacement in X-Direction.</param>
    /// <param name="uy">global displacement in Y-Direction.</param>
    /// <param name="rz">global rotation about Z-Direction.</param>
    /// <param name="loadCase">load case.</param>
    public SupportDisplacementLoad(double ux, double uy, double rz, LoadCase loadCase)
        : this()
    {
        Ux = ux;
        Uy = uy;
        Rz = rz;
        LoadCase = loadCase;
    }

    /// <summary>
    /// Displacement in X-Direction.
    /// </summary>
    public double Ux { get; }

    /// <summary>
    /// Displacement in Y-Direction.
    /// </summary>
    public double Uy { get; }

    /// <summary>
    /// rotation about Z-Direction.
    /// </summary>
    public double Rz { get; }

    
    /// <summary>
    /// 
    /// </summary>
    public LoadDirection LoadDirection { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public LoadCase LoadCase { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public double[] GetGlobalFixedEndDisplacement(Node2D node)
    {
        // create force vector
        var q = new[] { Ux, Uy, Rz };

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
        if (ReferenceEquals(obj, null))
        {
            return false;
        }
        if (obj.GetType() != typeof(SupportDisplacementLoad))
        {
            return false;
        }
        var nd = (SupportDisplacementLoad)obj;
        return Ux == nd.Ux && Uy == nd.Uy && Rz == nd.Rz && LoadCase == nd.LoadCase;
    }

    
    /// <summary>
    /// Equality
    /// </summary>
    /// <param name="nl1"></param>
    /// <param name="nl2"></param>
    /// <returns></returns>
    public static bool operator ==(SupportDisplacementLoad nl1, SupportDisplacementLoad nl2)
    {
        if (ReferenceEquals(nl1, null))
        {
            return false;
        }
        return nl1.Equals(nl2);
    }

    /// <summary>
    /// Inequality
    /// </summary>
    /// <param name="nl1"></param>
    /// <param name="nl2"></param>
    /// <returns></returns>
    public static bool operator !=(SupportDisplacementLoad nl1, SupportDisplacementLoad nl2)
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
        result += (Ux + 1e-10).GetHashCode();
        result += (Uy + 2e-10).GetHashCode();
        result += (Rz + 6e-10).GetHashCode();
        result += LoadCase.GetHashCode();
        return result;
    }
}