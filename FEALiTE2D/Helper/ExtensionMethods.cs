using System.Globalization;
using System.Text;

namespace FEALiTE2D.Helper
{
    /// <summary>
    /// some extension methods to facilitate the work
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class ExtensionMethods
    {
        /// <summary>
        /// Scale a matrix by a factor
        /// </summary>
        /// <param name="matrix">the matrix to be scaled</param>
        /// <param name="alpha">the scale factor</param>
        public static void ScaleMatrix(this CSparse.Storage.CompressedColumnStorage<double> matrix, double alpha)
        {
            for (var i = 0; i < matrix.Values.Length; i++)
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
            for (var i = 0; i < matrix.Values.Length; i++)
            {
                matrix.Values[i] *= alpha;
            }
        }

#if DEBUG

        internal static string PrintSparseMatrix2(this CSparse.Double.SparseMatrix sparseMatrix)
        {
            // calculate maximum width.
            var nColumns = sparseMatrix.ColumnCount;
            var nRows = sparseMatrix.RowCount;

            var widths = new int[nColumns];
            for (var j = 0; j < nColumns; j++)
            {
                for (var i = 0; i < nRows; i++)
                {
                    widths[j] = System.Math.Max(widths[j], sparseMatrix.At(i, j).ToString(CultureInfo.InvariantCulture).Length);
                }
            }

            var sb = new StringBuilder();
            for (var i = 0; i < nRows; i++)
            {
                sb.Append(sparseMatrix.At(i, 0).ToString(CultureInfo.InvariantCulture).PadLeft(widths[0]));
                for (var j = 1; j < nColumns; j++) // we add the first element already, so start from 1.
                {
                    sb.Append($"\t{sparseMatrix.At(i, j).ToString(CultureInfo.InvariantCulture).PadLeft(widths[j])}");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        internal static string PrintDenseMatrix2(this CSparse.Double.DenseMatrix denseMatrix)
        {
            // calculate maximum width.
            var nColumns = denseMatrix.ColumnCount;
            var nRows = denseMatrix.RowCount;

            var widths = new int[nColumns];
            for (var i = 0; i < nRows; i++)
            {
                for (var j = 0; j < nColumns; j++)
                {
                    widths[j] = System.Math.Max(widths[j], denseMatrix.At(i, j).ToString(CultureInfo.InvariantCulture).Length);
                }
            }

            var sb = new StringBuilder();
            for (var i = 0; i < nRows; i++)
            {
                sb.Append(denseMatrix.At(i, 0).ToString(CultureInfo.InvariantCulture).PadLeft(widths[0]));
                for (var j = 1; j < nColumns; j++) // we add the first element already, so start from 1.
                {
                    sb.Append($"\t\t{denseMatrix.At(i, j).ToString(CultureInfo.InvariantCulture).PadLeft(widths[j])}");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        internal static string PrintVector(this double[] vec)
        {
            var sb = new StringBuilder();
            foreach (var t in vec)
            {
                sb.AppendLine(t.ToString(CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }
#endif

    }
}
