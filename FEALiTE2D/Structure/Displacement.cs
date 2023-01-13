using System;
using System.Diagnostics.CodeAnalysis;

namespace FEALiTE2D.Structure;

/// <summary>
/// Defines a displacement of 3 components (Ux, Uy, Rz)
/// </summary>
[Serializable]
public class Displacement
{
    /// <summary>
    /// Gets or sets the ux.
    /// </summary>
    public double Ux { get; set; }

    /// <summary>
    /// Gets or sets the uy.
    /// </summary>
    public double Uy { get; set; }

    /// <summary>
    /// Gets or sets the rz.
    /// </summary>
    public double Rz { get; set; }

    /// <summary>
    /// Implements the operator + on 2 Displacements.
    /// </summary>
    /// <param name="d1">first Displacement.</param>
    /// <param name="d2">second Displacement.</param>
    public static Displacement operator +(Displacement d1, Displacement d2) => new() { Ux = d1.Ux + d2.Ux, Uy = d1.Uy + d2.Uy, Rz = d1.Rz + d2.Rz };

    /// <summary>
    /// Implements the operator - on 2 Displacements.
    /// </summary>
    /// <param name="d1">first displacement.</param>
    /// <param name="d2">second displacement.</param>
    public static Displacement operator -(Displacement d1, Displacement d2) => new() { Ux = d1.Ux - d2.Ux, Uy = d1.Uy - d2.Uy, Rz = d1.Rz - d2.Rz };

    /// <summary>
    /// Implements the operator number*Displacement.
    /// </summary>
    /// <param name="factor">factor.</param>
    /// <param name="d">Displacement.</param>
    public static Displacement operator *(double factor, Displacement d) => new() { Ux = factor * d.Ux, Uy = factor * d.Uy, Rz = factor * d.Rz };

    /// <summary>
    /// Convert To vector.
    /// </summary>
    public double[] ToVector()
    {
        return new[] { Ux, Uy, Rz };
    }

    /// <summary>
    /// Create Displacement from a given vector.
    /// </summary>
    /// <param name="dis">a Vector containing displacement</param>
    public static Displacement FromVector(double[] dis)
    {
        if (dis.Length == 3)
        {
            return new Displacement() { Ux = dis[0], Uy = dis[1], Rz = dis[2] };
        }
        return null;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return
            $"Ux = {Ux} {Environment.NewLine}" +
            $"Uy = {Uy} {Environment.NewLine}" +
            $"Rz = {Rz} {Environment.NewLine}";
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is not Displacement d) { return false; }

        return !(Math.Abs(d.Ux - Ux) > 1e-8) &&
               !(Math.Abs(d.Uy - Uy) > 1e-8) &&
               !(Math.Abs(d.Rz - Rz) > 1e-8);
    }

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() => $"Ux = {Math.Round(Ux, 8)} {Environment.NewLine}".GetHashCode() + $"Uy = {Math.Round(Uy, 8)} {Environment.NewLine}".GetHashCode() + $"Rz = {Math.Round(Rz, 8)} {Environment.NewLine}".GetHashCode();
}