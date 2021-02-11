using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Helper
{
    /// <summary>
    /// some extension methods to facilitate the work
    /// </summary>
    public static class  ExtensionMethods
    {
        /// <summary>
        /// Scale a matrix by a factor
        /// </summary>
        /// <param name="matrix">the matrix to be scaled</param>
        /// <param name="alpha">the scale factor</param>
        public static void ScaleMatrix(this CSparse.Storage.CompressedColumnStorage<double> matrix, double alpha)
        {
            for (int i = 0; i < matrix.Values.Length; i++)
            {
                matrix.Values[i] *= alpha;
            }
        }
    }
}
