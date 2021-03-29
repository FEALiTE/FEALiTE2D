﻿using FEALiTE2D.Materials;
using System;

namespace FEALiTE2D.CrossSections
{
    /// <summary>
    /// Represent an European I beams section.
    /// </summary>
    /// <seealso cref="FEALiTE2D.CrossSections.IFrame2DSection" />
    [System.Serializable]
    public class IPESection : IFrame2DSection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IPESection"/> class.
        /// </summary>
        /// <param name="tf">flange thickness.</param>
        /// <param name="tw">web thickness.</param>
        /// <param name="b">flange width.</param>
        /// <param name="h">total hight of the web.</param>
        /// <param name="r">fillet radius</param>
        /// <param name="material">the material.</param>
        public IPESection(double tf, double tw, double b, double h, double r, IMaterial material)
        {
            this.tf = tf;
            this.tw = tw;
            this.b = b;
            this.h = h;
            this.r = r;
            base.Material = material;

            // calculate section properties and set them here 
            // to avoid recalculation in each time they are called to save time.
            SetSectionProperties(tf, tw, b, h, r);
        }

        /// <summary>
        /// flange thickness.
        /// </summary>
        public double tf { get; set; }

        /// <summary>
        /// web thickness.
        /// </summary>
        public double tw { get; set; }

        /// <summary>
        /// total hight of the web.
        /// </summary>
        public double h { get; set; }

        /// <summary>
        /// flange width.
        /// </summary>
        public double b { get; set; }

        /// <summary>
        /// fillet radius
        /// </summary>
        public double r { get; set; }

        /// <summary>
        /// Sets the section properties.
        /// </summary>
        /// <param name="tf">flange thickness.</param>
        /// <param name="tw">web thickness.</param>
        /// <param name="b">flange width.</param>
        /// <param name="h">total hight of the web.</param>
        /// <param name="r">fillet radius</param>
        private void SetSectionProperties(double tf, double tw, double b, double h, double r)
        {
            base.A = 2 * tf * b + (h - 2 * tf) * tw + (4 - Math.PI) * r * r;
            base.Ay = base.A - 2 * b * tf + (tw + 2 * r) * tf;
            base.Ax = 0.6667 * b * tf;
            base.Ix = (b * h * h * h - (b - tw) * Math.Pow((h - 2 * tf), 3)) / 12 + 0.03 * Math.Pow(r, 4) + 0.2146 * r * r * Math.Pow((h - 2 * tf - 0.4468 * r), 2);
            base.Iy = (2 * tf * b * b * b + (h - 2 * tf) * tw * tw * tw) / 12 + 0.03 * Math.Pow(r, 4) + 0.2146 * r * r * Math.Pow((tw + 0.4468 * r), 2);
            base.J = 0.6667 * (b - 0.63 * tf) * tf * tf * tf + (h - 2 * tf) * tw * tw * tw / 3 + 2 * (tw / tf) * (0.145 + 0.1 * r * tf) * Math.Pow(
                (Math.Pow(r + tw / 2, 2) + Math.Pow(r + tf, 2) - r * r) / (2 * r + tf)
                , 4);
            base.MaxHeight = h;
            base.MaxWidth = b;
        }
    }
}
