using System;
using System.Collections.Generic;
using System.Text;

namespace FEALiTE2D.Plotting.Dxf
{
    /// <summary>
    /// A class for plotting options for dxf file format
    /// </summary>
    public class PlottingOption
    {
        /// <summary>
        /// Create new instance of <see cref="PlottingOption"/>
        /// </summary>
        public PlottingOption()
        {
            DxfVersion = netDxf.Header.DxfVersion.AutoCad2010;
            StructureScaleFactor = 1.0;
            NFDScaleFactor = 0.5;
            SFDScaleFactor = 0.5;
            BMDScaleFactor = 0.1;
            DisplacmentScaleFactor = 100.0;
            DiagramsHorizontalOffsets = DiagramsVerticalOffsets = 2;
            LayerOfStructure = new netDxf.Tables.Layer("FEALiTE-Structure")
            {
                Color = netDxf.AciColor.Cyan,
                Lineweight = netDxf.Lineweight.W35,
            };
            LayerOfNFD = new netDxf.Tables.Layer("FEALiTE-NFD")
            {
                Color = netDxf.AciColor.Yellow,
                Lineweight = netDxf.Lineweight.W15,
            };
            LayerOfSFD = new netDxf.Tables.Layer("FEALiTE-SFD")
            {
                Color = netDxf.AciColor.Blue,
                Lineweight = netDxf.Lineweight.W15,
            };
            LayerOfBMD = new netDxf.Tables.Layer("FEALiTE-BMD")
            {
                Color = netDxf.AciColor.Red,
                Lineweight = netDxf.Lineweight.W15,
            };
            LayerOfDisplacement = new netDxf.Tables.Layer("FEALiTE-Displacement")
            {
                Color = netDxf.AciColor.Green,
                Lineweight = netDxf.Lineweight.W15,
            };
        }

        /// <summary>
        /// Version of the dxf file.
        /// </summary>
        public netDxf.Header.DxfVersion DxfVersion { get; set; }

        /// <summary>
        /// Layer of the structure.
        /// </summary>
        public netDxf.Tables.Layer LayerOfStructure { get; set; }

        /// <summary>
        /// Layer of the normal force diagram.
        /// </summary>
        public netDxf.Tables.Layer LayerOfNFD { get; set; }

        /// <summary>
        /// Layer of the shear force diagram.
        /// </summary>
        public netDxf.Tables.Layer LayerOfSFD { get; set; }

        /// <summary>
        /// Layer of the bending moment diagram.
        /// </summary>
        public netDxf.Tables.Layer LayerOfBMD { get; set; }

        /// <summary>
        /// Layer of the displacement.
        /// </summary>
        public netDxf.Tables.Layer LayerOfDisplacement { get; set; }

        /// <summary>
        /// Scale factor to draw the statical system of the structure.
        /// </summary>
        public double StructureScaleFactor { get; set; }

        /// <summary>
        /// Scale factor to draw normal force diagram
        /// </summary>
        public double NFDScaleFactor { get; set; }

        /// <summary>
        /// Scale factor to draw bending moment diagram.
        /// </summary>
        public double BMDScaleFactor { get; set; }

        /// <summary>
        /// Scale factor to draw shear force diagram.
        /// </summary>
        public double SFDScaleFactor { get; set; }

        /// <summary>
        /// Scale factor to draw displacement of the structure.
        /// </summary>
        public double DisplacmentScaleFactor { get; set; }

        /// <summary>
        /// X-offset between diagrams.
        /// </summary>
        public double DiagramsHorizontalOffsets { get; set; }

        /// <summary>
        /// Y-offset between diagrams.
        /// </summary>
        public double DiagramsVerticalOffsets { get; set; }
    }
}
