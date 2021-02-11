
namespace FEALiTE2D.Helper
{
    /// <summary>
    /// this class represents an linear equation such Y=mX+b where m is the slope and b is a constant.
    /// </summary>
    internal class LinearFunction
    {
        /// <summary>
        /// Creates a new instance of <see cref="LinearFunction"/> using a slope and a constant
        /// </summary>
        /// <param name="m">the slope</param>
        /// <param name="b">a constant</param>
        internal LinearFunction(double m, double b)
        {
            this.slope = m;
            this.constant = b;
        }

        /// <summary>
        /// Creates a new instance of <see cref="LinearFunction"/> using tow points
        /// Y = mX+b
        /// 1- find the slope   y|
        ///     y2-y1            |   y1      |y2
        /// m =--------          |-----|-----|------>x
        ///     x2-x1                x1       x2
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        internal LinearFunction(double x1, double x2, double y1, double y2)
        {
            this.slope = (y2 - y1) / (x2 - x1);
            //b = Y-mX
            this.constant = y1 - slope * x1;
        }

        internal double slope { get; set; }
        internal double constant { get; set; }

        /// <summary>
        /// get value at a distance
        /// </summary>
        /// <param name="x">distant from the left side</param>
        internal double GetValueAt(double x)
        {
            // Y = mX+b
            return slope * x + constant;
        }
    }
}
