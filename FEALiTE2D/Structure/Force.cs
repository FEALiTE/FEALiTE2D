using System;
using System.Diagnostics.CodeAnalysis;

namespace FEALiTE2D.Structure;

/// <summary>
/// Defines a force of 3 components (Fx, Fy, Mz)
/// </summary>
[Serializable]
public class Force
{
    /// <summary>
    /// Gets or sets the fx.
    /// </summary>
    public double Fx { get; set; }

    /// <summary>
    /// Gets or sets the fy.
    /// </summary>
    public double Fy { get; set; }

    /// <summary>
    /// Gets or sets the mz.
    /// </summary>
    public double Mz { get; set; }

    /// <summary>
    /// Implements the operator + on 2 forces.
    /// </summary>
    /// <param name="f1">first force.</param>
    /// <param name="f2">second force.</param>
    public static Force operator +(Force f1, Force f2) => new() { Fx = f1.Fx + f2.Fx, Fy = f1.Fy + f2.Fy, Mz = f1.Mz + f2.Mz };

    /// <summary>
    /// Implements the operator - on 2 forces.
    /// </summary>
    /// <param name="f1">first force.</param>
    /// <param name="f2">second force.</param>
    public static Force operator -(Force f1, Force f2) => new() { Fx = f1.Fx - f2.Fx, Fy = f1.Fy - f2.Fy, Mz = f1.Mz - f2.Mz };

    /// <summary>
    /// Implements the operator number*force.
    /// </summary>
    /// <param name="factor">factor.</param>
    /// <param name="f">force.</param>
    public static Force operator *(double factor, Force f) => new() { Fx = factor * f.Fx, Fy = factor * f.Fy, Mz = factor * f.Mz };

    /// <summary>
    /// Convert To vector.
    /// </summary>
    public double[] ToVector() => new[] { Fx, Fy, Mz };

    /// <summary>
    /// Create force from a given vector.
    /// </summary>
    /// <param name="f">a Vector containing force</param>
    public static Force FromVector(double[] f)
    {
        return f.Length == 3 ? new Force() { Fx = f[0], Fy = f[1], Mz = f[2] } : null;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Fx = {Fx} {Environment.NewLine}Fy = {Fy} {Environment.NewLine}Mz = {Mz} {Environment.NewLine}";

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is not Force force) { return false; }

        return !(Math.Abs(force.Fx - Fx) > 1e-8) &&
               !(Math.Abs(force.Fy - Fy) > 1e-8) &&
               !(Math.Abs(force.Mz - Mz) > 1e-8);
    }

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode() => $"Fx = {Math.Round(Fx, 8)} {Environment.NewLine}".GetHashCode() + $"Fy = {Math.Round(Fy, 8)} {Environment.NewLine}".GetHashCode() + $"Mz = {Math.Round(Mz, 8)} {Environment.NewLine}".GetHashCode();
}