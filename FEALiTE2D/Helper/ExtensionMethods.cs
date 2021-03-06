using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Helper
{
    /// <summary>
    /// some extension methods to facilitate the work
    /// </summary>
    public static class ExtensionMethods
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

        /// <summary>
        /// Scale a matrix by a factor
        /// </summary>
        /// <param name="matrix">the matrix to be scaled</param>
        /// <param name="alpha">the scale factor</param>
        public static void ScaleMatrix(this CSparse.Storage.DenseColumnMajorStorage<double> matrix, double alpha)
        {
            for (int i = 0; i < matrix.Values.Length; i++)
            {
                matrix.Values[i] *= alpha;
            }
        }

#if DEBUG

        internal static string PrintSparseMatrix2(this CSparse.Double.SparseMatrix sparseMatrix)
        {
            // calculate maximum width.
            int nColumns = sparseMatrix.ColumnCount;
            int nRows = sparseMatrix.RowCount;

            var widths = new int[nColumns];
            for (int j = 0; j < nColumns; j++)
            {
                for (int i = 0; i < nRows; i++)
                {
                    widths[j] = Math.Max(widths[j], sparseMatrix.At(i, j).ToString().Length);
                }
            }

            var sb = new StringBuilder();
            for (int i = 0; i < nRows; i++)
            {
                sb.Append(sparseMatrix.At(i, 0).ToString().PadLeft(widths[0]));
                for (int j = 1; j < nColumns; j++) // we add the first element already, so start from 1.
                {
                    sb.Append($"\t{sparseMatrix.At(i, j).ToString().PadLeft(widths[j])}");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        internal static string PrintDenseMatrix2(this CSparse.Double.DenseMatrix denseMatrix)
        {
            // calculate maximum width.
            int nColumns = denseMatrix.ColumnCount;
            int nRows = denseMatrix.RowCount;

            var widths = new int[nColumns];
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nColumns; j++)
                {
                    widths[j] = Math.Max(widths[j], denseMatrix.At(i, j).ToString().Length);
                }
            }

            var sb = new StringBuilder();
            for (int i = 0; i < nRows; i++)
            {
                sb.Append(denseMatrix.At(i, 0).ToString().PadLeft(widths[0]));
                for (int j = 1; j < nColumns; j++) // we add the first element already, so start from 1.
                {
                    sb.Append($"\t\t{denseMatrix.At(i, j).ToString().PadLeft(widths[j])}");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        internal static string PrintVector(this double[] vec)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vec.Length; i++)
            {
                sb.AppendLine(vec[i].ToString());
            }
            return sb.ToString();
        }
#endif

    }
}
