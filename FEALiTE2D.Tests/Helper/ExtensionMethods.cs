using System;
using System.Text;
using static System.Math;

namespace FEALiTE2D.Tests.Helper
{
    public static class ExtensionMethods
    {
        public static string PrintSparseMatrix(this CSparse.Double.SparseMatrix sparseMatrix)
        {
            // calculate maximum width.
            var nColumns = sparseMatrix.ColumnCount;
            var nRows = sparseMatrix.RowCount;

            var widths = new int[nColumns];
            for (var i = 0; i < nRows; i++)
            {
                for (var j = 0; j < nColumns; j++)
                {
                    widths[j] = Math.Max(widths[j], sparseMatrix.At(i, j).ToString().Length);
                }
            }

            var sb = new StringBuilder();
            for (var i = 0; i < nRows; i++)
            {
                sb.Append(sparseMatrix.At(i, 0).ToString().PadLeft(widths[0]));
                for (var j = 1; j < nColumns; j++) // we add the first element already, so start from 1.
                {
                    sb.Append($"\t\t{sparseMatrix.At(i, j).ToString().PadLeft(widths[j])}");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static string PrintDenseMatrix(this CSparse.Double.DenseMatrix denseMatrix)
        {
            // calculate maximum width.
            var nColumns = denseMatrix.ColumnCount;
            var nRows = denseMatrix.RowCount;

            var widths = new int[nColumns];
            for (var i = 0; i < nRows; i++)
            {
                for (var j = 0; j < nColumns; j++)
                {
                    widths[j] = Math.Max(widths[j], denseMatrix.At(i, j).ToString().Length);
                }
            }

            var sb = new StringBuilder();
            for (var i = 0; i < nRows; i++)
            {
                sb.Append(denseMatrix.At(i, 0).ToString().PadLeft(widths[0]));
                for (var j = 1; j < nColumns; j++) // we add the first element already, so start from 1.
                {
                    sb.Append($"\t\t{denseMatrix.At(i, j).ToString().PadLeft(widths[j])}");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public static bool IsSymmetric(this CSparse.Double.DenseMatrix denseMatrix)
        {
            // calculate maximum width.
            var nColumns = denseMatrix.ColumnCount;
            var nRows = denseMatrix.RowCount;

            if (nColumns != nRows)
            {
                return false;
            }

            for (var i = 0; i < nRows; i++)
            {
                for (var j = 0; j < nColumns; j++)
                {
                    if ((Abs(denseMatrix[i, j]) - Abs(denseMatrix[j, i])) > 1e-8)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string PrintVector(this double[] vector)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < vector.Length; i++)
            {
                sb.AppendLine(vector[i].ToString());
            }
            return sb.ToString();
        }

    }
}
